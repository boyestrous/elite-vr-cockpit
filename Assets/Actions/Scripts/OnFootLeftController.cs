using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    using static EVRC.Core.Actions.ActionsController;
    using Direction = ActionsController.Direction;
    using static KeyboardInterface;
    using static EVRC.Core.Actions.VirtualJoystick;

    public class OnFootLeftController : OnFootGrippedController
    {        
        private ActionsControllerPressManager actionsPressManager;

        public EDControlButton POV3Press;

        private Vector2 moveAxis = Vector2.zero;
        private Vector2 itemWheelAxis = Vector2.zero;

        override protected void OnEnable()
        {
            base.OnEnable();
            actionsPressManager = new ActionsControllerPressManager(this)
                .VectorPOV3(OnPOV3Direction)
                .ButtonPOV3(OnButtonPOV3)
                ;

            POV3Press = EDControlButton.HumanoidSprintButton;
            PrimaryButton = EDControlButton.HumanoidThrowGrenadeButton;
            SecondaryButton = EDControlButton.HumanoidCrouchButton;
            AltButton = EDControlButton.HumanoidItemWheelButton;

        }

        private PressManager.UnpressHandlerDelegate<ActionsController.ActionChange> OnButtonPOV3(ActionsController.ActionChange ev)
        {
            if(!IsValidHand(ev.hand)) return (uEv)=>{};

            Action unpress = CallbackPress(controlBindingsState.GetControlButton(POV3Press));
            return (uEv) => unpress();
        }

        private void OnPOV3Direction(Vector2ActionChangeEvent ev)
        {
            if (!IsValidHand(ev.hand)) return;

            //Debug.LogFormat($"POV3 is Moving: {ev.state.x}/{ev.state.y}");

            // Set moveAxis (non-modified)
            moveAxis = ev.state;

            // Set itemWheelAxis (modified)
            itemWheelAxis = ev.state;

            UpdateMoveAxis();
            UpdateItemWheelAxis();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Reset();
            actionsPressManager.Clear();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void Reset()
        {
            base.Reset();
            moveAxis = Vector2.zero;
            itemWheelAxis = Vector2.zero;
        }

        protected void UpdateMoveAxis()
        {
            var output = vJoyInterface.instance;

            output.SetHumanoidMove(moveAxis);
        }

        protected void UpdateItemWheelAxis()
        {
            var output = vJoyInterface.instance;

            output.SetItemWheelAxis(itemWheelAxis);
        }
    }
}
