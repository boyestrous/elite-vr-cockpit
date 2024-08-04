using EVRC.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC.Desktop
{
    public class RequiredBindings : BindingProblemFinderBase
    {
        [SerializeField] RequiredBindingsState requiredBindingsState;

        internal List<EDControlButton> requiredBindings;

        internal override void OnEnable()
        {
            base.OnEnable();

            requiredBindings = requiredBindingsState.requiredBindings;
        }

        public override void FindBindingProblems(Dictionary<EDControlButton, ControlButtonBinding> bindings)
        {
            List<EDControlButton> problems = new List<EDControlButton>();

            problems = bindings
                .Where(kv => bindings[kv.Key].HasKeyboardKeybinding == false && bindings[kv.Key].HasVJoyKeybinding == false)
                .Select(kv => kv.Key)
                .Where(button => requiredBindings.Contains(button))
                .ToList();

            problemList = problems;
        }
    }
}
