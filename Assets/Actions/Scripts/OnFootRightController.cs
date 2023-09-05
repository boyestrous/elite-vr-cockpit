using EVRC.Core.Actions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    using static EVRC.Core.Actions.ActionsController;
    using Direction = ActionsController.Direction;
    using static KeyboardInterface;
    using DirectionActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.DirectionActionChange>;

    public class OnFootRightController : OnFootGrippedController
    {        
        private ActionsControllerPressManager actionsPressManager;

        //@todo move these to a ScriptableObject
        [Header("Control Bindings")]        
        public EDControlButton POV1Press;

        private Vector2 lookAxis = Vector2.zero;

        override protected void OnEnable()
        {
            base.OnEnable();
            actionsPressManager = new ActionsControllerPressManager(this)
                .VectorPOV1(OnPOV1Vector)
                .ButtonPOV1(OnButtonPOV1)
                .DirectionPOV2(OnPOV2Direction)
                ;

            POV1Press = EDControlButton.HumanoidZoomButton;
            PrimaryButton = EDControlButton.HumanoidPrimaryFireButton;
            SecondaryButton = EDControlButton.HumanoidJumpButton;
            AltButton = EDControlButton.HumanoidPrimaryInteractButton; // This isn't available on Vive Wands, assign with caution
        }
        
        // Map of action presses to Elite Dangerous Actions        
        private static Dictionary<Direction, EDControlButton> footActionMap = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.HumanoidPrimaryInteractButton },
            { Direction.Down, EDControlButton.HumanoidItemWheelButton },
        };

        private DirectionActionChangeUnpressHandler OnPOV2Direction(DirectionActionChange ev)
        {
            // If the correct control is not gripped (valid hand)
            // Or the actionMap doesn't contain a key for this direction
            // Return a null unpress
            if (!IsValidHand(ev.hand) || !footActionMap.ContainsKey(ev.direction)) return (uEv) => { };

            EDControlButton button = footActionMap[ev.direction];
            Action unpress = CallbackPress(controlBindingsState.GetControlButton(button));
            return (uEv) => unpress();

        }

        private PressManager.UnpressHandlerDelegate<ActionsController.ActionChange> OnButtonPOV1(ActionsController.ActionChange ev)
        {
            if(!IsValidHand(ev.hand)) return (uEv)=>{};

            Action unpress = CallbackPress(controlBindingsState.GetControlButton(POV1Press));
            return (uEv) => unpress();
        }

        private void OnPOV1Vector(Vector2ActionChangeEvent ev)
        {
            if (!IsValidHand(ev.hand)) return;

            //Debug.LogFormat($"POV1 is Moving: {ev.state.x}/{ev.state.y}");
                       
            // If modifier is active, set to lookaxis to zero
            lookAxis = ev.state;

            UpdateLookAxis();
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
            lookAxis = Vector2.zero;
        }

        protected void UpdateLookAxis()
        {
            var output = vJoyInterface.instance;

            output.SetHumanoidLook(lookAxis);
        }
    }
}
