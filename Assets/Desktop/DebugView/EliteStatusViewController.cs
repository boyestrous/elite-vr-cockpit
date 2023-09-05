using EVRC.Core;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class EliteStatusViewController : MonoBehaviour
    {
        [SerializeField] public UIDocument parentUIDocument;
        [SerializeField] public EliteDangerousState eliteDangerousState;


        private VisualElement root; // the root of the whole UI
        private Label timestamp;
        private Label guiFocus;
        private Label statusFlags;
        private Label processActive;
        private Label processName;
        private Label processId;
        private Label processDir;

        public void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;
            timestamp = root.Q<Label>("timestamp-value");
            guiFocus = root.Q<Label>("guifocus-value");
            statusFlags = root.Q<Label>("statusflags-value");
            // @todo Uncomment this when Flags2 are added to the EliteDangerousState
            //statusFlags2 = root.Q<Label>("statusFlags2-value");
            processActive = root.Q<Label>("process-active-value");
            processName = root.Q<Label>("process-name-value");
            processId = root.Q<Label>("process-id-value");
            processDir = root.Q<Label>("process-directory-value");

            if (eliteDangerousState == null) 
            {
                Debug.LogError($"Elite State Object is missing on object: {this.name}");
            }
        }

        public void Refresh()
        {
            timestamp.text = eliteDangerousState.lastStatusFromFile.timestamp;
            guiFocus.text = eliteDangerousState.guiFocus.ToString();
            statusFlags.text = FormatStatusFlagsForHumans<EDStatusFlags>(eliteDangerousState.statusFlags);
            //statusFlags2.text = FormatStatusFlagsForHumans<EDStatusFlags2>(eliteDangerousState.statusFlags2);
            processActive.text = eliteDangerousState.running.ToString();
            processName.text = eliteDangerousState.processName;
            processId.text = eliteDangerousState.processId.ToString();
            processDir.text = eliteDangerousState.processDirectory;
        }

        public string FormatStatusFlagsForHumans<T>(T flags) where T : Enum
        {

            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enum type");
            }

            StringBuilder combinedFlags = new StringBuilder();
            bool isFirstFlag = true;

            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (flags.HasFlag(value))
                {
                    if (!isFirstFlag)
                    {
                        combinedFlags.Append(" | ");
                    }

                    combinedFlags.Append(value.ToString());
                    isFirstFlag = false;
                }
            }

            return combinedFlags.ToString();
            
        }
    }
}
