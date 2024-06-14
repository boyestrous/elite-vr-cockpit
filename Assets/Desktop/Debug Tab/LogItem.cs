using System;
using UnityEngine;

namespace EVRC.Desktop
{
    [Serializable]
    public class LogItem
    {
        public string message;
        public Sprite icon;
        public Color color;
        public LogType logType;
    }
}
