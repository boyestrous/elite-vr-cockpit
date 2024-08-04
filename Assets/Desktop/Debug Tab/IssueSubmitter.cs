using EVRC.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class IssueSubmitter : MonoBehaviour
    {

        [SerializeField] public UIDocument parentUIDocument;

        [SerializeField] private EliteDangerousState eliteDangerousState;
        [SerializeField] private ControlBindingsState controlBindingsState;
        [SerializeField] private VJoyState vjoyState;
        [SerializeField] private LogState logState;

        // VisualElements
        private VisualElement root; // the root of the whole UI
        private Button githubButton;

        public void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;

            githubButton = root.Q<Button>("submit-github-issue");

            githubButton.clicked += SubmitIssue;

        }


        public void SubmitIssue()
        {
            string base_url = Paths.githubUrl;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Default issue title
            string defaultTitle = $"New Issue - opened: ";

            // Default issue body
            string bodyText = "**Please descript your issue in the space below.** \n - What happened? \n - What were you doing? \n - Has it worked in the past? \n - etc.";


            bodyText += "\n\n\n\n";
            bodyText += "**------------- Configuration and Logs ---------------** \n";
            bodyText += $"    - _vJoy Status_:       {vjoyState.vJoyStatus} \n";
            bodyText += $"    - _Process Directory_: {eliteDangerousState.processDirectory} \n";
            bodyText += $"    - _Process Name_:      {eliteDangerousState.processName} \n";
            bodyText += $"    - _Bindings File_:     {controlBindingsState.bindingsFileName} \n";
            bodyText += $"    - _Start Preset_:      {controlBindingsState.startPreset} \n";

            bodyText += "\n\n";

            string logHistory = logState.GetAllLogs();
            bodyText += logHistory + "\n";

            // URL encode the title and body
            string encodedTitle = WWW.EscapeURL(defaultTitle + timestamp);
            string encodedBody = WWW.EscapeURL(bodyText);

            string url = base_url + $"?title={encodedTitle}&body={encodedBody}";

            Application.OpenURL(url);
        }

        public string BindsToString()
        {
            return "";
        }
    }


}