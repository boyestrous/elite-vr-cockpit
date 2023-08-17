using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Elite Dangerous State"), System.Serializable]
    public class EliteDangerousState : GameState
    {
        public bool running = false;
        public uint processId;
        public string processName;
        public string processDirectory;
        public EDStatusFlags statusFlags;
        public EDStatusFlags2 statusFlags2;
        public EDGuiFocus guiFocus;
        public EDStatus lastStatusFromFile;

        [Header("Convenience References")]
        public EDStatusFlagsEvent statusFlagsEvent;
        public EDGuiFocusEvent guiFocusEvent;

        public void Clear()
        {
            running = false;
            processId = 0;
            processName = null;
            processDirectory = null;    
            statusFlags= 0;
            statusFlags2 = 0;
            guiFocus = EDGuiFocus.NoFocus;

            // For items directly read from Status.json
            lastStatusFromFile.timestamp = null;
            lastStatusFromFile.Flags = 0;
            lastStatusFromFile.Flags2 = 0;
            lastStatusFromFile.Pips = new byte[] { };
            lastStatusFromFile.FireGroup = 0;
            lastStatusFromFile.GuiFocus = 0;
        }

        public override string GetStatusText()
        {
            return running ? "Running" : "Not Running";
        }

        void OnDisable()
        {
            Clear();
        }
    }

   
}