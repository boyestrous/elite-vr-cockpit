using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EVRC.Core.Actions.VirtualJoystick;

namespace EVRC.Core
{
    public class VirtualOnFootController : MonoBehaviour, IGrabable
    {
        public OverlayEditLockState editLockedState;
        private ControllerInteractionPoint attachedInteractionPoint;
        [SerializeField] private OnFootGrippedController buttons;

        public void OnEnable()
        {
            buttons = GetComponent<OnFootGrippedController>();
            if(buttons == null)
            {
                Debug.LogError($"Could not find virtual buttons component on this GameObject: {name}");
            }
        }

        public GrabMode GetGrabMode()
        {
            return GrabMode.VirtualControl;
        }

        public bool Grabbed(ControllerInteractionPoint interactionPoint)
        {
            if (attachedInteractionPoint != null) return false;
            // Don't allow joystick use when editing is unlocked, so the movable surface can be used instead
            if (!editLockedState.EditLocked) return false;

            attachedInteractionPoint = interactionPoint;

            if (buttons)
            {
                buttons.Grabbed(interactionPoint.Hand);
            }

            return true;
        }

        public void Ungrabbed(ControllerInteractionPoint interactionPoint)
        {
            if (interactionPoint == attachedInteractionPoint)
            {
                attachedInteractionPoint = null;

                if (buttons)
                {
                    buttons.Ungrabbed();
                    buttons.Reset();
                }

                vJoyInterface.instance?.SetVirtualJoystick(StickAxis.Zero);
            }            
        }
    }
}
