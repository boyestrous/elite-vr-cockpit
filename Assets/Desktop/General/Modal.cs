using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public abstract class Modal : MonoBehaviour
    {
        [Header("Base Modal Configuration")]
        [SerializeField] protected UIDocument parentUIDocument;
        [Description("The header bar that allows dragging the window and includes the 'x' to close")]
        [SerializeField] VisualTreeAsset modalHeader;
        [SerializeField] bool headerEnabled = true;


        // UI Elements
        protected VisualElement root;
        protected VisualElement modalUI;
        protected Button okButton;
        protected Button closeButton;
        protected Button cancelButton;

        // Draggable Modal Stuff
        private bool isDragging;
        private Vector2 originalMousePosition;
        private Vector2 originalElementPosition;

        [Header("Additional Configuration")]
        [Description("UXML doc template for this modal")]
        [SerializeField] VisualTreeAsset modalContent;
        private Action _onClose;
        private Action _onSubmit;
        private Action _onCancel;

        public virtual void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;
            
            if (modalHeader == null && headerEnabled)
            {
                Debug.LogWarning($"Header template is missing for modal in {gameObject.name} ({GetType().Name})");
            }

        }

        public virtual void LaunchModal()
        {
            modalUI = modalContent.Instantiate();

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

            if (headerEnabled && modalHeader != null)
            {
                AddHeader();
            }

            parentUIDocument.rootVisualElement.Add(modalUI);
        }

        /// <summary>
        /// Pass a callback method that will be called when closing the modal
        /// </summary>
        /// <param name="onClose"></param>
        public virtual void LaunchModal(Action onClose)
        {
            _onClose = onClose;
            LaunchModal();
        }

        /// <summary>
        /// Separate methods to be called when submitting or canceling the modal
        /// </summary>
        /// <param name="onClose"></param>
        public virtual void LaunchModal(Action onSubmit, Action onCancel)
        {
            _onSubmit = onSubmit;
            _onCancel = onCancel;
            LaunchModal();
        }

        private void AddHeader()
        {
            VisualElement headerBar = modalHeader.Instantiate();
            modalUI.Insert(0, headerBar);

            closeButton = modalUI.Q<Button>("x-button");
            closeButton.clicked += Close;

            // Register the necessary event handlers
            headerBar.RegisterCallback<PointerDownEvent>(OnPointerDown);
            root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            root.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        public abstract void Submit();
        public virtual void Close()
        {
            modalUI.RemoveFromHierarchy();

            // Call the callbacks if not null
            _onClose?.Invoke();
            _onCancel?.Invoke();
            _onSubmit?.Invoke();

            // Optional: Reset the callback after invoking it
            _onClose = null;
            _onCancel = null;
            _onSubmit = null;
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            //Debug.Log($"Pointer Down event detected on modal header in {gameObject.name} ({GetType().Name})");
            isDragging = true;
            originalMousePosition = evt.position;
            originalElementPosition = modalUI.transform.position;
            root.CaptureMouse();
            //Debug.Log("PointerDown - Mouse Captured");
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            //Debug.Log($"Pointer moving for: {gameObject.name} ({GetType().Name})");
            if (isDragging)
            {
                Vector2 delta = (Vector2)evt.position - originalMousePosition;
                Vector3 newPosition = (Vector3)originalElementPosition + new Vector3(delta.x, delta.y, 0);
                modalUI.transform.position = newPosition;
                //Debug.Log($"   -------> isDragging: {originalElementPosition + delta}");
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            //Debug.Log($"Pointer Up event detected on modal header in {gameObject.name} ({GetType().Name})");
            isDragging = false;
            root.ReleaseMouse(); // Releases the mouse capture
        }
    }
}