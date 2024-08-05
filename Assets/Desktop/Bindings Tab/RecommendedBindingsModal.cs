using EVRC.Core;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{

    public class RecommendedBindingsModal : MonoBehaviour
    {
        [SerializeField] UIDocument parentUIDocument;
        [SerializeField] VisualTreeAsset m_FixBindingModalTemplate;
        VisualElement modalUI;

        VisualElement recommendedBindingsArea;
        Button recommendedBindingsButton;
        ListView bindingsListView;
        public ControlBindingsState bindings;
       
        void OnEnable()
        {
            VisualElement root = parentUIDocument.rootVisualElement;

            recommendedBindingsArea = root.Q<VisualElement>("recommended-bindings");
            bindingsListView = root.Q<ListView>("required-bindings-list");

            recommendedBindingsButton = root.Q<Button>("replace-bindings-button");
            recommendedBindingsButton.clicked += LaunchModal;
        }

        public void ShouldShowButton(string bindingsName)
        {
            if (bindings == null) return;
            if (string.IsNullOrEmpty(bindingsName)) return;
            if (Paths.StartPresetPath == null) return;
            if (recommendedBindingsArea == null) return;

            // Only show this button if the bindingsName is not EVRC
            recommendedBindingsArea.style.display = bindings.startPreset != "EVRC" ? DisplayStyle.Flex : DisplayStyle.None;
        }
        void LaunchModal()
        {
            modalUI = m_FixBindingModalTemplate.Instantiate();
            //availableSingleKeyBindings
            Button okButton = modalUI.Q<Button>("ok-button");
            Button cancelButton = modalUI.Q<Button>("cancel-button");
            
            okButton.clicked += Submit;
            cancelButton.clicked += CloseModal;
            
            // Get the VisualElement representing the ListView
            VisualElement listViewElement = bindingsListView.hierarchy.parent;

            // Now you can use the RectTransform listViewRectTransform as needed
            // For example, you can get its position, size, etc.
            Vector2 listViewPosition = listViewElement.contentRect.position;
            Vector2 listViewSize = listViewElement.contentRect.size;

            // Calculate the center position of the ListView
            Vector2 listViewCenter = new Vector2(listViewPosition.x + (listViewSize.x / 2f),
                                                 listViewPosition.y + (listViewSize.y / 2f));

            // Set the position of modalUI
            modalUI.style.position = Position.Absolute;
            modalUI.style.left = listViewCenter.x;
            modalUI.style.top = listViewCenter.y;

            parentUIDocument.rootVisualElement.Add(modalUI);
        }

        private void Submit()
        {
            string sourceFileName = "EVRC.4.1.binds";

            string destinationFile = Path.Combine(Paths.CustomBindingsFolder, sourceFileName);
            // Check if the destination file exists
            if (File.Exists(destinationFile))
            {
                EDControlBindingsUtils.SaveCopyWithTimestamp(destinationFile);
            }

            File.Copy(Paths.BindingsTemplatePath, destinationFile, overwrite: true);
            Debug.Log($"Copied {sourceFileName} to Bindings folder: {Paths.CustomBindingsFolder}");


            EDControlBindingsUtils.UpdateStartPreset("EVRC");
            Debug.Log($"Updated Start Preset: {Path.GetFileName(bindings.startPresetFileName)} to use EVRC.X.binds bindings");

            bindings.gameEvent.Raise();
            CloseModal();
        }

        private void CloseModal()
        {
            modalUI.RemoveFromHierarchy();
        }
    }
}
