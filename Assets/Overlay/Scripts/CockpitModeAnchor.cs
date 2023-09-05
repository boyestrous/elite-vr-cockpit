using System;
using EVRC.Core.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI.Extensions;
using static EVRC.Core.Overlay.CockpitModeAnchor;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Identifies and activates a group of gameobjects (children) as related to a specific CockpitMode. You DO NOT need to create anchors unless there are specific  
    /// conditions that would make it difficult to generate the anchor with a script. Most things that are saved in the SavedState file will automatically generate 
    /// the relevant Anchor if it doesn't exist.
    /// </summary>
    /// <remarks>
    /// For example, when placing a controlButton, we need to know the root cockpit
    /// object, so we can place the controlButton as a child
    /// is active.
    /// </remarks>
    public class CockpitModeAnchor : MonoBehaviour
    {
        [System.Serializable]
        public struct AnchorSetting
        {
            [SerializeField, Tooltip("The single status flag that must be present for this Object to be active")]
            public EDStatusFlags shipActivationFlag;
            [SerializeField, Tooltip("Which GUI Focus must be present for this Object to be active.")]
            public EDGuiFocus activationGuiFocus;
            [SerializeField, Tooltip("Which single status flag (for the Flag2 field) must be present for this Object to be active.")]
            public EDStatusFlags2 footActivationFlag;
        }

        //[Tooltip("The single status flag that must be present for this Object to be active")] public EDStatusFlags shipActivationFlag;
        //[Description("The default value: 'Panel Or No Focus' will make this anchor stay active if the guiFocus is any of the 'panels' (internal, comms, etc.)")]
        //[Tooltip("Which GUI Focus must be present for this Object to be active.")] public EDGuiFocus activationGuiFocus = EDGuiFocus.PanelOrNoFocus;
        public List<AnchorSetting> activationSettings;
        [SerializeField] internal EliteDangerousState eliteDangerousState;

        // These objects will be activated/deactivated when the statusFlag or GuiFocus changes
        [SerializeField] private List<GameObject> targets;
        public List<GameObject> TargetList { get { return targets; } }
        

        // This is internal so that the test assembly can call it during unit tests
        internal void OnEnable()
        {
            if (activationSettings == null)
            {
                activationSettings = new List<AnchorSetting>();
            }
            if (targets == null || targets.Count == 0)
            {
                AddImmediateChildrenToList();
            }
        }

        internal void OnDisable()
        {
        }

        public void AddControlButton(ControlButton controlButton)
        {
            targets.Add(controlButton.gameObject);
            controlButton.gameObject.transform.SetParent(transform, false);
            Refresh();
        }

        private void AddImmediateChildrenToList()
        {
            targets = new List<GameObject>();

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                targets.Add(child);
            }
            Refresh();
        }

        public void AddAnchorSetting(EDStatusFlags shipStatusFlag, EDGuiFocus guiFocus, EDStatusFlags2 footStatusFlag)
        {
            if (activationSettings == null || activationSettings.Count == 0) activationSettings = new List<AnchorSetting>();
            activationSettings.Add(
                new AnchorSetting() 
                { 
                    activationGuiFocus = guiFocus,
                    shipActivationFlag = shipStatusFlag,
                    footActivationFlag = footStatusFlag
                }
            );

        }

        public void ResetTargets()
        {
            targets = new List<GameObject>();
            AddImmediateChildrenToList();
        }

        private void Refresh()
        {

            // GuiFocusChanged evaluates both the Status Flag and the GuiFocus. By passing the current guiFocus from the state object, we force a refresh that evalutes the full current game situation
            OnGuiFocusChanged(eliteDangerousState.guiFocus);
        }

        private bool ShouldStatusFlagActivate(AnchorSetting activationSetting, EDStatusFlags statusFlags)
        {
            
            return activationSetting.shipActivationFlag == default(EDStatusFlags) ? false : statusFlags.HasFlag(activationSetting.shipActivationFlag);
        }

        private bool ShouldStatusFlag2Activate(AnchorSetting anchorSetting, EDStatusFlags2 statusFlags)
        {
            return anchorSetting.footActivationFlag == default ? false : statusFlags.HasFlag(anchorSetting.footActivationFlag);
        }

        private bool ShouldGuiFocusActivate(AnchorSetting activationSetting, EDGuiFocus guiFocus)
        {
            if (activationSetting.activationGuiFocus == EDGuiFocus.PanelOrNoFocus && (int)guiFocus <= 4)
            {
                return true;
            }
            return guiFocus == activationSetting.activationGuiFocus;
        }


        public void OnEDStatusFlagsChanged(EDStatusFlags newStatusFlags, EDStatusFlags2 newStatusFlags2)
        {
            // GuiFocus values above 4 are "mode" types, so status flag changes won't affect the UI
            if ((int)eliteDangerousState.guiFocus > 4) return;
           
            ActivateTargets(
                activationSettings.Any(
                    anchorSetting =>
                        // StatusFlags & GuiFocus match
                        ShouldStatusFlagActivate(anchorSetting, newStatusFlags) &&                        
                        ShouldGuiFocusActivate(anchorSetting, eliteDangerousState.guiFocus)
                        // OR
                        ||
                        // StatusFlags2 matches
                        ShouldStatusFlag2Activate(anchorSetting, newStatusFlags2)
                    )
                );
            

        }

        public void OnGuiFocusChanged(EDGuiFocus newFocus) 
        {
            // GuiFocus values above 4 are "mode" types, so status flag changes won't affect the UI
            if ((int)newFocus > 4)
            {
                ActivateTargets(activationSettings.Any(setting => ShouldGuiFocusActivate(setting, newFocus)));
                return;
            }

            //ActivateTargets(ShouldStatusFlagActivate(eliteDangerousState.statusFlags) && ShouldGuiFocusActivate(newFocus));

            ActivateTargets(
                activationSettings.Any(
                    anchorSetting =>
                        // StatusFlags & GuiFocus match
                        ShouldStatusFlagActivate(anchorSetting, eliteDangerousState.statusFlags) &&
                        ShouldGuiFocusActivate(anchorSetting, newFocus)
                        // OR
                        ||
                        // StatusFlags2 matches
                        ShouldStatusFlag2Activate(anchorSetting, eliteDangerousState.statusFlags2)
                    )
                );
            
        }
        

        private void ActivateTargets(bool shouldActivate)
        {
            foreach (var target in targets)
                target.SetActive(shouldActivate);
        }

        #region --------------Event Listener Methods-----------------
        public void OnMenuModeActive(bool menuModeActive)
        {         
            // Deactivate the targets if menuMode is active
            Activate(!menuModeActive);

            // Also turn off the listeners
            GetComponent<EDGuiFocusListener>().enabled = !menuModeActive;
            GetComponent<EDStatusFlagsListener>().enabled = !menuModeActive;
        }

        public void OnGameStart(bool started)
        {
            Activate(started);
        }

        public void OnOpenVRStart(bool started)
        {
            Activate(started);
        }

        private void Activate(bool activate)
        {
            if (activate)
            {
                Refresh();
                return;
            }
            ActivateTargets(false);
            
        }
        #endregion
    }
}
