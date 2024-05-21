using EVRC.Core;
using EVRC.Core.Overlay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class ControlButtonSettings : MonoBehaviour
    {
        // Stuff to interact with all of the scene elements
        [SerializeField] UIDocument parentUIDocument;
        private VisualElement root; // the root of the whole UI
        [SerializeField] BoolGameSetting buttonLabelSetting;
        [SerializeField] SavedGameState savedState;

        // Elements related to settings that can be configured
        Toggle buttonLabelToggleElement;


        public void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;

            buttonLabelToggleElement = root.Q<Toggle>("toggle-labels");
            buttonLabelToggleElement.SetEnabled(false);
        }
        
        public void OnOverlayStateLoaded()
        {
            Debug.Log("ControlButtonSettings OverlayStateLoaded");

            // Get the value from the SavedState file
            buttonLabelToggleElement.value = GetOrCreateSettingValue(buttonLabelSetting.saveFileKey);
            //Enable to toggle element, so it's clickable
            buttonLabelToggleElement.SetEnabled(true);

            buttonLabelToggleElement.RegisterValueChangedCallback(buttonLabelSetting.OnToggle);
        }

        private bool GetOrCreateSettingValue(string saveFileKey)
        {
            // Get the value from the file before registering a callback for future changes
            SavedBooleanSetting settingFromFile = savedState.booleanSettings.FirstOrDefault(x => x.name == saveFileKey);

            if (!settingFromFile.Equals(default(SavedBooleanSetting)))
            {
                // The setting was found, take an action
                //Debug.Log($"Setting found: {saveFileKey} with value: {settingFromFile.value}");
                buttonLabelToggleElement.value = settingFromFile.value;
                return settingFromFile.value;
            }
            else
            {

                Debug.Log($"Setting with name '{saveFileKey}' not found. Adding to SavedState with value false");
                savedState.booleanSettings.Add(new SavedBooleanSetting() { name = saveFileKey, value = false });
                savedState.Save();

                return false;
            }
        }
        


    }
}
