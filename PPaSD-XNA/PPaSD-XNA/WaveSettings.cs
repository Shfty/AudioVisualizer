using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PPaSD_XNA
{
    class WaveSettings
    {
        KeyboardState keyState, prevKeyState;

        // WAVE GRID
        public Vector2 GridDimensions = new Vector2(100, 100); //Total grid size (including border)
        public Vector2 GridStride; 
        public Vector2 GridOffset = new Vector2(10, 10); //How many blocks to skip before the first visualised (must be at least 1,1)
        public float strideOffset = 5; //How many blocks to skip before the next visualised (must be at least 1,1)

        //Note: Sample values have a much lower update rate than frequency.
        public float FrequencyFactor = 50f; //Multiply frequency values (x) by this
        public float SampleFactor = 100f; //Multiply sample values (y) by this
        public float Damping = 0.91f; //How much energy should waves have? (must be 0-1)

        public float WaveFactor = 0.5f; //Min: -0.529f, Max: 0.529f.
        /* Intrinsic wave algorithm multiplication factor. Makes crazy shit happen if you mess with it.
         * Known good values:
         * 0.5 - 'Normal'
         * 0.515 - Nice and Wavy
         * 0.52 - Still Nice and Wavy, but maybe a little much
         * 0.529 - WOULD YOU LIKE SOME SKYRIM? a.k.a GIANT MUSICAL APOCALYPSE-BRINGING DEATH CLAM
         * >= 0.53 - DO NOT USE: Resonance Cascade
         * 
         * Known bad values:
         * -0.5 - Bad trip. Lots of jittering.
         */
        // END WAVE GRID

        private bool random = true;
        private float[] randoms = new float[5];

        private Random rand = new Random(DateTime.Now.Second);
        private int frameCount, i , j;

        public WaveSettings()
        {
            randoms[0] = 20;
            randoms[1] = 8;
            randoms[2] = 10;
            randoms[3] = 6;
            randoms[4] = 25;
        }

        public void Update()
        {
            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.R) && !prevKeyState.IsKeyDown(Keys.R))
            {
                random = !random;
            }

            if (!random)
            {
                ApplyWaveValues();
                ClampValues();
            }
            else
            {
                frameCount++;

                if (frameCount > 240)
                {
                    ApplyWaveValues();
                    ClampValues();
                    frameCount = 0;
                }
            }

            prevKeyState = keyState;
        }

        public void ApplyWaveValues()
        {
            WaveFactorInteractive();
            DampingInteractive();
            VisualiserBlocks();
        }

        private void WaveFactorInteractive()
        {
            if (keyState.IsKeyDown(Keys.Up))
            {
                WaveFactor += 0.0005f;
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                WaveFactor -= 0.001f;
            }

            if (WaveFactor >= 0.5252f) WaveFactor = 0.5252f;
            if (WaveFactor <= 0.47f) WaveFactor = 0.47f;
        }

        private void DampingInteractive()
        {
            if (keyState.IsKeyDown(Keys.Space))
            {
                Damping += 0.001f;
            }

            if (keyState.IsKeyDown(Keys.X))
            {
                Damping -= 0.001f;
            }

            if (Damping >= 0.98f) Damping = 0.98f;
            if (Damping <= 0.8f) Damping = 0.08f;
        }

        private void VisualiserBlocks()
        {
            if (!random)
            {
                if (keyState.IsKeyDown(Keys.LeftAlt) && prevKeyState.IsKeyDown(Keys.LeftAlt))
                {
                    strideOffset += 1;
                }

                if (keyState.IsKeyDown(Keys.Z) && prevKeyState.IsKeyDown(Keys.Z))
                {
                    strideOffset -= 1;
                }

                MathHelper.Clamp(strideOffset, 1, 99);
                GridStride = new Vector2(strideOffset, strideOffset);
            }
            else
            {
                rand = new Random();

                i = rand.Next(0, 5);

                while (i == j)
                {
                    i = rand.Next(0, 5);
                }

                GridStride = new Vector2(randoms[i], randoms[i]);
                j = i;
            }
        }

        private void ClampValues()
        {
            if (WaveFactor >= 0.504f && Damping >= 0.9f)
            {
                Damping = 0.91f;
            }
        }
    }
}
