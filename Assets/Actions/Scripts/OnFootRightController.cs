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
                .VectorPOV1(OnPOV1Direction)
                .ButtonPOV1(OnButtonPOV1)
                ;

            POV1Press = EDControlButton.HumanoidZoomButton;
            PrimaryButton = EDControlButton.HumanoidPrimaryFireButton;
            SecondaryButton = EDControlButton.HumanoidJumpButton;
            AltButton = EDControlButton.HumanoidPrimaryInteractButton;
        }

        private PressManager.UnpressHandlerDelegate<ActionsController.ActionChange> OnButtonPOV1(ActionsController.ActionChange ev)
        {
            if(!IsValidHand(ev.hand)) return (uEv)=>{};

            Action unpress = CallbackPress(controlBindingsState.GetControlButton(POV1Press));
            return (uEv) => unpress();
        }

        private void OnPOV1Direction(Vector2ActionChangeEvent ev)
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
