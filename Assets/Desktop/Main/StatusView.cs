using EVRC.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class StatusView : MonoBehaviour
    {
        // Public fields
        public GameStateBase gameState;
        [HideInInspector] public string statusUxmlLabelName; //Custom inspector lets you choose these fields from a dropdown

        // Private fields
        private Label _statusLabel;
        [SerializeField] internal UIDocument uiDocument;

        private void OnValidate()
        {
            uiDocument = GetComponentInParent<UIDocument>();
            EnsureUIDocument();
        }

        private void EnsureUIDocument()
        {
            if (uiDocument == null)
            {
                Debug.LogWarning("Unable to find UIDocument in parent hierarchy.");
            }
        }

        private void OnEnable()
        {
            VisualElement root = uiDocument.rootVisualElement;

            // Bind Status Values
            _statusLabel = root.Query<Label>(statusUxmlLabelName).First();

            OnEnablePostChecks();
        }

        private void OnEnablePostChecks()
        {
            if (_statusLabel == null )
            {
                UnityEngine.Debug.LogError("Status Label not found in UI Document. Check the provided 'status Uxml label name' in the inspector");
            }

            Refresh();

        }

        public void Refresh()
        {
            _statusLabel.text = gameState.GetStatusText();
        }
    }
}
