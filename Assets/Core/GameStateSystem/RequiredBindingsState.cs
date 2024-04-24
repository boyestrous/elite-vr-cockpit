using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Required Bindings State"), Serializable]
    public class RequiredBindingsState : GameState
    {
        public override string GetStatusText()
        {
            throw new System.NotImplementedException();
        }

        public List<EDControlButton> requiredBindings;
    }
}
