//#define MONOCHROME
#undef MONOCHROME

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
    class BackgroundVisualizer
    {
        GraphicsDevice graphics;

        Texture2D texture;
        Color[] colorData;
        private bool toggleBackground = true;
        KeyboardState keyState, prevKeyState;

        public BackgroundVisualizer(GraphicsDevice graphics, VisualizationData visualizer)
        {
            this.graphics = graphics;
            texture = new Texture2D(graphics, visualizer.Frequencies.Count, visualizer.Samples.Count);
            colorData = new Color[texture.Width * texture.Height];
        }

        public void Update(VisualizationData visualizer)
        {
            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.LeftControl) && !prevKeyState.IsKeyDown(Keys.LeftControl))
            {
                toggleBackground = !toggleBackground;
            }

            if (toggleBackground)
            {
                for (int x = 0; x < visualizer.Frequencies.Count; x++)
                {
                    for (int y = 0; y < visualizer.Samples.Count; y++)
                    {
#if !MONOCHROME
                    colorData[(x * visualizer.Frequencies.Count) + y] =
                        new Color(visualizer.Frequencies[x],
                            (float)Math.Asin(visualizer.Samples[y]),
                            visualizer.Frequencies[x] + (float)Math.Asin(visualizer.Samples[y]));
#else
                        float colour = visualizer.Frequencies[x] + (float)Math.Asin(visualizer.Samples[y]);
                        colour *= 0.4f; //Dialin' it back, LIKE A SIR.
                        colorData[(x * visualizer.Frequencies.Count) + y] = new Color(colour, colour, colour);
#endif
                    }
                }

                texture.SetData<Color>(colorData);
            }

            prevKeyState = keyState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(toggleBackground)
                spriteBatch.Draw(texture, graphics.Viewport.Bounds, Color.White);
        }
    }
}
