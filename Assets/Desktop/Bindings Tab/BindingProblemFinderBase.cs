using EVRC.Core;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Desktop
{

    /// <summary>
    /// Base class for classes that are used to identify problematic bindings in a user's Custom.X.Y.binds files
    /// </summary>
    public abstract class BindingProblemFinderBase : MonoBehaviour
    {
        public List<EDControlButton> problemList;

        internal virtual void OnEnable()
        {
            problemList = new List<EDControlButton>();
        }

        public virtual void Remove(EDControlButton controlButtonToRemove)
        {
            problemList.Remove(controlButtonToRemove);
        }

        public abstract void FindBindingProblems(Dictionary<EDControlButton, ControlButtonBinding> bindings);

    }
}
