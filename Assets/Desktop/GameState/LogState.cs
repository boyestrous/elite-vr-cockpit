using EVRC.Desktop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Log State"), Serializable]
    public class LogState : GameState
    {
        [Serializable]
        public struct LogTypeStyle
        {
            public Color color;
            public Sprite icon;

        }

        [Header("Configuration")]
        public LogTypeStyle infoStyle;
        public LogTypeStyle warningStyle;
        public LogTypeStyle errorStyle;

        public int maxLines = 100;

        public List<LogItem> allLogs;

        public void RemoveAll()
        {
            allLogs = new List<LogItem>();
        }

        public List<LogItem> Add(LogItem itemToLog)
        {
            allLogs.Add(itemToLog);

            // Limit the number of items displayed in the ListView to maxLines
            int startIndex = Math.Max(0, allLogs.Count - maxLines);
            List<LogItem> logsToDisplay = allLogs.GetRange(startIndex, Math.Min(maxLines, allLogs.Count));
            return logsToDisplay;
        }

        /// <summary>
        /// Export all logs to the default path
        /// </summary>
        public void ExportAllLogs()
        {
            string filePath = Paths.ExportedLogFileName;
            ExportAllLogs(filePath);
        }


        /// <summary>
        /// Overload allowing you to specify the export destination (mostly for testing)
        /// </summary>
        /// <param name="filePath"></param>
        public void ExportAllLogs(string filePath)
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

                foreach (var log in allLogs)
                {
                    writer.WriteLine($"{log.logType}: {log.message}");
                }
            }
            Debug.Log($"All logs have been exported to {filePath}");
        }

        public string GetAllLogs(bool numberedLines = true)
        {
            StringBuilder logBuilder = new StringBuilder();

            logBuilder.AppendLine("--------------------------------------------------------------");
            // Date in format readable without confusion about European vs. American month/date
            // 12 Hour Time with timezone 
            logBuilder.AppendLine($"Log Exported on: {DateTime.Today.ToString("MMMM dd, yyyy")} at {DateTime.Now.ToString("hh:mm:ss tt (K)")}");
            logBuilder.AppendLine("--------------------------------------------------------------");

            int lineNumber = 1;
            foreach (var log in allLogs)
            {
                string message = log.message;

                // Try to anonymize the user's name by masking appdata
                string appDataString = "AppData";
                int appDataIndex = message.IndexOf(appDataString, StringComparison.OrdinalIgnoreCase);
                if (appDataIndex >= 0)
                {
                    message = "%AppData%" + message.Substring(appDataIndex + appDataString.Length);
                }

                // Unity uses Log/Warning/Error as logtypes. Info is a better description, so swap this text in the output.                
                string logTypeString = "";
                if (log.logType == LogType.Log)
                {
                    logTypeString = "Info";
                } else
                {
                    logTypeString = log.logType.ToString();
                }

                // Add line numbers
                logBuilder.AppendLine($"{lineNumber}. **{logTypeString}**: {log.message}");
                lineNumber++;
            }

            string logOutput = logBuilder.ToString();
            Debug.Log("All logs have been exported.");

            return logOutput;
        }

        public override string GetStatusText()
        {
            return "ready";
        }
    }
}
