using EVRC.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class StartStopButton : MonoBehaviour
    {
        public GameEvent StartOpenVREvent;
        public GameEvent StopOpenVREvent;
        public OpenVrState openVrState;

        private VisualElement root;
        private Button button;
        private IMGUIContainer icon;
        private bool running = false;
        private bool preLaunchErrors = true;

        [Header("GUI Settings")]
        public string startButtonText;
        public string stopButtonText;



        private void OnEnable()
        {
            UIDocument menu = GetComponent<UIDocument>();
            root = menu.rootVisualElement;

            // button and icon references
            button = root.Q<Button>("OpenVRButton");
            icon = root.Q<IMGUIContainer>("button-icon");

            // Attach an event listener to the button's onClick event
            button.clicked += OnButtonClick;

        }

        public void SetPreLaunchStatus(bool hasErrors)
        {
            preLaunchErrors = hasErrors;
            SetButtonStyle();

        }

        private void OnButtonClick() 
        { 
            if (running)
            {
                // Send stopped event
                StopOpenVREvent.Raise();
                running = false;
            } 
            else
            {
                StartOpenVREvent.Raise();
                running = true;
            }
            SetButtonStyle(); 
        }

        private void SetButtonStyle()
        {

            if (preLaunchErrors)
            {
                DisableStartButton();
            } 
            else if (running)
            {
                ApplyRunningStyle();
            } 
            else
            {
                EnableStartButton();
                ApplyStoppedStyle();
            }

            
        }

        private void DisableStartButton()
        {
            if (button == null) { return; }
            button.SetEnabled(false);
            ApplyDisabledStyle();
        }

        private void EnableStartButton()
        {
            if (button == null) { return; }
            button.SetEnabled(true);
            ApplyStoppedStyle();
        }

        /// <summary>
        /// Style for when OpenVR is running
        /// </summary>
        private void ApplyRunningStyle()
        {
            button.RemoveFromClassList("startButtonBorder");            
            icon.RemoveFromClassList("startIcon");            

            button.AddToClassList("stopButtonBorder");
            icon.AddToClassList("stopIcon");
            button.text = stopButtonText;
        }

        /// <summary>
        /// Style for when OpenVR is stopped
        /// </summary>
        private void ApplyStoppedStyle()
        {           
            button.RemoveFromClassList("stopButtonBorder");
            icon.RemoveFromClassList("stopIcon");
            button.RemoveFromClassList("disabledButtonBorder");
            button.RemoveFromClassList("disabledButton");
            icon.RemoveFromClassList("disabledIcon");

            button.AddToClassList("startButtonBorder");
            icon.AddToClassList("startIcon");

            button.text = startButtonText;
        }

        // <summary>
        /// Style for when the start button is disabled
        /// </summary>
        private void ApplyDisabledStyle()
        {
            button.RemoveFromClassList("startButtonBorder");
            icon.RemoveFromClassList("startIcon");
            button.RemoveFromClassList("stopButtonBorder");
            icon.RemoveFromClassList("stopIcon");

            button.AddToClassList("disabledButtonBorder");
            button.AddToClassList("disabledButton");
            icon.AddToClassList("disabledIcon");
            button.text = startButtonText;
        }
    }
}
