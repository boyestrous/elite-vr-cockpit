using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class PreLaunchController : MonoBehaviour
    {
        private GameObject parentObject; // Reference to the parent object with UIDocument
        private UIDocument mainUIDocument;
        private StartStopButton startStopButton;
        private PreCheck preCheckComponent;

        private Label errorText;
        private VisualElement errorIcon;

        public string tabname;

        private void OnEnable()
        {
            // Find the parent object with UIDocument component
            FindParentWithUIDocument(transform);
            startStopButton = mainUIDocument.GetComponent<StartStopButton>();

            VisualElement root = mainUIDocument.rootVisualElement;
            errorText = root.Q<Label>("error-text");

            // Get the related precheck component (on this same gameObject)
            preCheckComponent = gameObject.GetComponent<PreCheck>();
            if (preCheckComponent == null ) { Debug.LogWarning($"Precheck component NOT found for: {gameObject.name}"); }

            if (tabname == null) { Debug.LogError($"tab name must be specified for {gameObject.name}"); }
            else { errorIcon = root.Q<VisualElement>(tabname.ToLower()+"-error-icon"); }
            
        }

        public void SetReady(bool status)
        {
            if (status)
            {
                startStopButton.DisableStartButton();
                errorText.style.display = DisplayStyle.Flex;
                errorIcon.style.display = DisplayStyle.Flex;
            }
            else
            {
                startStopButton.EnableStartButton();
                errorText.style.display = DisplayStyle.None;
                errorIcon.style.display = DisplayStyle.None;
            }
        }

        private void FindParentWithUIDocument(Transform currentTransform)
        {
            // Base case: If the current transform has a UIDocument component, return it
            UIDocument uiDocument = currentTransform.GetComponent<UIDocument>();
            if (uiDocument != null)
            {
                //Debug.Log($"Found parentObject: {currentTransform.gameObject.name}");
                parentObject = currentTransform.gameObject;
                mainUIDocument = uiDocument;
                return;
            }

            // Recursive case: Search parent transforms
            if (currentTransform.parent != null)
            {
                FindParentWithUIDocument(currentTransform.parent);
            } 
            else
            {
                Debug.LogError("Unable to find the UI Document for the pre-launch checker");
            }
        }
    }
}
