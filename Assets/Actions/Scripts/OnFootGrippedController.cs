using EVRC.Core.Actions;
using System;
using UnityEngine;

namespace EVRC.Core
{
    using static KeyboardInterface;

    public class OnFootGrippedController : VirtualControlButtons
    {
        public ControlBindingsState controlBindingsState;
        private ActionsControllerPressManager actionsPressManager;

        //@todo move these to a ScriptableObject
        public EDControlButton PrimaryButton;
        public EDControlButton SecondaryButton;
        public EDControlButton AltButton;

        override protected void OnEnable()
        {
            base.OnEnable();
            actionsPressManager = new ActionsControllerPressManager(this)
                .ButtonPrimary(OnButtonPrimary)
                .ButtonSecondary(OnButtonSecondary)
                .ButtonAlt(OnButtonAlt)
                ;
        }
        private PressManager.UnpressHandlerDelegate<ActionsController.ActionChange> OnButtonAlt(ActionsController.ActionChange ev)
        {
            if (!IsValidHand(ev.hand)) return (uEv) => { };

            Debug.LogFormat($"Alt pressed with hand: {ev.hand}");
            Action unpress = CallbackPress(controlBindingsState.GetControlButton(AltButton));
            return (uEv) => unpress();
        }

        private PressManager.UnpressHandlerDelegate<ActionsController.ActionChange> OnButtonSecondary(ActionsController.ActionChange ev)
        {
            if (!IsValidHand(ev.hand)) return (uEv) => { };

            Debug.LogFormat($"Secondary pressed with hand: {ev.hand}");
            Action unpress = CallbackPress(controlBindingsState.GetControlButton(SecondaryButton));
            return (uEv) => unpress();
        }

        private PressManager.UnpressHandlerDelegate<ActionsController.ActionChange> OnButtonPrimary(ActionsController.ActionChange ev)
        {
            if (!IsValidHand(ev.hand)) return (uEv) => { };

            Debug.LogFormat($"Primary pressed with hand: {ev.hand}");
            Action unpress = CallbackPress(controlBindingsState.GetControlButton(PrimaryButton));
            return (uEv) => unpress();
        }


        public virtual void Reset()
        {

        }
    }
}
