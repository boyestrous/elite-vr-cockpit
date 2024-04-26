﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace EVRC.Core
{
    public static class EDControlBindingsUtils
    {

        public static Dictionary<EDControlButton, ControlButtonBinding> ParseFile(string customBindingsOptionsPath)
        {
            if (!File.Exists(customBindingsOptionsPath)) { throw new FileNotFoundException("bindings path not found"); }
            var bindings = new Dictionary<EDControlButton, ControlButtonBinding>();

            XDocument doc = XDocument.Load(customBindingsOptionsPath);
            foreach (XElement control in doc.Descendants())
            {
                if (!Enum.IsDefined(typeof(EDControlButton), control.Name.LocalName)) continue;

                var controlButton = (EDControlButton)Enum.Parse(typeof(EDControlButton), control.Name.LocalName);

                var controlBinding = new ControlButtonBinding
                {
                    Primary = ParseControlBinding(control, "Primary"),
                    Secondary = ParseControlBinding(control, "Secondary")
                };

                bindings.TryAdd(controlButton, controlBinding);

                // @todo Parse axis and options if we ever have a use for them
            }

            return bindings;
        }

        public static ControlButtonBinding.KeyBinding ParseControlBinding(XElement control, string nodeName)
        {
            var node = (from el in control.Descendants(nodeName) select el).First();
            var keyBinding = new ControlButtonBinding.KeyBinding
            {
                Device = GetAttributeValue(node, "Device"),
                Key = GetAttributeValue(node, "Key"),
                DeviceIndex = GetAttributeValue(node, "DeviceIndex"),
                Modifiers = new HashSet<ControlButtonBinding.KeyModifier>(),
            };

            foreach (var modifier in node.Descendants("Modifier"))
            {
                keyBinding.Modifiers.Add(new ControlButtonBinding.KeyModifier
                {
                    Device = GetAttributeValue(modifier, "Device"),
                    Key = GetAttributeValue(modifier, "Key"),
                });
            }

            return keyBinding;
        }

        private static string GetAttributeValue(XElement el, string localName)
        {
            localName = localName.ToLowerInvariant();
            XAttribute attribute = el.Attributes()
                                    .FirstOrDefault(attr => attr.Name.LocalName.ToLowerInvariant() == localName);

            return attribute?.Value;  // Return the attribute value if found, otherwise return null
        }

        public static void UpdateBindingXml(string filePath, string tagName, string newKey)
        {
            // Save a copy, just in case
            SaveCopyWithTimestamp(filePath);

            // Load XML document from file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);


            // Find elements with the specified tag name
            XmlNodeList nodes = xmlDoc.GetElementsByTagName(tagName);

            // Iterate through the found elements
            foreach (XmlNode node in nodes)
            {
                // Get the "Secondary" element
                XmlNode secondaryNode = node.SelectSingleNode("Secondary");
                if (secondaryNode != null)
                {
                    // Update the "Device" and "Key" properties of the "Secondary" element
                    XmlNode deviceAttr = secondaryNode.Attributes.GetNamedItem("Device");
                    if (deviceAttr != null)
                    {
                        deviceAttr.Value = "Keyboard";
                    }

                    XmlNode keyAttr = secondaryNode.Attributes.GetNamedItem("Key");
                    if (keyAttr != null)
                    {
                        keyAttr.InnerText = newKey;
                    }
                }
            }

            // Save the modified XML back to the file
            xmlDoc.Save(filePath);
        }

        public static void SaveCopyWithTimestamp(string originalFilePath)
        {
            try
            {
                // Check if the original file exists
                if (!File.Exists(originalFilePath))
                {
                    Debug.LogError("Original file does not exist: " + originalFilePath);
                    return;
                }

                // Get the directory and filename from the original file path
                string directory = Path.GetDirectoryName(originalFilePath);
                string filename = Path.GetFileNameWithoutExtension(originalFilePath);
                string extension = Path.GetExtension(originalFilePath);

                // Create the backup folder if it doesn't exist
                string backupFolder = Path.Combine(directory, "backup_binds");
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                CleanBackupBindsFiles(originalFilePath); // cleanup old files before making more

                // Construct the new filename with timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string newFilename = $"{filename}_{timestamp}{extension}";

                // Construct the path for the copy within the backup folder
                string copyFilePath = Path.Combine(backupFolder, newFilename);

                // Copy the original file to the new path
                File.Copy(originalFilePath, copyFilePath);

                Debug.Log("Copy of bindings file saved: " + copyFilePath);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while saving file copy: " + ex.Message);
            }
        }

        public static void CleanBackupBindsFiles(string originalFilePath)
        {

            // Get the directory and filename from the original file path
            string directory = Path.GetDirectoryName(originalFilePath);
            string backupFolderPath = Path.Combine(directory, "backup_binds");

            try
            {
                // Check if the backup folder exists
                if (!Directory.Exists(backupFolderPath))
                {
                    Debug.LogError("Backup folder does not exist: " + backupFolderPath);
                    return;
                }

                // Get all files in the backup folder
                string[] files = Directory.GetFiles(backupFolderPath);

                // Loop through each file
                foreach (string file in files)
                {
                    // Get the datetime from the file name
                    string filename = Path.GetFileNameWithoutExtension(file);
                    string[] parts = filename.Split('_');
                    if (parts.Length < 2)
                    {
                        // Skip files without datetime in the name
                        continue;
                    }

                    if (!DateTime.TryParseExact(parts[1], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime fileDateTime))
                    {
                        Debug.LogWarning("Failed to parse datetime from file name: " + filename);
                        continue;
                    }

                    // Calculate the difference in days between the file datetime and current datetime
                    TimeSpan difference = DateTime.Now - fileDateTime;
                    double daysDifference = difference.TotalDays;

                    // Check if the file is older than 30 days
                    if (daysDifference > 30)
                    {
                        // Delete the file
                        File.Delete(file);
                        Debug.Log($"Backup Binds File deleted, older than 30 days: {Path.GetFileNameWithoutExtension(file)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while cleaning backup files: " + ex.Message);
            }
        }


        public static string EDControlFriendlyName(EDControlButton button)
        {
            string input = button.ToString();
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var result = new StringBuilder();
            var currentWord = new StringBuilder();
            char prevChar = char.MinValue;

            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    if (currentWord.Length > 0 && !char.IsUpper(prevChar))
                    {
                        result.Append(currentWord + " ");
                        currentWord.Clear();
                    }
                }
                else if (char.IsDigit(c))
                {
                    if (currentWord.Length > 0 && !char.IsDigit(prevChar))
                    {
                        result.Append(currentWord + " ");
                        currentWord.Clear();
                    }
                }
                else if (c == '_')
                {
                    if (currentWord.Length > 0)
                    {
                        result.Append(currentWord + " ");
                        currentWord.Clear();
                    }
                    continue;
                }
                else if (currentWord.Length >= 16)
                {
                    break;
                }

                currentWord.Append(c);
                prevChar = c;
            }

            if (currentWord.Length > 0)
            {
                result.Append(currentWord);
            }


            string MaybeShorten(string buttonName)
            {
                if (buttonName.Length < 20) { return buttonName; }

                List<(string, string)> replaceStrings = new List<(string, string)>()
                {
                    ("Exploration FSS", "FSS"),
                    ("Exploration SAA", "SAA"),
                    ("Button", "Btn"),
                    ("Commander", "Cmdr"),
                    ("Alternate", "Alt"),
                    ("Distribution", "Dist"),
                    ("Previous", "Prev"),
                    ("Combination", "Combo"),
                    ("Forward", "Fwd"),
                    ("Backward", "Back"),
                    ("Next", "Nxt"),
                    ("Power", "Pwr"),
                    ("Disable", "Kill"),
                    ("Increase", "Raise"),
                    ("Decrease", "Lower"),
                    ("Buggy", "SRV"),
                    ("Panel", "Pane"),
                    ("Vertical", "Vert"),
                    ("Cycle", ""),
                    ("Multi Crew", "Crew"),
                    ("Utility", "Util"),
                    ("Camera", "Cam"),
                    ("Reverse", "Flip"),
                    ("Aggressive", "Agg"),
                    ("Defensive", "Def"),
                    ("Lower", "Down"),
                    ("Toggle", ""),
                    ("Landing", ""),
                    ("Audio", ""),
                    ("Third Person", "Cam"),
                    ("Target", "Tgt"),
                    ("Increase", "Add"),
                    ("Decrease", "Less"),
                };

                for (int i = 0; i < replaceStrings.Count; i++)
                {
                    if (buttonName.Length < 20) { return buttonName; }
                    buttonName = buttonName.Replace(replaceStrings[i].Item1, replaceStrings[i].Item2);
                }
                return buttonName;

            }

            return MaybeShorten(result.ToString()).Trim();
        }

    }

}
