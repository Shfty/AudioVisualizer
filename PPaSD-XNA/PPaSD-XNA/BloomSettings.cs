using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PPaSD_XNA
{
    public class BloomSettings
    {
        public readonly float BloomThreshold;
        public readonly float BlurAmount;
        public readonly float BloomIntensity;
        public readonly float BaseIntensity;
        public readonly float BloomSaturation;
        public readonly float BaseSaturation;
        public readonly string BloomName;


        public BloomSettings(float bloomThreshold, float blurAmount,
                             float bloomIntensity, float baseIntensity,
                             float bloomSaturation, float baseSaturation, string bloomName)
        {
            BloomThreshold = bloomThreshold;
            BlurAmount = blurAmount;
            BloomIntensity = bloomIntensity;
            BaseIntensity = baseIntensity;
            BloomSaturation = bloomSaturation;
            BaseSaturation = baseSaturation;
            BloomName = bloomName;
        }

        public static BloomSettings[] PresetSettings =
        {
            //Default
            new BloomSettings(0.25f, 4, 1.25f, 1, 1, 1, "Default"),
            //Vivid
            new BloomSettings(0,      3,   1,     2,    1,       1, "Vivid"),
            //Blur
            new BloomSettings(0,      2,   1,     0.1f, 1,       1, "Blurry"),
            //Smooth
            new BloomSettings(0.5f,   2,   1,     1,    1,       1, "Smooth"),
            //Low Saturation
            new BloomSettings(0.5f,   8,   2,     1,    0,       1, "Desaturated"),
            //High Saturation
            new BloomSettings(0.25f,  4,   2,     1,    2,       0, "Saturated"),
        };

    }
}