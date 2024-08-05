using EVRC.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Valve.Newtonsoft.Json.Linq;

namespace EVRC.Desktop
{
    public class SteamViewContainer : PreCheck
    {
        // Open VR tracks the device properties, but only when SteamVR is running
        public OpenVrState openVrState;
        private JObject vrSettings;
        
        public string hmdModel;
        public List<string> currentURLs;

        // This is the key that should be in the steamvr.vrsettings file and is used to identify the selected bindings for this game
        public string ApplicationName = "application.generated.unity.elitevrcockpit.exe";

        [SerializeField] UIDocument parentUIDocument;
        Label deviceValueElement;
        Label steamBindingVersionElement;
        VisualElement errorDialogElement;
        Label errorMessageElement;
        TextInputBaseField<string> alternatePathElement;


        public void OnEnable()
        {
            hasErrors = false;
          
            VisualElement root = parentUIDocument.rootVisualElement;

            deviceValueElement = root.Q<Label>("vr-device-value");
            steamBindingVersionElement = root.Q<Label>("steamvr-binding-version");
            errorDialogElement = root.Q<VisualElement>("error-dialog");
            errorMessageElement = root.Q<Label>("error-message");
            alternatePathElement = root.Q<TextInputBaseField<string>>("alt-path-input");

            ReadSteamVRConfigFile();
            deviceValueElement.text = hmdModel == null ? null : hmdModel;

            if (currentURLs.Count > 1)
            {
                string concatenatedURLs = string.Join("\n", currentURLs);
                concatenatedURLs = "Multiple Current Bindings found \n" + concatenatedURLs;
                steamBindingVersionElement.text = concatenatedURLs;
            } else if (currentURLs.Count == 0) {
                Debug.Log("No CurrentURL Found for SteamVR");
            } 
            else
            {
                steamBindingVersionElement.text = currentURLs[0];
            }
            

        }

        public void ReadSteamVRConfigFile()
        {
            if (File.Exists(Paths.SteamVRConfigPath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(Paths.SteamVRConfigPath);
                    vrSettings = JObject.Parse(jsonContent);

                    Debug.Log($"Read steamvr.vrsettings from {Paths.SteamVRConfigPath}");

                    ParseLastKnown();
                    GetCurrentURLs();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Failed to read steamvr.vrsettings: " + e.Message);
                }
            }
            else
            {
                Debug.LogWarning("steamvr.vrsettings file not found at " + Paths.SteamVRConfigPath);

                // Display the dialog
                errorDialogElement.style.display = DisplayStyle.Flex;
                errorMessageElement.text = "steamvr.vrsettings file not found at " + Paths.SteamVRConfigPath;
            }
        }

        private void ParseLastKnown()
        {
            // Navigate to the "LastKnown" -&gt; "HMDModel" key
            JToken lastKnown = vrSettings["LastKnown"];
            if (lastKnown != null && lastKnown["HMDModel"] != null)
            {
                hmdModel = lastKnown["HMDModel"].ToString();
            }
            else
            {
                hmdModel = "Not Available";
            }
        }

        public void GetCurrentURLs()
        {
            currentURLs = new List<string>();

            if (vrSettings != null)
            {
                JObject applicationBindings = (JObject)vrSettings[ApplicationName];

                if (applicationBindings != null)
                {
                    foreach (var controllerBinding in applicationBindings)
                    {
                        string key = controllerBinding.Key;
                        string value = controllerBinding.Value.ToString();

                        if (key.Contains("_CurrentURL_steamvrinput"))
                        {
                            string[] parts = value.Split(new[] { "://" }, StringSplitOptions.None);
                            if (parts.Length > 1)
                        {
                                currentURLs.Add(parts[1]);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"No bindings found for {ApplicationName}.");
                }
            }
            else
            {
                Console.WriteLine("vrSettings is null.");
            }
        }
    }

}
