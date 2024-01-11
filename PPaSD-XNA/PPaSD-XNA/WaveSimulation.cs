#define MONOCHROME
//#undef MONOCHROME

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PPaSD_XNA
{
    class WaveSimulation
    {
        Vector2 gridDimensions;
        Vector2 gridStride;
        Vector2 gridOffset;

        float frequencyFactor;
        float sampleFactor;
        float damping;
        float waveFactor;

        WaveSettings waveSettings;
        Menu menu;

        float[,] gridHeightsFrontBuffer, gridHeightsBackBuffer;
        float averageHeight;

        public WaveSimulation(WaveSettings waveSettings, ContentManager Content, GraphicsDevice device)
        {
            this.waveSettings = waveSettings;
            menu = new Menu(Content, device);

            waveSettings.ApplyWaveValues();
            UpdateSettings();

            gridHeightsFrontBuffer = new float[(int)gridDimensions.X, (int)gridDimensions.Y];
            gridHeightsBackBuffer = new float[(int)gridDimensions.X, (int)gridDimensions.Y];

            //menu = new Menu(Content, device);
        }

        public void UpdateSettings()
        {
            this.gridDimensions = waveSettings.GridDimensions;
            this.gridStride = waveSettings.GridStride;
            this.gridOffset = menu.GridOffset;
            this.frequencyFactor = waveSettings.FrequencyFactor;
            this.sampleFactor = waveSettings.SampleFactor;
            this.damping = menu.Damping;
            this.waveFactor = menu.WaveFactor;
            //this.damping = waveSettings.Damping;
            //this.waveFactor = waveSettings.WaveFactor;
        }

        public void Update(VisualizationData visualizer, GraphicsDevice device)
        {
            waveSettings.Update();
            UpdateSettings();
            menu.Update(device);

            if (gridDimensions.X * gridDimensions.Y != gridHeightsFrontBuffer.Length)
            {
                gridHeightsFrontBuffer = new float[(int)gridDimensions.X, (int)gridDimensions.Y];
                gridHeightsBackBuffer = new float[(int)gridDimensions.X, (int)gridDimensions.Y];
            }

            averageHeight = 0;

            for (int x = 1; x < (int)gridDimensions.X - 1; x++)
            {
                for (int z = 1; z < (int)gridDimensions.Y - 1; z++)
                {
                    //If the cube should be visualized, override it's position
                    if ((x - gridOffset.X) % gridStride.X == 0 && (z - gridOffset.Y) % gridStride.Y == 0)
                    {
                        gridHeightsFrontBuffer[x, z] = (
                            (
                                (visualizer.Frequencies[x * (int)(visualizer.Frequencies.Count / gridDimensions.X)] * frequencyFactor) +
                                ((float)Math.Asin(visualizer.Samples[z * (int)(visualizer.Samples.Count / gridDimensions.Y)]) * sampleFactor)
                            ) / 2
                        );
                    }
                    else
                    {
                        //Calculate new height based on surrounding cubes
                        gridHeightsFrontBuffer[x, z] =
                           ((gridHeightsBackBuffer[x - 1, z] +
                            gridHeightsBackBuffer[x + 1, z] +
                            gridHeightsBackBuffer[x, z + 1] +
                            gridHeightsBackBuffer[x, z - 1]) * waveFactor) - gridHeightsFrontBuffer[x, z];
                    }

                    //Reduce the new height by the damping factor
                    gridHeightsFrontBuffer[x, z] = gridHeightsFrontBuffer[x, z] * damping;

                    averageHeight += gridHeightsFrontBuffer[x, z];
                }
            }

            averageHeight /= gridHeightsFrontBuffer.Length;

            //Swap Buffers
            float[,] tempBuffer = gridHeightsFrontBuffer;
            gridHeightsFrontBuffer = gridHeightsBackBuffer;
            gridHeightsBackBuffer = tempBuffer;
        }

        public Vector2 Dimensions()
        {
            return gridDimensions;
        }

        public float[,] Heights()
        {
            return gridHeightsFrontBuffer;
        }

        public float AverageHeight()
        {
            return averageHeight;
        }
    }
}
