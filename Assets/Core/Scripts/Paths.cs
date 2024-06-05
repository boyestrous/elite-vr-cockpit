using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace EVRC.Core
{
    public static class Paths
    {
        private static string _appDataPath;
        public static string AppDataPath
        {
            get
            {
                if (_appDataPath == null)
                {
                    var LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    _appDataPath = Path.Combine(LocalAppData, "Frontier Developments", "Elite Dangerous");
                }

                return _appDataPath;
            }
        }

        private static string _saveDataPath;

        public static string SaveDataPath
        {
            get
            {
                if (_saveDataPath == null)
                {
                    var savedGamesDir = WindowsUtilities.GetKnownFolderPath(WindowsUtilities.KnownFolderId.SavedGames, WindowsUtilities.KnownFolderFlag.DONT_VERIFY);
                    _saveDataPath = Path.Combine(savedGamesDir, "Frontier Developments", "Elite Dangerous");
                }

                return _saveDataPath;
            }
        }

        public static string GraphicsConfigurationOverridePath
            => Path.Combine(AppDataPath, "Options", "Graphics", "GraphicsConfigurationOverride.xml");
        public static string CustomBindingsFolder
            => Path.Combine(AppDataPath, "Options", "Bindings");

        public static string BindingsBackupFolder => GetOrCreateBindingsBackupFolder();

        private static string GetOrCreateBindingsBackupFolder()
        {
            string tempBackupPath = Path.Combine(CustomBindingsFolder, "backup_binds");

            // Ensure the backup folder exists
            if (!Directory.Exists(tempBackupPath))
            {
                Directory.CreateDirectory(tempBackupPath);
            }
            return tempBackupPath;
        }
        public static string StartPresetPath
            => GetFirstValidStartPresetPath();

        private static string GetFirstValidStartPresetPath()
        {
            string returnStartPresetPath = "";

            if (startPresetPaths.Count == 0)
            {
                // Seed the path list if empty
                startPresetPaths.Add(Path.Combine(CustomBindingsFolder, "StartPreset.4.start"));
                startPresetPaths.Add(Path.Combine(CustomBindingsFolder, "StartPreset.start"));
            }

            // Find the first valid entry and return it.
            foreach (var startPresetPath in startPresetPaths)
            {
                if (File.Exists(startPresetPath))
                {
                    return startPresetPath;
                }
            }

            Debug.LogError("Could not find valid path for StartPreset");
            return returnStartPresetPath;
        }

        public static string SteamVRConfigPath = @"C:\Program Files (x86)\Steam\config\steamvr.vrsettings";

        public static string BindingNameFromStartPreset
            => GetBindingNameFromStartPresetFile();

        public static string GetBindingNameFromStartPresetFile()
        {
            string startPreset = File.Exists(StartPresetPath) ? File.ReadAllText(StartPresetPath) : "";

            if ((startPreset ?? "").Trim() == "")
                startPreset = "Custom";
            else
            {
                string[] splitPreset = startPreset.Split(new string[] { "\n" }, StringSplitOptions.None);
                startPreset = splitPreset[0];
            }

            return startPreset;
        }

        public static string DesktopUIDocumentsPath
            => Path.Combine(Application.dataPath, "Desktop");

        public static string StatusFilePath
            => Path.Combine(SaveDataPath, "Status.json");

        public static string OverlayStatePath
            => CockpitStatePath();

        public static string OverlayStateTemplatePath = Path.Combine(Application.dataPath, "Documentation", "SavedState_Template.json");

        public static int currentOverlayFileVersion = 5;

        private static string OverlayStateFileNameWithoutExtension = "SavedState";
        public static string OverlayStateFileName => CockpitStateFileName();

        private static string CockpitStateFileName()
        {
            string filename = OverlayStateFileNameWithoutExtension + ".json";
            #if UNITY_EDITOR
                filename = OverlayStateFileNameWithoutExtension + "_Editor.json";
            #endif

            return filename;
        }

        public static string CockpitStatePath()
        {
            string overlayStateFilePath = Path.Combine(Application.persistentDataPath, OverlayStateFileName);


            return overlayStateFilePath;
        }

        public static string BindingsTemplatePath = Path.Combine(Application.dataPath, "Documentation", "EVRC.4.1.binds");

        public static string ControlBindingsPath
            => GetFirstValidControlBindingsPath();

        public static string GetFirstValidControlBindingsPath()
        {
            string returnBindingsPath = "";

            if (controlBindingsPaths.Count == 0)
            {
                // Seed the path list if empty
                controlBindingsPaths.Add(Path.Combine(CustomBindingsFolder, BindingNameFromStartPreset + ".4.1.binds"));
                controlBindingsPaths.Add(Path.Combine(CustomBindingsFolder, BindingNameFromStartPreset + ".4.0.binds"));
                controlBindingsPaths.Add(Path.Combine(CustomBindingsFolder, BindingNameFromStartPreset + ".3.0.binds"));
                controlBindingsPaths.Add(Path.Combine(CustomBindingsFolder, BindingNameFromStartPreset + ".2.0.binds"));
                controlBindingsPaths.Add(Path.Combine(CustomBindingsFolder, BindingNameFromStartPreset + ".1.8.binds"));
            }

            // Find the first valid entry and return it.
            foreach (var bindingsPath in controlBindingsPaths)
            {
                if (File.Exists(bindingsPath))
                {
                    return bindingsPath;
                }
            }

            Debug.LogError("Could not find valid path for controlBindings;");
            return returnBindingsPath;
        }
        
        public static HashSet<string> controlBindingsPaths = new HashSet<string>();
        public static HashSet<string> startPresetPaths = new HashSet<string>();

    }  
}
