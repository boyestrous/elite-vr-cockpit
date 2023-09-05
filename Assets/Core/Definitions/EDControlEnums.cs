﻿namespace EVRC.Core
{
        public enum EDControlAxis
        {
            YawAxisRaw,
            RollAxisRaw,
            PitchAxisRaw,
            LateralThrustRaw,
            VerticalThrustRaw,
            AheadThrust,
            YawAxisAlternate,
            RollAxisAlternate,
            PitchAxisAlternate,
            LateralThrustAlternate,
            VerticalThrustAlternate,
            ThrottleAxis,
            YawAxis_Landing,
            PitchAxis_Landing,
            RollAxis_Landing,
            LateralThrust_Landing,
            VerticalThrust_Landing,
            AheadThrust_Landing,
            RadarRangeAxis,
            HeadLookPitchAxisRaw,
            HeadLookYawAxis,
            CamPitchAxis,
            CamYawAxis,
            CamTranslateYAxis,
            CamTranslateXAxis,
            CamTranslateZAxis,
            CamZoomAxis,
            SteeringAxis,
            BuggyRollAxisRaw,
            BuggyPitchAxis,
            BuggyTurretYawAxisRaw,
            BuggyTurretPitchAxisRaw,
            DriveSpeedAxis,
            IncreaseSpeedButtonPartial,
            DecreaseSpeedButtonPartial,
            MultiCrewThirdPersonYawAxisRaw,
            MultiCrewThirdPersonPitchAxisRaw,
            MultiCrewThirdPersonFovAxisRaw,
            MoveFreeCamY,
            MoveFreeCamX,
            MoveFreeCamZ,
            MoveFreeCamUpAxis,
            MoveFreeCamDownAxis,
            PitchCamera,
            YawCamera,
            RollCamera,
            StorePitchCamera,
            StoreYawCamera,
            CommanderCreator_Rotation,
            ExplorationFSSCameraPitch,
            ExplorationFSSCameraYaw,
            ExplorationFSSRadioTuningX_Raw,
            ExplorationFSSRadioTuningAbsoluteX,
            SAAThirdPersonYawAxisRaw,
            SAAThirdPersonPitchAxisRaw,
            SAAThirdPersonFovAxisRaw,
        }

        public enum EDControlButton
        {
            MouseReset,
            YawLeftButton,
            YawRightButton,
            YawToRollButton,
            RollLeftButton,
            RollRightButton,
            PitchUpButton,
            PitchDownButton,
            LeftThrustButton,
            RightThrustButton,
            UpThrustButton,
            DownThrustButton,
            ForwardThrustButton,
            BackwardThrustButton,
            UseAlternateFlightValuesToggle,
            ToggleReverseThrottleInput,
            ForwardKey,
            BackwardKey,
            SetSpeedMinus100,
            SetSpeedMinus75,
            SetSpeedMinus50,
            SetSpeedMinus25,
            SetSpeedZero,
            SetSpeed25,
            SetSpeed50,
            SetSpeed75,
            SetSpeed100,
            YawLeftButton_Landing,
            YawRightButton_Landing,
            PitchUpButton_Landing,
            PitchDownButton_Landing,
            RollLeftButton_Landing,
            RollRightButton_Landing,
            LeftThrustButton_Landing,
            RightThrustButton_Landing,
            UpThrustButton_Landing,
            DownThrustButton_Landing,
            ForwardThrustButton_Landing,
            BackwardThrustButton_Landing,
            ToggleFlightAssist,
            UseBoostJuice,
            HyperSuperCombination,
            Supercruise,
            Hyperspace,
            DisableRotationCorrectToggle,
            OrbitLinesToggle,
            SelectTarget,
            CycleNextTarget,
            CyclePreviousTarget,
            SelectHighestThreat,
            CycleNextHostileTarget,
            CyclePreviousHostileTarget,
            TargetWingman0,
            TargetWingman1,
            TargetWingman2,
            SelectTargetsTarget,
            WingNavLock,
            CycleNextSubsystem,
            CyclePreviousSubsystem,
            TargetNextRouteSystem,
            PrimaryFire,
            SecondaryFire,
            CycleFireGroupNext,
            CycleFireGroupPrevious,
            DeployHardpointToggle,
            ToggleButtonUpInput,
            DeployHeatSink,
            ShipSpotLightToggle,
            RadarIncreaseRange,
            RadarDecreaseRange,
            IncreaseEnginesPower,
            IncreaseWeaponsPower,
            IncreaseSystemsPower,
            ResetPowerDistribution,
            HMDReset,
            ToggleCargoScoop,
            EjectAllCargo,
            LandingGearToggle,
            MicrophoneMute,
            UseShieldCell,
            FireChaffLauncher,
            ChargeECM,
            WeaponColourToggle,
            EngineColourToggle,
            NightVisionToggle,
            UIFocus,
            FocusLeftPanel,
            FocusCommsPanel,
            QuickCommsPanel,
            FocusRadarPanel,
            FocusRightPanel,
            GalaxyMapOpen,
            SystemMapOpen,
            ShowPGScoreSummaryInput,
            HeadLookToggle,
            Pause,
            FriendsMenu,
            OpenCodexGoToDiscovery,
            PlayerHUDModeToggle,
            ExplorationFSSEnter,
            UI_Up,
            UI_Down,
            UI_Left,
            UI_Right,
            UI_Select,
            UI_Back,
            UI_Toggle,
            CycleNextPanel,
            CyclePreviousPanel,
            CycleNextPage,
            CyclePreviousPage,
            HeadLookReset,
            HeadLookPitchUp,
            HeadLookPitchDown,
            HeadLookYawLeft,
            HeadLookYawRight,
            CamPitchUp,
            CamPitchDown,
            CamYawLeft,
            CamYawRight,
            CamTranslateForward,
            CamTranslateBackward,
            CamTranslateLeft,
            CamTranslateRight,
            CamTranslateUp,
            CamTranslateDown,
            CamZoomIn,
            CamZoomOut,
            CamTranslateZHold,
            GalaxyMapHome,
            ToggleDriveAssist,
            SteerLeftButton,
            SteerRightButton,
            BuggyRollLeftButton,
            BuggyRollRightButton,
            BuggyPitchUpButton,
            BuggyPitchDownButton,
            VerticalThrustersButton,
            BuggyPrimaryFireButton,
            BuggySecondaryFireButton,
            AutoBreakBuggyButton,
            HeadlightsBuggyButton,
            ToggleBuggyTurretButton,
            BuggyCycleFireGroupNext,
            BuggyCycleFireGroupPrevious,
            SelectTarget_Buggy,
            BuggyTurretYawLeftButton,
            BuggyTurretYawRightButton,
            BuggyTurretPitchUpButton,
            BuggyTurretPitchDownButton,
            BuggyToggleReverseThrottleInput,
            IncreaseSpeedButtonMax,
            DecreaseSpeedButtonMax,
            IncreaseEnginesPower_Buggy,
            IncreaseWeaponsPower_Buggy,
            IncreaseSystemsPower_Buggy,
            ResetPowerDistribution_Buggy,
            ToggleCargoScoop_Buggy,
            EjectAllCargo_Buggy,
            RecallDismissShip,
            UIFocus_Buggy,
            FocusLeftPanel_Buggy,
            FocusCommsPanel_Buggy,
            QuickCommsPanel_Buggy,
            FocusRadarPanel_Buggy,
            FocusRightPanel_Buggy,
            GalaxyMapOpen_Buggy,
            SystemMapOpen_Buggy,
            OpenCodexGoToDiscovery_Buggy,
            PlayerHUDModeToggle_Buggy,
            HeadLookToggle_Buggy,
            MultiCrewToggleMode,
            MultiCrewPrimaryFire,
            MultiCrewSecondaryFire,
            MultiCrewPrimaryUtilityFire,
            MultiCrewSecondaryUtilityFire,
            MultiCrewThirdPersonYawLeftButton,
            MultiCrewThirdPersonYawRightButton,
            MultiCrewThirdPersonPitchUpButton,
            MultiCrewThirdPersonPitchDownButton,
            MultiCrewThirdPersonFovOutButton,
            MultiCrewThirdPersonFovInButton,
            MultiCrewCockpitUICycleForward,
            MultiCrewCockpitUICycleBackward,
            OrderRequestDock,
            OrderDefensiveBehaviour,
            OrderAggressiveBehaviour,
            OrderFocusTarget,
            OrderHoldFire,
            OrderHoldPosition,
            OrderFollow,
            OpenOrders,
            PhotoCameraToggle,
            PhotoCameraToggle_Buggy,
            VanityCameraScrollLeft,
            VanityCameraScrollRight,
            ToggleFreeCam,
            VanityCameraOne,
            VanityCameraTwo,
            VanityCameraThree,
            VanityCameraFour,
            VanityCameraFive,
            VanityCameraSix,
            VanityCameraSeven,
            VanityCameraEight,
            VanityCameraNine,
            FreeCamToggleHUD,
            FreeCamSpeedInc,
            FreeCamSpeedDec,
            ToggleReverseThrottleInputFreeCam,
            MoveFreeCamForward,
            MoveFreeCamBackwards,
            MoveFreeCamRight,
            MoveFreeCamLeft,
            MoveFreeCamUp,
            MoveFreeCamDown,
            PitchCameraUp,
            PitchCameraDown,
            YawCameraLeft,
            YawCameraRight,
            RollCameraLeft,
            RollCameraRight,
            ToggleRotationLock,
            FixCameraRelativeToggle,
            FixCameraWorldToggle,
            QuitCamera,
            ToggleAdvanceMode,
            FreeCamZoomIn,
            FreeCamZoomOut,
            FStopDec,
            FStopInc,
            StoreEnableRotation,
            StoreCamZoomIn,
            StoreCamZoomOut,
            StoreToggle,
            CommanderCreator_Undo,
            CommanderCreator_Redo,
            CommanderCreator_Rotation_MouseToggle,
            GalnetAudio_Play_Pause,
            GalnetAudio_SkipForward,
            GalnetAudio_SkipBackward,
            GalnetAudio_ClearQueue,
            ExplorationFSSCameraPitchIncreaseButton,
            ExplorationFSSCameraPitchDecreaseButton,
            ExplorationFSSCameraYawIncreaseButton,
            ExplorationFSSCameraYawDecreaseButton,
            ExplorationFSSZoomIn,
            ExplorationFSSZoomOut,
            ExplorationFSSMiniZoomIn,
            ExplorationFSSMiniZoomOut,
            ExplorationFSSRadioTuningX_Increase,
            ExplorationFSSRadioTuningX_Decrease,
            ExplorationFSSDiscoveryScan,
            ExplorationFSSQuit,
            ExplorationFSSTarget,
            ExplorationFSSShowHelp,
            ExplorationSAAChangeScannedAreaViewToggle,
            ExplorationSAAExitThirdPerson,
            SAAThirdPersonYawLeftButton,
            SAAThirdPersonYawRightButton,
            SAAThirdPersonPitchUpButton,
            SAAThirdPersonPitchDownButton,
            SAAThirdPersonFovOutButton,
            SAAThirdPersonFovInButton,
            // Custom.4.0 additions
            TriggerFieldNeutraliser,
            VanityCameraTen,
            ExplorationSAANextGenus,
            ExplorationSAAPreviousGenus,
            ExplorationSAAShowHelp,
            // Humanoid buttons from Custom.4.0
            HumanoidForwardButton,
            HumanoidBackwardButton,
            HumanoidStrafeLeftButton,
            HumanoidStrafeRightButton,
            HumanoidRotateLeftButton,
            HumanoidRotateRightButton,
            HumanoidPitchUpButton,
            HumanoidPitchDownButton,
            HumanoidSprintButton,
            HumanoidWalkButton,
            HumanoidCrouchButton,
            HumanoidJumpButton,
            HumanoidPrimaryInteractButton,
            HumanoidSecondaryInteractButton,
            HumanoidItemWheelButton,
            HumanoidEmoteWheelButton,
            HumanoidItemWheelButton_XLeft,
            HumanoidItemWheelButton_XRight,
            HumanoidItemWheelButton_YUp,
            HumanoidItemWheelButton_YDown,
            HumanoidPrimaryFireButton,
            HumanoidZoomButton,
            HumanoidThrowGrenadeButton,
            HumanoidMeleeButton,
            HumanoidReloadButton,
            HumanoidSelectPrimaryWeaponButton,
            HumanoidSelectSecondaryWeaponButton,
            HumanoidSelectUtilityWeaponButton,
            HumanoidSelectNextWeaponButton,
            HumanoidSelectPreviousWeaponButton,
            HumanoidHideWeaponButton,
            HumanoidSelectNextGrenadeTypeButton,
            HumanoidSelectPreviousGrenadeTypeButton,
            HumanoidToggleFlashlightButton,
            HumanoidToggleNightVisionButton,
            HumanoidToggleShieldsButton,
            HumanoidClearAuthorityLevel,
            HumanoidHealthPack,
            HumanoidBattery,
            HumanoidSelectFragGrenade,
            HumanoidSelectEMPGrenade,
            HumanoidSelectShieldGrenade,
            HumanoidSwitchToRechargeTool,
            HumanoidSwitchToCompAnalyser,
            HumanoidSwitchToSuitTool,
            HumanoidToggleToolModeButton,
            HumanoidToggleMissionHelpPanelButton,
            HumanoidPing,
            GalaxyMapOpen_Humanoid,
            SystemMapOpen_Humanoid,
            FocusCommsPanel_Humanoid,
            QuickCommsPanel_Humanoid,
            HumanoidOpenAccessPanelButton,
            HumanoidConflictContextualUIButton,
    }
    
}