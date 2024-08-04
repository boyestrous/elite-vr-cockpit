using EVRC.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    /*
     * class for the list of log entries in the main view
     */
    public class LogListController: MonoBehaviour
    {

        [Header("Templates and Scene Refs")]
        // UXML template for list entries
        [SerializeField] internal VisualTreeAsset m_ListEntryTemplate;
        [SerializeField] UIDocument parentUIDocument;
        [SerializeField] internal LogState logState;

        // UI element references
        internal ListView m_LogList;
        internal Button exportButtonElement;

        public void OnEnable()
        {
            VisualElement root = parentUIDocument.rootVisualElement;

            // Store a reference to the log list element
            m_LogList = root.Q<ListView>("log-list");
            exportButtonElement = root.Q<Button>("export-button");

            // Reset the current logs to empty
            logState.RemoveAll();

            exportButtonElement.clicked += logState.ExportAllLogs;

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
            var style = type == LogType.Log ? logState.infoStyle
                : type == LogType.Warning ? logState.warningStyle
                : logState.errorStyle;

            LogItem logItem = new LogItem()
            {
                message = text,
                color = style.color,
                icon = style.icon,
                logType = type
            };

            List<LogItem> logsToDisplay = logState.Add(logItem);

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
                (item.userData as LogEntryDisplay).SetLogData(logState.allLogs[index]);
            };

            // Set a fixed item height
            //m_LogList.fixedItemHeight = 45;

            // Set the actual item's source list/array
            m_LogList.itemsSource = logState.allLogs;
        }
    }
}
