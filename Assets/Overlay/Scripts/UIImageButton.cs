﻿using UnityEngine;
using UnityEngine.UI;

namespace EVRC.Core.Overlay
{
    using Utils = OverlayUtils;

    /**
     * A helper that allows a UI.Image to be used in place of a HolographicButton
     */
    [RequireComponent(typeof(Image), typeof(HolographicColor))]
    public class UIImageButton : MonoBehaviour, IHighlightable
    {
        public Sprite backface;
        private Texture lastTexture;
        private Sprite sprite;
        private bool lastIsFacingHmd = true;
        private Image _image;
        private Color activeColor;
        private Color baseColor;
        private Color highlightColor;
        private Color invalidColor;


        private Image image
        {
            get
            {
                if (_image == null)
                {
                    _image = GetComponent<Image>();
                }

                return _image;
            }
        }

        private void Awake()
        {
            sprite = image.sprite;
        }

        public void SetBaseColors(Color setColor, Color invalid, Color unavailable)
        {
            baseColor = setColor;
            invalidColor = invalid;
        }

        public void SetTexture(Texture texture)
        {
            var texture2d = texture as Texture2D;
            if (texture2d == null)
            {
                Debug.LogWarningFormat("UIImageButton only works with Texture2D textures, cannot use {0}", texture); ;
                return;
            }

            if (lastTexture != texture)
            {
                sprite = Sprite.Create(texture2d, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
                lastTexture = texture;
                if (lastIsFacingHmd) Refresh(true);
            }
        }

        private void Update()
        {
            var isFacingHmd = Utils.IsFacingHmd(transform);
            if (lastIsFacingHmd != isFacingHmd)
            {
                Refresh(isFacingHmd);
            }
        }

        private void Refresh(bool isFacingHmd)
        {
            image.sprite = isFacingHmd ? sprite : backface;
            lastIsFacingHmd = isFacingHmd;
            OnDemandRenderer.SafeDirty(gameObject);
        }

        public void Highlight()
        {
            activeColor = highlightColor;
        }

        public void UnHighlight()
        {
            activeColor = baseColor;
        }

        public void SetHighlightColor(Color color)
        {
            highlightColor = color;
        }
    }
}
