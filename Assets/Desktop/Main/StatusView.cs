using EVRC.Core;
using System;
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
        internal Label _statusLabel;
        [SerializeField] internal UIDocument uiDocument;

        internal void OnEnable()
        {
            try
            {
                VisualElement root = uiDocument.rootVisualElement;
            
                // Bind Status Values
                _statusLabel = root.Query<Label>(statusUxmlLabelName).First();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to find UIDocument in parent hierarchy. {e}");
            }

            VerifyAfterEnable();
        }

        private void VerifyAfterEnable()
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
