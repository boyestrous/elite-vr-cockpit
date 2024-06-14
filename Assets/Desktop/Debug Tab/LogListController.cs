using EVRC.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    /*
     * class for the list of log entries in the main view
     */
    public class LogListController: MonoBehaviour
    {
        [Serializable]
        public struct LogTypeStyle
        {
            public Color color;
            public Sprite icon;
        }

        [Header("Templates and Scene Refs")]
        // UXML template for list entries
        [SerializeField] internal VisualTreeAsset m_ListEntryTemplate;
        [SerializeField] UIDocument parentUIDocument;

        [Header("Configuration")]
        public LogTypeStyle infoStyle;
        public LogTypeStyle warningStyle;
        public LogTypeStyle errorStyle;
        public int maxLines = 100;

        // UI element references
        internal ListView m_LogList;
        internal Button exportButtonElement;

        // Logs object
        internal List<LogItem> m_AllLogs;

        public void OnEnable()
        {
            m_AllLogs = new List<LogItem>();

            VisualElement root = parentUIDocument.rootVisualElement;

            // Store a reference to the log list element
            m_LogList = root.Q<ListView>("log-list");
            exportButtonElement = root.Q<Button>("export-button");

            exportButtonElement.clicked += ExportAllLogs;

            SetListBindingMethods();

            // listen for new logs
            Application.logMessageReceived += OnLogMessage;
        }        

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessage;
        }

        internal void OnLogMessage(string text, string stackTrace, LogType type)
        {
            var style = type == LogType.Log ? infoStyle
                : type == LogType.Warning ? warningStyle
                : errorStyle;

            LogItem logItem = new LogItem()
            {
                message = text,
                color = style.color,
                icon = style.icon,
                logType = type
            };

            m_AllLogs.Add(logItem);

            // Limit the number of items displayed in the ListView to maxLines
            int startIndex = Math.Max(0, m_AllLogs.Count - maxLines);
            List<LogItem> logsToDisplay = m_AllLogs.GetRange(startIndex, Math.Min(maxLines, m_AllLogs.Count));

            m_LogList.itemsSource = logsToDisplay;
            m_LogList.RefreshItems();

        }

        internal void SetListBindingMethods()
        {
            //Set up a make item function for a list entry
            m_LogList.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = m_ListEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                var newListEntryLogic = new LogEntryDisplay();

                // Assign the controller script to the visual element
                newListEntry.userData = newListEntryLogic;

                // Initialize the controller script
                newListEntryLogic.SetVisualElement(newListEntry);

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            // Set up bind function for a specific list entry
            m_LogList.bindItem = (item, index) =>
            {
                (item.userData as LogEntryDisplay).SetLogData(m_AllLogs[index]);
            };

            // Set a fixed item height
            //m_LogList.fixedItemHeight = 45;

            // Set the actual item's source list/array
            m_LogList.itemsSource = m_AllLogs;
        }

        /// <summary>
        /// Export all logs to the default path
        /// </summary>
        private void ExportAllLogs()
        {
            string filePath = Paths.ExportedLogFileName;
            ExportAllLogs(filePath);
        }


        /// <summary>
        /// Overload allowing you to specify the export destination (mostly for testing)
        /// </summary>
        /// <param name="filePath"></param>
        internal void ExportAllLogs(string filePath)
        {
            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("--------------------------------------------------------------");
                // Date in format readable without confusion about european vs american month/date
                // 12 Hour Time with timezone 
                writer.WriteLine($"Log Exported on: {DateTime.Today.ToString("MMMM dd, yyyy")} at {DateTime.Now.ToString("hh:mm:ss tt (K)")}");
                writer.WriteLine("--------------------------------------------------------------");

                foreach (var log in m_AllLogs)
                {
                    writer.WriteLine($"{log.logType}: {log.message}");
                }
            }
            Debug.Log($"All logs have been exported to {filePath}");
        }

    }
}
