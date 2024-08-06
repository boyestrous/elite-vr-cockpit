using EVRC.Core;
using EVRC.Core.Overlay;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class CreateSavedStateModal : Modal
    {
        private string userFileName;
        private Label modalMessageElement;
        private TextField textInputElement;

        public override void LaunchModal()
        {
            base.LaunchModal();
            GetAdditionalElements();
        }

        public override void LaunchModal(Action onClose)
        {
            base.LaunchModal(onClose);
            GetAdditionalElements();
        }

        public override void LaunchModal(Action onSubmit, Action onCancel)
        {
            base.LaunchModal(onSubmit, onCancel);
            GetAdditionalElements();
        }

        private void GetAdditionalElements()
        {
            modalMessageElement = modalUI.Q<Label>("modal-message");
            textInputElement = modalUI.Q<TextField>("filename-input");

            textInputElement.RegisterValueChangedCallback(OnInputChanged);
        }

        private void OnInputChanged(ChangeEvent<string> evt)
        {
            userFileName = evt.newValue;
        }

        private void DisplayMessage(string messageText)
        {
            modalMessageElement.style.display = DisplayStyle.Flex;
            modalMessageElement.text = messageText;
        }

        private void HideMessage()
        {
            modalMessageElement.text = "";
            modalMessageElement.style.display = DisplayStyle.None;
        }

        public override void Submit()
        {
            HideMessage();

            userFileName = EnsureJsonExtension(userFileName);
            Debug.Log($"...after EnsureJsonExtension: {userFileName}");
            if (ValidInput(userFileName)) 
            {
                OverlayFileUtils.CreateBlankSavedStateFile(userFileName);
                Close();
            }
        }

        private bool ValidInput(string userInput)
        {
            bool valid = false;

            valid = NameNotReserved(userInput) 
                    && NameNotUsed(userInput);
            return valid;
        }

        

        private bool NameNotReserved(string filename)
        {
            //Debug.Log($"NameNotReserved starting with: {filename}");
            List<string> reserved = new List<string>() { "SavedState.json" };

            if (reserved.Contains(filename))
            {
                DisplayMessage($"{filename} Reserved - choose another name");
                Debug.LogError($"Filename Reserved: {filename}");
                return false;
            }
            return true;
        }

        private bool NameNotUsed(string filename)
        {
            //Debug.Log($"NameNotUsed starting with: {filename}");
            List<string> usedFilenames = OverlayFileUtils.GetAllSavedStateFiles();

            if (usedFilenames.Contains(filename))
            {
                DisplayMessage($"Filename already in use: {filename}");
                Debug.LogError($"Filename is already in use: {filename}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add .json to the end of the text, if the user didn't type it themselves
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string EnsureJsonExtension(string filename)
        {
            Debug.Log($"EnsureJsonExtension original: {filename}");
            // Check if the filename ends with ".json"
            if (filename.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return filename;
            }          

            // If no extension, append ".json"
            return filename + ".json";
        }

        
    }
}
