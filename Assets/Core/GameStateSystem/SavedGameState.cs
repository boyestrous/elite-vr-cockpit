using EVRC.Core.Overlay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC.Core
{
    /// <summary>
    /// The current state that is used in the EVRC system
    /// </summary>
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Saved Game State"), Serializable]
    public class SavedGameState : GameState
    {
        public int fileVersion;
        public string currentSavedStateFile;
        public List<SavedGameObject> staticLocations;
        public List<SavedControlButton> controlButtons;
        public List<SavedBooleanSetting> booleanSettings; 
        public bool loaded = false;

        public void SwitchFile(string newFileName)
        {
            Reset();
            Load(newFileName);
        }

        public void Reset()
        {
            fileVersion = 0;
            staticLocations = new List<SavedGameObject>();
            controlButtons = new List<SavedControlButton>();
            booleanSettings = new List<SavedBooleanSetting>();
            loaded = false;
        }

        public override string GetStatusText()
        {
            return loaded ? "Loaded" : "Not Loaded";
        }

        public void Load()
        {
            // Default, if not selected already
            string lastUsedFile = UserPreferences.GetLastUsedJsonFile();
            currentSavedStateFile = lastUsedFile == null ? Paths.OverlayStateFileName : lastUsedFile;

            Load(currentSavedStateFile);
        }

        public void Load(string fileName)
        {
            currentSavedStateFile = fileName;

            SavedStateFile file = OverlayFileUtils.LoadFromFile(fileName);
            fileVersion = file.version;
            staticLocations = file.staticLocations.ToList();
            controlButtons = file.controlButtons.ToList();
            booleanSettings = file.booleanSettings.ToList();
            loaded = true;
            gameEvent.Raise();
        }

        public void Save()
        {
            SavedStateFile file = new SavedStateFile();
            file.version = fileVersion;
            file.staticLocations = staticLocations.ToArray();
            file.controlButtons = controlButtons.ToArray();
            file.booleanSettings = booleanSettings.ToArray();

            OverlayFileUtils.WriteToFile(file, currentSavedStateFile);
        }

        

    }
}
