using EVRC.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC.Desktop
{
    /// <summary>
    /// Used to track pairs of lists where ControlButtons cannot have overlapping bindings configured.
    /// </summary>
    /// <remarks>
    /// For example, UI_Up from listOne and CamTranslateForward cannot BOTH have Arrow-Up configured as the binding
    /// </remarks>
    public struct ExclusiveBindingSet
    {
        public List<EDControlButton> listOne;
        public List<EDControlButton> listTwo;
    }

    public class ExclusiveBindings : BindingProblemFinderBase
    {
        [SerializeField] internal List<ExclusiveBindingSet> exclusiveBindingSets = new List<ExclusiveBindingSet>();

        internal override void OnEnable()
        {
            base.OnEnable();

            exclusiveBindingSets = new List<ExclusiveBindingSet>() 
            { 
                // Map controls may not overlap with UI Controls
                new ExclusiveBindingSet()
                {
                    // Map movement controls 
                    listOne = new List<EDControlButton>() 
                    { 
                        EDControlButton.CamPitchDown, 
                        EDControlButton.CamPitchUp,
                        EDControlButton.CamYawLeft, 
                        EDControlButton.CamYawRight,
                        EDControlButton.CamZoomIn, 
                        EDControlButton.CamZoomOut,
                        EDControlButton.CamTranslateBackward, 
                        EDControlButton.CamTranslateForward,
                        EDControlButton.CamTranslateDown,
                        EDControlButton.CamTranslateUp,
                        EDControlButton.CamTranslateLeft, 
                        EDControlButton.CamTranslateRight,
                        EDControlButton.CamTranslateZHold
                    },
                    // UI Controls
                    listTwo = new List<EDControlButton>()
                    {
                        EDControlButton.UI_Back,
                        EDControlButton.UI_Up, 
                        EDControlButton.UI_Down,
                        EDControlButton.UI_Left,
                        EDControlButton.UI_Right,
                        EDControlButton.UI_Toggle,
                        EDControlButton.UI_Select
                    }
                }
            };
        }

        public override void FindBindingProblems(Dictionary<EDControlButton, ControlButtonBinding> bindings)
        {
            List<(EDControlButton, EDControlButton, ControlButtonBinding.KeyBinding)> problems = new List<(EDControlButton, EDControlButton, ControlButtonBinding.KeyBinding)>();

            // for each set in ExclusiveBindingSet
            foreach(ExclusiveBindingSet exclusiveBindingSet in exclusiveBindingSets)
            {
                // look at each item in listOne
                foreach(EDControlButton listOneControlButton in exclusiveBindingSet.listOne)
                {
                    // find the EDControlButton and both of its corresponding ControlButtonBinding.KeyBindings
                    if (!bindings.TryGetValue(listOneControlButton, out var listOneButtonBinding)) continue;
                    var tempPrimary = listOneButtonBinding.Primary;
                    var tempSecondary = listOneButtonBinding.Secondary;

                    // Check each of the EDControlButtons in listTwo
                    foreach(EDControlButton listTwoControlButton in exclusiveBindingSet.listTwo)
                    {
                        // find the EDControlButton and both of its corresponding ControlButtonBinding.KeyBindings
                        if (!bindings.TryGetValue(listTwoControlButton, out var listTwoButtonBinding)) continue;

                        if (tempPrimary.IsValidKeypress && (listTwoButtonBinding.Primary.Equals(tempPrimary) || listTwoButtonBinding.Secondary.Equals(tempPrimary)))
                        {
                            problems.Add((listOneControlButton, listTwoControlButton, tempPrimary));
                        }

                        if (tempSecondary.IsValidKeypress && (listTwoButtonBinding.Secondary.Equals(tempSecondary) || listTwoButtonBinding.Primary.Equals(tempSecondary)))
                        {
                            problems.Add((listOneControlButton, listTwoControlButton, tempSecondary));
                        }
                    }
                }
            }

            problemList = problems
                .SelectMany(tuple => new[] { tuple.Item1, tuple.Item2 })
                .Distinct()
                .ToList();
        }

    }
}
