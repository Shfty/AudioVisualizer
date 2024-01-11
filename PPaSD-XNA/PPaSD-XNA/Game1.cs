#define FULLSCREEN
//#undef FULLSCREEN

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PPaSD_XNA
{
    public partial class Game1 : Microsoft.Xna.Framework.Game
    {
        // CONFIGURATION
        Vector3 cameraOffset = new Vector3(0, 30, 70);
        Vector3 cameraRotation = Vector3.Zero;
        // END CONFIGURATION

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BloomComponent bloom;

        VisualizationData visualizer;

        BackgroundVisualizer background;
        MediaPlayerControl mediaControl;

        Cubes cubes;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

#if FULLSCREEN
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
#else
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
            graphics.IsFullScreen = false;
#endif

            graphics.PreferMultiSampling = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            visualizer = new VisualizationData();

            background = new BackgroundVisualizer(GraphicsDevice, visualizer);
            mediaControl = new MediaPlayerControl(Content);

            MediaPlayer.IsVisualizationEnabled = true;
            MediaPlayer.Volume = 0.5f;

            cubes = new Cubes(GraphicsDevice, Content);

            bloom = new BloomComponent(this, GraphicsDevice);
            Components.Add(bloom);

            bloom.Load();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (MediaPlayer.GameHasControl)
            {
                mediaControl.Update();
            }

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                MediaPlayer.Stop();
                this.Exit();
            }

            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.GetVisualizationData(visualizer);

                Console.WriteLine(gameTime.TotalGameTime.Milliseconds % 2);

                if (gameTime.TotalGameTime.Milliseconds % 2 == 1)
                {
                    background.Update(visualizer);
                }
                cubes.Update(visualizer);
            }
            //Rotate Camera
            cameraRotation.X += 0.001f;

            //Post-Processing
            bloom.Visible = true;
            bloom.Update();

            base.Update(gameTime);
        }

        float averageSample(int startIdx, int endIdx, VisualizationData visualizer)
        {
            float average = 0.0f;
            int range = endIdx - startIdx;

            for (int i = 0; i < range; i++)
            {
                average += visualizer.Samples[startIdx + i];
            }

            average /= range;

            return average;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            bloom.BeginDraw();

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            background.Draw(spriteBatch);
            mediaControl.Draw(spriteBatch);
            spriteBatch.End();

            Matrix View = Matrix.CreateLookAt(
                Vector3.Transform(
                    cameraOffset,
                    Quaternion.CreateFromYawPitchRoll(cameraRotation.X, cameraRotation.Y, cameraRotation.Z)
                ),
                Vector3.Zero,
                Vector3.Up);

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 1000);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            cubes.Draw(View, Projection);

            base.Draw(gameTime);
        }
    }
}
