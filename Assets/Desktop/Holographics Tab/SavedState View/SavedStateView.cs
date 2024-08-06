using EVRC.Core;
using EVRC.Core.Overlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    [RequireComponent(typeof(CreateSavedStateModal))]
    public class SavedStateView : MonoBehaviour
    {
        [SerializeField] UIDocument parentUIDocument;
        private VisualElement root; // the root of the whole UI
        private DropdownField savedStateFileDropdown;
        private Button openExplorerButton;

        public SavedGameState savedState;

        [Header("These can be private after initial testing")]
        [SerializeField] string selectedSavedStateFile;
        private string addNewSavedStateString = "-- Create New --";
        CreateSavedStateModal createNewSavedStateModal;

        public void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;
            savedStateFileDropdown = root.Q<DropdownField>("current-savedstate-file");
            openExplorerButton = root.Q<Button>("open-explorer");

            createNewSavedStateModal = GetComponent<CreateSavedStateModal>();

            // Default, if not selected already
            string lastUsedFile = UserPreferences.GetLastUsedJsonFile();
            selectedSavedStateFile = lastUsedFile == null ? Paths.OverlayStateFileName : lastUsedFile;

            if (savedState.GetStatusText() != "Loaded")
            {
                savedState.Load(selectedSavedStateFile);
            }

            PopulateSavedStateDropdown();

            savedStateFileDropdown.RegisterValueChangedCallback(OnSavedStateDropdownChanged);
            openExplorerButton.clicked += OpenSavedStatePath;
            // Load the file selected by the user
            //savedState.Load(selectedSavedStateFile); // savedState will raise a gameEvent automatically when it's loaded
        }

        private void OpenSavedStatePath()
        {
            Utils.OpenExplorer(Application.persistentDataPath);
        }

        public void OnDisable()
        {
            savedStateFileDropdown.UnregisterValueChangedCallback(OnSavedStateDropdownChanged);
        }

        public void PopulateSavedStateDropdown()
        {
            Debug.LogWarning("Populated SavedState Dropdown");
            List<string> overlayFiles = OverlayFileUtils.GetAllSavedStateFiles();
            savedStateFileDropdown.choices = overlayFiles;

            // Set dropdown to the selected file, if it exists in the list of files
            savedStateFileDropdown.index = overlayFiles.Contains(selectedSavedStateFile) ? overlayFiles.IndexOf(selectedSavedStateFile) : 0;

            savedStateFileDropdown.choices.Add(addNewSavedStateString);
        }

        private void OnSavedStateDropdownChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == addNewSavedStateString)
            {
                createNewSavedStateModal.LaunchModal(FindNewFileAndSwitch, () => { });// Do nothing on cancel
            }
            else
            {
                SwitchSavedStateFile(evt.previousValue, evt.newValue);
                savedStateFileDropdown.choices = new List<string>();
                PopulateSavedStateDropdown();
            }
        }

        private void FindNewFileAndSwitch()
        {
            List<string> overlayFiles = OverlayFileUtils.GetAllSavedStateFiles();
            List<string> newFile = overlayFiles.Except(savedStateFileDropdown.choices).ToList();

            if (newFile.Count == 1)
            {
                SwitchSavedStateFile(savedState.currentSavedStateFile, newFile[0]);
            }
        }


        private void SwitchSavedStateFile(string oldFileName, string newFileName)
        {
            Debug.Log($"Changing SavedState file from {oldFileName} to {newFileName}");
            savedState.SwitchFile(newFileName); // savedState will raise a GameEvent when done
            selectedSavedStateFile = newFileName;
            UserPreferences.SaveLastUsedJsonFile(newFileName);
        }
    }
}
