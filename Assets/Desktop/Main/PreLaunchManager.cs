using EVRC.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class PreLaunchManager : MonoBehaviour
    {
        [SerializeField] PreCheck[] preChecks;
        bool currentErrors = true;
        [Description("Icon for tabs with errors")]
        public Texture2D errorIconTexture;
        public Color errorIconColor;
        


        [Header("---These can be made private after initial debugging---")]
        // Visual Component References
        public StartStopButton startStopButton; // Technically the class that manages the visual start btn
        public UIDocument parentUIDocument;
        private VisualElement root;
        private List<MainViewTab> preCheckTabs;
        public List<PreCheck> tabsWithErrors;
        public List<VisualElement> errorIcons; // Track all of the icons currently on tabs


        struct MainViewTab
        {
            public string name;
            public Label labelRef;
        }

        private void OnEnable()
        {
            parentUIDocument = GetComponent<UIDocument>();
            if (parentUIDocument == null)
            {
                Debug.LogError($"Could not find Parent UI Document for: {this.name}");
            }
            root = parentUIDocument.rootVisualElement;

            Debug.Log("Finding PreCheck scripts in child hierarchy");

            _ = CoreUtils.DelayAndExecute(FindPreCheckScripts, 1);
        }

        public void FindPreCheckScripts()
        {
            preCheckTabs = new List<MainViewTab>();
            tabsWithErrors = new List<PreCheck>();
            errorIcons = new List<VisualElement>();

            startStopButton = GetComponent<StartStopButton>();

            preChecks = FindObjectsOfType<PreCheck>(true);

            foreach (PreCheck preCheck in preChecks)
            {
                // force tab name to Camel Case, in case it's necessary
                string formatTabName = char.ToUpper(preCheck.tabName[0]) + preCheck.tabName[1..].ToLower();

                var newtab = new MainViewTab()
                {
                    name = preCheck.tabName,
                    labelRef = root.Q<Label>(formatTabName + "Tab"),
                };
                preCheckTabs.Add(newtab);

                preCheck.updatePreCheckState += OnPreCheckStateChanged;
            }

            CheckForErrors();
        }

        public void CheckForErrors()
        {
            //currentErrors = preChecks.Any(x => x.HasErrors());
            tabsWithErrors = preChecks.Where(p => p.hasErrors).ToList();
            AddErrorIcons();
            SetStartButtonState();
        }

        private void AddErrorIcons()
        {
            // Destroy the existing VisualElements 
            errorIcons.ForEach(p => { p.RemoveFromHierarchy(); });
            // Clear the list of VisualElements
            errorIcons.Clear();

            foreach(PreCheck preCheck in tabsWithErrors)
            {
                VisualElement errorIcon = new VisualElement();
                errorIcon.style.width = 20;
                errorIcon.style.height = 20;
                errorIcon.style.marginLeft = 15;
                errorIcon.style.backgroundImage = errorIconTexture;
                errorIcon.style.unityBackgroundImageTintColor = errorIconColor;

                errorIcons.Add(errorIcon);

                var tab = preCheckTabs.Find(tab => tab.name == preCheck.tabName);
                tab.labelRef.Add(errorIcon);
            }
        }

        private void SetStartButtonState()
        {
            startStopButton.SetPreLaunchStatus(tabsWithErrors.Count > 0);
        }

        private void OnPreCheckStateChanged()
        {
            CheckForErrors();
        }
    }
}
