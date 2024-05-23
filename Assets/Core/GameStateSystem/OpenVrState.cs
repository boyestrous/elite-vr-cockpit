using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/OpenVrState"), Serializable]
    public class OpenVrState : GameState<bool>
    {
        public bool running = false;
        public string hmdSystemName;
        public string hmdSystemModel;
        public string hmdSystemType;

        public override string GetStatusText()
        {
            return running ? "Running" : "Not Running";
        }
    }
}
