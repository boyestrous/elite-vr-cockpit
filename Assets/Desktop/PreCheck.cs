using EVRC.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Desktop
{
    [RequireComponent(typeof(PreCheck))]
    public abstract class PreCheck : MonoBehaviour
    {
        [Description("The uss name of the tab that will display the error style")]
        public string tabName;
        public bool hasErrors;
        public delegate void NotifyPreCheckManager();

        public NotifyPreCheckManager updatePreCheckState;

        public virtual void OnPreCheckStateChanged()
        {
            updatePreCheckState.Invoke();
        }
    }
}
