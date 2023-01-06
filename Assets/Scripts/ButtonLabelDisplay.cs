﻿using System.Reflection.Emit;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Utils = OverlayUtils;

    public class ButtonLabelDisplay : MonoBehaviour
    {
        protected string id;
        protected Color color = Color.white;
        public float width = .1f;
        public string label;
        public RenderTexture renderTexture;

        protected uint num = 0;

        private ulong handle = OpenVR.k_ulOverlayHandleInvalid;

        public string key
        {
            get
            {
                return Utils.GetKey(GetComponent<ControlButton>().name, GetComponent<ControlButton>().GetInstanceID().ToString());
            }
        }

        void Start()
        {
            
        }

        void OnEnable()
        {
            renderTexture= GetComponent<ControlButton>().renderTexture;
            label = GetComponent<ControlButton>().label;
            id = key;            
            Refresh();
        }

        private void OnValidate()
        {
            if (Application.isPlaying && enabled)
            {
                Refresh();
            }
        }

        void Refresh()
        {
            
        }

         void Update()
        {
            var overlay = OpenVR.Overlay;
            if (overlay == null) return;

            if (handle == OpenVR.k_ulOverlayHandleInvalid)
            {
                Utils.CreateOverlay(key, gameObject.name, ref handle);
            }
            var o = new Utils.OverlayHelper(handle);
            if (renderTexture && renderTexture.IsCreated() && o.Valid)
            {
                o.Show();

                o.SetColorWithAlpha(color);
                o.SetWidthInMeters(width);

                o.SetInputMethod(VROverlayInputMethod.None);
                o.SetMouseScale(1, 1);

                var offset = new SteamVR_Utils.RigidTransform(transform);
                if (!Utils.IsFacingHmd(transform))
                {
                    offset.rot = offset.rot * Quaternion.AngleAxis(180, Vector3.up);
                }
                o.SetFullTexture(renderTexture);
                o.SetTransformAbsolute(ETrackingUniverseOrigin.TrackingUniverseStanding, offset);
            }
        }

        void OnDisable()
        {
            renderTexture.DiscardContents();

            var o = new Utils.OverlayHelper(handle, false);
            if (o.Valid)
            {
                o.Destroy();
            }

            handle = OpenVR.k_ulOverlayHandleInvalid;
        }
    }
}
