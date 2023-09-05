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
    using DirectionActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.DirectionActionChange>;

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
                .VectorPOV3(OnPOV3Vector)
                .ButtonPOV3(OnButtonPOV3)
                ;

            POV3Press = EDControlButton.HumanoidSprintButton;
            PrimaryButton = EDControlButton.HumanoidThrowGrenadeButton;
            SecondaryButton = EDControlButton.HumanoidCrouchButton;
            AltButton = EDControlButton.HumanoidMeleeButton; // This isn't available on Vive Wands, assign with caution

        }

        // Map of action presses to Elite Dangerous Actions        
        //private static Dictionary<Direction, EDControlButton> footActionMap = new Dictionary<Direction, EDControlButton>()
        //{
        //    { Direction.Up, EDControlButton.HumanoidItemWheelButton },
        //};

        //private DirectionActionChangeUnpressHandler OnPOV3Direction(DirectionActionChange ev)
        //{
        //    // If the correct control is not gripped (valid hand)
        //    // Or the actionMap doesn't contain a key for this direction
        //    // Return a null unpress
        //    if (!IsValidHand(ev.hand) || !footActionMap.ContainsKey(ev.direction)) return (uEv) => { };

        //    EDControlButton button = footActionMap[ev.direction];
        //    Action unpress = CallbackPress(controlBindingsState.GetControlButton(button));
        //    return (uEv) => unpress();

        //}

        private PressManager.UnpressHandlerDelegate<ActionsController.ActionChange> OnButtonPOV3(ActionsController.ActionChange ev)
        {
            if(!IsValidHand(ev.hand)) return (uEv)=>{};

            Action unpress = CallbackPress(controlBindingsState.GetControlButton(POV3Press));
            return (uEv) => unpress();
        }

        private void OnPOV3Vector(Vector2ActionChangeEvent ev)
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
