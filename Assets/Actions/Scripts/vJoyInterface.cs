using System.Collections.Generic;
using EVRC.Core.Overlay;
using UnityEngine;
using Valve.VR;
using vJoyInterfaceWrap;
using static EVRC.Core.Actions.ActionsController;
using static EVRC.Core.Actions.Virtual6DOFController;

namespace EVRC.Core.Actions
{
    using NameType = NameType;
    /**
     * Behaviour that outputs state to a virtual HOTAS using vJoy
     */
    public class vJoyInterface : MonoBehaviour
    {
        public VJoyState vJoyState;
        public GameEvent vJoyStatusChange;
        public static SteamVR_Events.Event<VJoyStatus> VJoyStatusChange = new SteamVR_Events.Event<VJoyStatus>();

        public static vJoyInterface _instance;
        public static vJoyInterface instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[vJoy]");
            }
        }

        public enum HatDirection : byte
        {
            Up = 0,
            Right = 1,  
            Down = 2,
            Left = 3,
            Neutral = 0xF,
        }

        [Range(0f, 90f)]
        public float joystickDeadzoneDegrees = 0f;
        [Range(0f, 90f)]
        public float joystickMaxDegrees = 90f;
        [Range(0f, 100f)]
        public float directionalThrustersDeadzonePercentage = 0f;

        private vJoy vjoy;

        // These two devices must be configured in the vJoy Configuration GUI
        private vJoy.JoystickState iReport = new vJoy.JoystickState(); // Device 1
        private vJoy.JoystickState iReport2 = new vJoy.JoystickState(); // Device 2

        public static uint deviceId = 1;
        public static uint secondaryDeviceId = 2;
        

        public bool MapAxisEnabled { get; private set; } = false;

        // All axis values should be ratio values between -1 and +1
        private float xAxis = 0f;
        private float yAxis = 0f;
        private float zAxis = 0f;
        private float r_xAxis = 0f;
        private float r_yAxis = 0f;
        private float r_zAxis = 0f;
        private float sliderAxis = 0f;
        private float dialAxis = 0f;
        private Vector3 mapTranslationAxis = Vector3.zero;
        private float mapPitchAxis = 0;
        private float mapYawAxis = 0;
        private float mapZoomAxis = 0;
        private uint buttons = 0;


        // Each value in this list represents a HAT/POV switch. They can each independently be a single direction at a time.
        private HatDirection[] hat = new HatDirection[] {
            HatDirection.Neutral,
            HatDirection.Neutral,
            HatDirection.Neutral,
            HatDirection.Neutral,
        };

        void SetStatus(VJoyStatus status)
        {
            vJoyState.vJoyStatus = status;
            vJoyStatusChange.Raise();
        }

        void OnEnable()
        {
            vjoy = new vJoy();

            if (!vjoy.vJoyEnabled())
            {
                SetStatus(VJoyStatus.NotInstalled);
                enabled = false;
                return;
            }

            uint DllVer = 0, DrvVer = 0;
            bool match = vjoy.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
            {
                Debug.LogFormat("vJoy Driver Version Matches vJoy DLL Version ({0:X})", DllVer);
            }
            else
            {
                Debug.LogErrorFormat("vJoy Driver Version ({0:X}) does NOT match vJoy DLL Version ({1:X})", DrvVer, DllVer);
                SetStatus(VJoyStatus.VersionMismatch);
                enabled = false;
                return;
            }


            VjdStat deviceStatus = vjoy.GetVJDStatus(deviceId);
            VjdStat secondaryDeviceStatus = vjoy.GetVJDStatus(secondaryDeviceId);
            if (!IsDeviceStatusOk(deviceId, deviceStatus) || !IsDeviceStatusOk(secondaryDeviceId, secondaryDeviceStatus))
            {
                enabled = false;
                return;
            }

            if (!IsDeviceValid(deviceId))
            {
                Debug.LogError("vJoy device is not configured correctly");
                SetStatus(VJoyStatus.DeviceMisconfigured);
                enabled = false;
                return;
            }

            if (!IsSecondaryDeviceValid(deviceId))
            {
                Debug.LogError("Secondary vJoy device is not configured correctly");
                SetStatus(VJoyStatus.DeviceMisconfigured);
                enabled = false;
                return;
            }

            if (!AcquireDevice(deviceId, deviceStatus) || !AcquireDevice(secondaryDeviceId, secondaryDeviceStatus))
            {
                enabled = false;
                return;
            }

            SetStatus(VJoyStatus.Ready);
        }

        /**
         * Verify that a vJoy device is vailable
         */
        private bool IsDeviceStatusOk(uint deviceId, VjdStat deviceStatus)
        {
            switch (deviceStatus)
            {
                case VjdStat.VJD_STAT_FREE:
                case VjdStat.VJD_STAT_OWN:
                    // We can continue if the device is free or we own it
                    return true;
                case VjdStat.VJD_STAT_MISS:
                    Debug.LogWarningFormat("vJoy Device {0} is not installed or is disabled", deviceId);
                    SetStatus(VJoyStatus.DeviceUnavailable);
                    return false;
                case VjdStat.VJD_STAT_BUSY:
                    Debug.LogWarningFormat("vJoy Device {0} is owned by another application", deviceId);
                    SetStatus(VJoyStatus.DeviceOwned);
                    return false;
                default:
                    Debug.LogError("Unknown vJoy device status error");
                    SetStatus(VJoyStatus.DeviceError);
                    return false;
            }
        }

        /**
         * Aquire or verify a vJoy device is already aquired
         */
        private bool AcquireDevice(uint deviceId, VjdStat deviceStatus)
        {
            if (deviceStatus == VjdStat.VJD_STAT_FREE)
            {
                if (vjoy.AcquireVJD(deviceId))
                {
                    Debug.LogFormat("Aquired vJoy device {0}", deviceId);
                }
                else
                {
                    Debug.LogErrorFormat("Unable to aquire vJoy device {0}", deviceId);
                    SetStatus(VJoyStatus.DeviceNotAquired);
                    return false;
                }
            }
            else if (deviceStatus == VjdStat.VJD_STAT_OWN)
            {
                Debug.LogFormat("vJoy device {0} already aquired", deviceId);
            }

            return true;
        }

        void OnDisable()
        {
            if (vJoyState.vJoyStatus == VJoyStatus.Ready)
            {
                vjoy.RelinquishVJD(deviceId);
                vjoy.RelinquishVJD(secondaryDeviceId);
                SetStatus(VJoyStatus.Unknown);
            }
        }

        /**
         * Checks to make sure the vJoy device has all the required configuration
         * @note Make sure to update this when adding code that adds buttons, axis, haptics, etc
         */
        private bool IsDeviceValid(uint deviceId)
        {
            var buttonN = vjoy.GetVJDButtonNumber(deviceId);
            var hatN = vjoy.GetVJDDiscPovNumber(deviceId);

            if (buttonN < 8)
            {
                Debug.LogWarningFormat("vJoy device has {0} buttons, at least 8 are required", buttonN);
                return false;
            }

            if (hatN < 4)
            {
                Debug.LogWarningFormat("vJoy device has {0} directional pov hat switches, 4 configured as directional are required", hatN);
                return false;
            }

            var xAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_X);
            var yAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_Y);
            var rzAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RZ);
            if (!xAxis || !yAxis || !rzAxis)
            {
                Debug.LogWarningFormat("vJoy device is missing one of the X/Y/Rz axis needed for the joystick [X:{0}, Y: {1}, Rz:{2}]", xAxis, yAxis, rzAxis);
                return false;
            }

            var zAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_Z);
            if (!zAxis)
            {
                Debug.LogWarning("vJoy device is missing the Z axis needed for the throttle");
                return false;
            }

            var rxAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RX);
            var ryAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RY);
            var sliderAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_SL0);
            if (!rxAxis || !ryAxis || !sliderAxis)
            {
                // Slider Axis is Joy_UAxis in the bindings file
                Debug.LogWarningFormat("vJoy device is missing one of the Rx/Ry/Slider axis needed for the thruster axis [Rx:{0}, Ry: {1}, Slider:{2}]", rxAxis, ryAxis, sliderAxis);
                return false;
            }

            var dialAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_SL1);
            if (!dialAxis)
            {
                // Slider2 a.k.a. 'Dial' Axis is Joy_VAxis in the bindings file
                Debug.LogWarning("vJoy device is missing the Dial/Slider2 axis needed for the map zoom axis");
                return false;
            }

            return true;
        }

        /**
         * Checks to make sure the vJoy device used for secondary axis has all the required configuration
         * @note Make sure to update this when adding code that adds buttons, axis, haptics, etc
         */
        private bool IsSecondaryDeviceValid(uint deviceId)
        {
            var xAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_X);
            var yAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_Y);
            var zAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_Z);
            if (!xAxis || !yAxis || !zAxis)
            {
                Debug.LogWarningFormat("vJoy device is missing one of the X/Y/Z axis needed for galaxy map movement [X:{0}, Y: {1}, Z:{2}]", xAxis, yAxis, zAxis);
                return false;
            }

            var rxAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RX);
            var rzAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RZ);
            if (!rxAxis || !rzAxis)
            {
                Debug.LogWarningFormat("vJoy device is missing one of the Rx/Rz axis needed for the galaxy map pitch/rotation [Rx:{0}, Rz: {1}]", rxAxis, rzAxis);
                return false;
            }

            var dialAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_SL1);
            if (!dialAxis)
            {
                Debug.LogWarning("vJoy device is missing the Dial/Slider2 axis needed for the map zoom axis");
                return false;
            }

            return true;
        }

        /**
         * Update the joystick axis
         */
        public void SetVirtualJoystick(VirtualJoystick.StickAxis axis)
        {
            //stickAxis = axis;
            var stick = axis.WithDeadzone(joystickDeadzoneDegrees);
            
            // @todo fix this so it passes normalized vectors instead of doing the conversion here
            yAxis = -stick.Pitch / joystickMaxDegrees;
            xAxis = stick.Roll / joystickMaxDegrees;
            r_zAxis = stick.Yaw / joystickMaxDegrees;
        }

        /**
         * Update the axis used for directional thrusters
         */
        public void SetThrusters(Virtual6DOFController.ThrusterAxis axis)
        {
            //thrusterAxis = axis;

            var dThrusters = axis.WithDeadzone(directionalThrustersDeadzonePercentage / 100f);

            Vector3 simpleThruster = dThrusters.Value.normalized;        

            r_xAxis = simpleThruster.x;
            r_yAxis = simpleThruster.y;
            sliderAxis = simpleThruster.z;
        }

        /**
         * Update the throttle
         */
        public void SetThrottle(float throttle)
        {
            //this.throttle = throttle;
            zAxis = throttle;
        }

        /**
         * Update the axes for pitch/rotate in Humanoid mode
         */
        public void SetHumanoidLook(Vector2 lookAxis)
        {
            yAxis = lookAxis.y;
            r_zAxis = lookAxis.x;
        }

        /**
         * Update the axes for Humanoid movement (walk/strafe)
         */
        public void SetHumanoidMove(Vector2 moveAxis) 
        {            
            xAxis = moveAxis.x; 
            zAxis = moveAxis.y;
        }

        /**
        * Update the Humanoid Item Wheel Axis 
        */
        public void SetItemWheelAxis(Vector2 itemAxis)
        {
            sliderAxis = itemAxis.x;
            dialAxis = itemAxis.y;
        }

        /**
         * Update the radar range (sensor zoom)
         */
        public void SetRadarRange(float newRadarRange)
        {
            dialAxis = newRadarRange;
        }
        /**
         * Update the fss tuning
         */
        public void SetFSSTuning(float fssTuning)
        {
            dialAxis = fssTuning;
        }

        /**
         * Enable the map control axis
         */
        public void EnableMapAxis()
        {
            MapAxisEnabled = true;
            ResetMapAxis();
        }

        /**
         * Disable the map control axis
         */
        public void DisableMapAxis()
        {
            MapAxisEnabled = false;
            ResetMapAxis();
        }

        private void ResetMapAxis()
        {
            mapTranslationAxis = Vector3.zero;
        }

        /**
         * Update the map translation axis (device 1)
         */
        public void SetMapTranslationAxis(Vector3 translation)
        {
            mapTranslationAxis = translation;
        }

        /**
         * Update the map pitch axis (device 1)
         */
        public void SetMapPitchAxis(float pitch)
        {
            mapPitchAxis = pitch;
        }

        /**
         * Update the map yaw axis (device 1)
         */
        public void SetMapYawAxis(float yaw)
        {
            mapYawAxis = yaw;
        }

        /**
         * Update the map zoom axis (device 1)
         */
        public void SetMapZoomAxis(float zoom)
        {
            mapZoomAxis = zoom;
        }

        /**
         * Update the pressed state of a button
         */
        public void SetButton(uint buttonNumber, bool pressed)
        {
            int buttonIndex = (int)buttonNumber - 1;
            if (buttonNumber == 0)
            {
                throw new System.IndexOutOfRangeException("Button number 0 is too low, button numbers are zero indexed");
            }
            if (buttonIndex >= 32)
            {
                throw new System.IndexOutOfRangeException(string.Format("Button index {0} is too high", buttonIndex));
            }

            if (pressed)
            {
                buttons |= (uint)1 << buttonIndex;
            }
            else
            {
                buttons &= ~((uint)1 << buttonIndex);
            }
        }

        public void SetHatDirection(uint hatNumber, HatDirection dir)
        {
            int hatIndex = (int)hatNumber - 1;
            if (hatNumber == 0)
            {
                throw new System.IndexOutOfRangeException("Button number 0 is too low, button numbers are zero indexed");
            }
            if (hatIndex > hat.Length)
            {
                throw new System.IndexOutOfRangeException(string.Format("HAT index {0} is too high", hatIndex));
            }

            hat[hatIndex] = dir;
        }

        /// <summary>
        /// Convert an Axis ratio (joystick degrees/max degrees) into an integer readable by vJoy.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="axisRatio"></param>
        /// <param name="hid"></param>
        /// <returns></returns>
        int ConvertAxisRatioToAxisInt(uint deviceId, float axisRatio, HID_USAGES hid)
        {
            long min = 0, max = 0;

            // Attempt to get the minimum axis value using the provided vjoy library
            // gotMin will be true if the operation succeeded, and the retrieved value will be stored in 'min'
            bool gotMin = vjoy.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_X, ref min);

            // Attempt to get the maximum axis value using the provided vjoy library
            // gotMax will be true if the operation succeeded, and the retrieved value will be stored in 'max'
            bool gotMax = vjoy.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_X, ref max);

            // Check if either getting the minimum or maximum value failed
            if (!gotMin || !gotMax)
            {
                // Return 0 to indicate an error condition or an inability to calculate the result
                Debug.LogError($"Error getting min/max of HID axis {hid.ToString()}");
                return 0;
            }

            // Get an absolute ratio where 0 is -Max, .5 is 0, and 1 is +Max
            float absRatio = axisRatio / 2f + .5f;
            long range = max - min;
            return (int)((long)(range * absRatio) + min);
        }

        int ConvertStickAxisDegreesToAxisInt(uint deviceId, float axisDegres, HID_USAGES hid)
        {
            return ConvertAxisRatioToAxisInt(deviceId, axisDegres / joystickMaxDegrees, hid);
        }

        void Update()
        {
            iReport.bDevice = (byte)deviceId;
            iReport2.bDevice = (byte)secondaryDeviceId;

            // Device 1, joystick/throttle + humanoid
            if (!MapAxisEnabled)
            {

                iReport.AxisY = ConvertAxisRatioToAxisInt(deviceId, yAxis, HID_USAGES.HID_USAGE_Y);
                iReport.AxisX = ConvertAxisRatioToAxisInt(deviceId, xAxis, HID_USAGES.HID_USAGE_X);
                iReport.AxisZRot = ConvertAxisRatioToAxisInt(deviceId, r_zAxis, HID_USAGES.HID_USAGE_RZ);
                iReport.AxisXRot = ConvertAxisRatioToAxisInt(deviceId, r_xAxis, HID_USAGES.HID_USAGE_RX);
                iReport.AxisYRot = ConvertAxisRatioToAxisInt(deviceId, r_yAxis, HID_USAGES.HID_USAGE_RY);
                iReport.Slider = ConvertAxisRatioToAxisInt(deviceId, sliderAxis, HID_USAGES.HID_USAGE_SL0); // (a.k.a. UAxis in vJoy)
                iReport.AxisZ = ConvertAxisRatioToAxisInt(deviceId, zAxis, HID_USAGES.HID_USAGE_Z);
                iReport.Dial = ConvertAxisRatioToAxisInt(deviceId, dialAxis, HID_USAGES.HID_USAGE_SL1); // (a.k.a. VAxis in vJoy)
            }
            else
            {
                ResetAll();
            }

            iReport.Buttons = buttons;

            iReport.bHats = (uint)((byte)hat[3] << 12)
                | (uint)((byte)hat[2] << 8)
                | (uint)((byte)hat[1] << 4)
                | (uint)hat[0];

            // Device 2, primarily map
            if (MapAxisEnabled)
            {
                // Translation
                iReport2.AxisX = ConvertAxisRatioToAxisInt(secondaryDeviceId, mapTranslationAxis.x, HID_USAGES.HID_USAGE_X);
                iReport2.AxisY = ConvertAxisRatioToAxisInt(secondaryDeviceId, mapTranslationAxis.y, HID_USAGES.HID_USAGE_Y);
                iReport2.AxisZ = ConvertAxisRatioToAxisInt(secondaryDeviceId, mapTranslationAxis.z, HID_USAGES.HID_USAGE_Z);

                // Pitch / Yaw
                iReport2.AxisYRot = ConvertAxisRatioToAxisInt(secondaryDeviceId, mapPitchAxis, HID_USAGES.HID_USAGE_RX);
                iReport2.AxisXRot = ConvertAxisRatioToAxisInt(secondaryDeviceId, mapYawAxis, HID_USAGES.HID_USAGE_RZ);

                // FSS tuning axis (a.k.a. VAxis in vJoy)
                iReport2.Dial = ConvertAxisRatioToAxisInt(deviceId, dialAxis, HID_USAGES.HID_USAGE_SL1);

                // Zoom
                iReport2.AxisZRot = ConvertAxisRatioToAxisInt(secondaryDeviceId, -mapZoomAxis, HID_USAGES.HID_USAGE_SL1);
            }
            else
            {
                // Make sure the map axis are reset
                iReport2.AxisX = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_X);
                iReport2.AxisY = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_Y);
                iReport2.AxisZ = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_Z);
                iReport2.AxisXRot = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_RX);
                iReport2.AxisYRot = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_RX);
                iReport2.AxisZRot = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_RZ);
                iReport2.Dial = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_SL1);
            }

            if (!vjoy.UpdateVJD(deviceId, ref iReport))
            {
                Debug.LogFormat("vJoy Device {0} update failed", deviceId);
                SetStatus(VJoyStatus.DeviceError);
                enabled = false;
            }
            if (!vjoy.UpdateVJD(secondaryDeviceId, ref iReport2))
            {
                Debug.LogFormat("vJoy Device {0} update failed", secondaryDeviceId);
                SetStatus(VJoyStatus.DeviceError);
                enabled = false;
            }
        }

        public void ResetAll()
        {
            iReport.AxisY = 0;
            iReport.AxisX = 0;
            iReport.AxisZRot = 0;
            iReport.AxisXRot = 0;
            iReport.AxisYRot = 0;
            iReport.Slider = 0;
            iReport.AxisZ = 0;
            iReport.Dial = 0;
            iReport2.AxisX = 0;
            iReport2.AxisY = 0;
            iReport2.AxisZ = 0;
            iReport2.AxisXRot = 0;
            iReport2.AxisYRot = 0;
            iReport2.AxisZRot = 0;
            iReport2.Dial = 0;
        }
    }
}
