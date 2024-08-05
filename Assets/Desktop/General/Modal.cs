using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public abstract class Modal : MonoBehaviour
    {
        [SerializeField] protected UIDocument parentUIDocument;
        [Description("UXML doc template for this modal")]
        [SerializeField] VisualTreeAsset modalTemplate;

        protected VisualElement modalUI;

        Button okButton;
        Button closeButton;
        Button cancelButton;

        public virtual void OnEnable()
        {
            VisualElement root = parentUIDocument.rootVisualElement;
        }

        public virtual void LaunchModal()
        {
            modalUI = modalTemplate.Instantiate();

            okButton = modalUI.Q<Button>("submit-button");
            if (okButton == null) { Debug.LogWarning($"{this.name} - cannot find button with name 'submit-button'"); }
            cancelButton = modalUI.Q<Button>("cancel-button");
            if (cancelButton == null) { Debug.LogWarning($"{this.name} - cannot find button with name 'cancel-button'"); }

            okButton.clicked += Submit;
            cancelButton.clicked += Close;

            Vector2 modalPosition = Utils.FindCenterForModal(parentUIDocument);
            modalUI.style.position = Position.Absolute;
            modalUI.style.left = modalPosition.x;
            modalUI.style.top = modalPosition.y;

            parentUIDocument.rootVisualElement.Add(modalUI);
        }

        public abstract void Submit();
        public virtual void Close()
        {
            modalUI.RemoveFromHierarchy();
        }
    }
}