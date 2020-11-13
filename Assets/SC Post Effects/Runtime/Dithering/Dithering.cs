﻿#if URP
using UnityEngine.Rendering.Universal;
#endif

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
#if URP
    [Serializable, VolumeComponentMenu("SC Post Effects/Retro/Dithering")]
    public sealed class Dithering : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Note that the texture's filter mode (Point or Bilinear) greatly affects the behavior of the pattern")]
        public TextureParameter lut = new TextureParameter(null);

        [Range(0f, 1f), Tooltip("Fades the effect in or out")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        [Range(0f, 1f), Tooltip("The screen's luminance values control the density of the dithering matrix")]
        public ClampedFloatParameter luminanceThreshold = new ClampedFloatParameter(0.5f, 0f, 1f);

        //[Range(0f,2f), DisplayName("Tiling")]
        public ClampedFloatParameter tiling = new ClampedFloatParameter(1f, 0f, 2f);

        public bool IsActive() => intensity.value > 0f && this.active;

        public bool IsTileCompatible() => false;
#endif
    }
}