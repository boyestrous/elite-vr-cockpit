using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using EVRC.Core.Actions;
using Moq;
using UnityEngine;
using UnityEngine.UIElements;
using Valve.Newtonsoft.Json;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Read and Write from the various SavedState Files used by the overlay. Not used to interface with
    /// any of the Elite Dangerous files 
    /// </summary>
    public static class OverlayFileUtils
    {
        #region ---------------Load---------------------
        public static List<string> GetAllSavedStateFiles()
        {
            // Default path
            string path = Application.persistentDataPath;

            return GetAllSavedStateFiles(path);
        }

        public static List<string> GetAllSavedStateFiles(string path)
        {
            List<string> jsonFiles = new List<string>();
            try
            {
                // Get all .json files in the specified directory
                string[] files = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);

                // Check each file for the "version" key
                foreach (string file in files)
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(file);
                        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);

                        if (data.TryGetValue("version", out object value))
                        {
                            int version = ((IConvertible)value).ToInt32(null);
                            jsonFiles.Add(Path.GetFileName(file));
                        }
                        // TODO - maybe add a way to indicate old file versions
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error reading or parsing JSON file {file} while trying to populate the Desktop List of SavedState Files. Messag: {ex.Message}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error accessing files at {path}: {ex.Message}");
            }

            return jsonFiles;
        }

        /// <summary>
        /// Loads the Default SavedState file from the Default Location
        /// </summary>
        /// <returns></returns>
        public static SavedStateFile LoadFromFile()
        {
            var defaultFilename = Paths.OverlayStateFileName;
            return LoadFromFile(defaultFilename);
        }

        /// <summary>
        /// Load a specific SavedState filename from default file location
        /// </summary>
        /// <param name="fileName">File name - assumed to be in the default location</param>
        /// <returns></returns>
        public static SavedStateFile LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Application.persistentDataPath);
        }

        /// <summary>
        /// Load a specific filename from a specific location.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pathFolder"></param>
        /// <returns></returns>

        public static SavedStateFile LoadFromFile(string fileName, string pathFolder)
        {
            string filePath = Path.Combine(pathFolder, fileName);

            // If the filename is the default file AND it doesn't exist, use the template
            if (fileName == Paths.OverlayStateFileName && !File.Exists(filePath))
            {
                Debug.LogWarning("Default File not found, will use a copy of the template file instead");
                CopyTemplateFile(Path.Combine(pathFolder, fileName));
            }

            if (filePath != null && File.Exists(filePath))
            {
                return Load(filePath);
            }

            Debug.LogWarning($"Could not find the provided path: {filePath}.");
            Debug.Log($"Trying to load default SavedState File: {Paths.OverlayStatePath}");

            if (pathFolder == Application.persistentDataPath)
            {
                return LoadFromFile(); // load from default name and location
            } else
            {
                return LoadFromFile(Paths.OverlayStateFileName, pathFolder); 
            }
        }

        /// <summary>
        /// Copy the template file (from default location) to the default location
        /// </summary>
        private static void CopyTemplateFile()
        {
            string dest = Paths.OverlayStatePath;
            CopyTemplateFile(dest);
        }


        /// <summary>
        /// Copy the template file from a specific location
        /// </summary>
        /// <param name="destination">full path to the desired destination (including filename)</param>
        private static void CopyTemplateFile(string destination)
        {
            File.Copy(Paths.OverlayStateTemplatePath, destination);
            Debug.Log($"Copied Template SavedState to: {destination}");
        }

        internal static SavedStateFile Load(string path)
        {
            Debug.LogFormat("Loading from {0}", path);

            var returnState = new SavedStateFile();
            var fileVersion = TryGetSavedStateVersion(path);
            
            
            // If it's not the current file version, start the upgrade process, which will
            // return an updated 
            if (fileVersion < Paths.currentOverlayFileVersion)
            {
                Debug.LogWarning($"File version: {fileVersion} is not current. Starting upgrade...");
                OverlayStateUpgradeManager upgradeManager = new OverlayStateUpgradeManager();
                returnState = upgradeManager.UpgradeOverlayStateFile(path, fileVersion);
                return returnState;
            }
        
            try
            {
                // If it's already the right version, Deserialize and return
                returnState = JsonConvert.DeserializeObject<SavedStateFile>(File.ReadAllText(path));
                return returnState;
            }
            catch (Exception ex)
            {
                // Log the error and raise an exception
                Debug.LogError($"Failed to load JSON file at {path}: {ex.Message}");
                throw new Exception($"Failed to load JSON file at {path}", ex);
            }
            
        }

        public static int TryGetSavedStateVersion(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
            if (data.TryGetValue("version", out object value))
            {
                int version = ((IConvertible)value).ToInt32(null);
                return version;
            }
            else
            {
                Debug.LogError($"Could not find version in SavedState File: {filePath}. Starting a fresh SavedState file.");
                return Paths.currentOverlayFileVersion;
            }

        }

        #endregion


        #region ---------------Save---------------------
        public static void WriteToFile(SavedStateFile state)
        {
            string savedStatePath = Paths.OverlayStatePath;
            WriteToFile(state, savedStatePath);
        }

        public static void WriteToFile(SavedStateFile state, string saveFileName)
        {
            WriteToFile(state, Application.persistentDataPath, saveFileName);
        }

        public static void WriteToFile(SavedStateFile state, string destinationPath, string saveFileName)
        {
            string filePath = Path.Combine(destinationPath, saveFileName);

            try
            {
                File.WriteAllText(filePath, JsonUtility.ToJson(state));
                Debug.LogFormat($"SavedState Saved to {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Could not save file: {saveFileName} to path:{destinationPath}. Exception: {ex.Message}");
            }
        }
        #endregion

    }
}