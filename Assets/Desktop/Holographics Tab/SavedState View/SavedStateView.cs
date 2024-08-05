using EVRC.Core;
using EVRC.Core.Overlay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    [RequireComponent(typeof(CreateSavedStateModal))]
    public class SavedStateView : MonoBehaviour
    {
        [SerializeField] UIDocument parentUIDocument;
        private VisualElement root; // the root of the whole UI
        public DropdownField savedStateFileDropdown;

        public SavedGameState savedState;

        [Header("These can be private after initial testing")]
        [SerializeField] string selectedSavedStateFile;
        private string addNewSavedStateString = "-- Create New --";
        CreateSavedStateModal createNewSavedStateModal;

        public void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;
            savedStateFileDropdown = root.Q<DropdownField>("current-savedstate-file");

            createNewSavedStateModal = GetComponent<CreateSavedStateModal>();

            // Default, if not selected already
            string lastUsedFile = UserPreferences.GetLastUsedJsonFile();
            selectedSavedStateFile = lastUsedFile == null ? Paths.OverlayStateFileName : lastUsedFile;

            PopulateSavedStateDropdown();

            // Load the file selected by the user
            savedState.Load(selectedSavedStateFile); // savedState will raise a gameEvent automatically when it's loaded
        }


        private void PopulateSavedStateDropdown()
        {
            List<string> overlayFiles = OverlayFileUtils.GetAllSavedStateFiles();
            savedStateFileDropdown.choices = overlayFiles;

            // Set dropdown to the selected file, if it exists in the list of files
            savedStateFileDropdown.index = overlayFiles.Contains(selectedSavedStateFile) ? overlayFiles.IndexOf(selectedSavedStateFile) : 0;

            savedStateFileDropdown.choices.Add(addNewSavedStateString);

            savedStateFileDropdown.RegisterValueChangedCallback(OnSavedStateDropdownChanged);
        }

        private void OnSavedStateDropdownChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == addNewSavedStateString)
            {
                createNewSavedStateModal.LaunchModal();
            }
            else
            {
                SwitchSavedStateFile(evt.previousValue, evt.newValue);
            }
        }


        private void SwitchSavedStateFile(string oldFileName, string newFileName)
        {
            Debug.Log($"Changing SavedState file from {oldFileName} to {newFileName}");
            savedState.SwitchFile(newFileName); // savedState will raise a GameEvent when done
            selectedSavedStateFile = newFileName;
            UserPreferences.SaveLastUsedJsonFile(newFileName);
        }

        private void CreateNewSavedStateFile(string fileName)
        {

        }
    }
}
