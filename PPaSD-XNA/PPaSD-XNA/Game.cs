//#define FULLSCREEN
#undef FULLSCREEN

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
    public partial class Game : Microsoft.Xna.Framework.Game
    {
        // CONFIGURATION
        Vector3 cameraOffset = new Vector3(0, 20, 120);
        Quaternion cameraRotation = Quaternion.Identity;
        // END CONFIGURATION

        //States and epilepsy warning
        private enum GameState
        {
            Loading = 0,
            Visualiser
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BloomComponent bloom;

        VisualizationData visualizer;

        BackgroundVisualizer background;
        MediaPlayerControl mediaControl;
        Menu menu;

        Animator cameraAnimator;

        private bool loading;
        private Color loadingColor = new Color(255, 255, 255, 255);
        private SpriteFont loadingFont;
        private GameState states = new GameState();
        private int timeElapsed;
        private string warning = "Epilepsy Warning: Press Esc to quit";

        Camera camera;
        Cubes cubes;

        public Game()
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
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            visualizer = new VisualizationData();
            loadingFont = Content.Load<SpriteFont>("Font");

            background = new BackgroundVisualizer(GraphicsDevice, visualizer);
            mediaControl = new MediaPlayerControl(Content, onTrackChanged);
            menu = new Menu(Content, GraphicsDevice);
            
            MediaPlayer.IsVisualizationEnabled = true;
            MediaPlayer.Volume = 0.5f;

            camera = new Camera(0, cameraOffset, Vector3.Zero);
            cameraAnimator = new Animator(0f, 1f, 1000, Animator.EasingCurve.LINEAR, false, onCameraAnimationUpdate, onCameraAnimationFinish);
            onCameraAnimationFinish(); //Setup default looping rotation animation

            cubes = new Cubes(GraphicsDevice, Content);

            bloom = new BloomComponent(this, GraphicsDevice);
            Components.Add(bloom);

            //Debug
            states = GameState.Visualiser;

            bloom.Load();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            switch (states)
            {
                case GameState.Loading:

                    timeElapsed++;
                    loading = true;

                    if (timeElapsed > 169)
                    {
                        loadingColor.R -= 2;
                        loadingColor.G -= 2;
                        loadingColor.B -= 2;

                        MathHelper.Clamp(loadingColor.R, 0, 255);
                        MathHelper.Clamp(loadingColor.G, 0, 255);
                        MathHelper.Clamp(loadingColor.B, 0, 255);
                    }

                    if (timeElapsed > 295)
                    {
                        states = GameState.Visualiser;
                    }

                    break;

                case GameState.Visualiser:
                    loading = false;                    

                    if (MediaPlayer.GameHasControl)
                    {
                        mediaControl.Update(GraphicsDevice);
                    }

                    if (MediaPlayer.State == MediaState.Playing)
                    {
                        MediaPlayer.GetVisualizationData(visualizer);

                        if (gameTime.TotalGameTime.Milliseconds % 4 == 1)
                        {
                            background.Update(visualizer);
                        }
                        cubes.Update(visualizer, GraphicsDevice);
                    }

                    camera.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, 0.001f);
                    cameraAnimator.Update();

                    //Post-Processing
                    bloom.Visible = true;
                    bloom.Update();
                    menu.Update(GraphicsDevice); 
                    break;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || (Keyboard.GetState().IsKeyDown(Keys.C) && Keyboard.GetState().IsKeyDown(Keys.J)))
            {
                MediaPlayer.Stop();
                this.Exit();
            }

            base.Update(gameTime);
        }

        void onCameraAnimationUpdate(float value)
        {
            camera.Position = new Vector3((float)Math.Sin(value * Math.PI * 2) * cameraOffset.Z, cameraOffset.Y, (float)Math.Cos(value * Math.PI * 2) * cameraOffset.Z);

            Vector3 source = Vector3.Normalize(-camera.Position);
            Vector3 dest = Vector3.Forward;

            Vector3 newAxis = Vector3.Cross(dest, source);
            newAxis.Normalize();
            float newAngle = (float)Math.Acos(Vector3.Dot(dest, source));

            camera.Rotation = Quaternion.CreateFromAxisAngle(newAxis, newAngle);
        }

        void onCameraAnimationFinish()
        {
            cameraAnimator.Start = cameraAnimator.Value;
            cameraAnimator.End = cameraAnimator.Value + 1;
            cameraAnimator.Duration = 10000;
            cameraAnimator.Curve = Animator.EasingCurve.LINEAR;
            cameraAnimator.Loop = true;
            cameraAnimator.Play();
        }

        void onTrackChanged(bool next)
        {
            cameraAnimator.Start = cameraAnimator.Value;
            cameraAnimator.End = cameraAnimator.Value + (next ? 1 : -1);
            cameraAnimator.Duration = 2000;
            cameraAnimator.Curve = Animator.EasingCurve.CUBIC;
            cameraAnimator.Loop = false;
            cameraAnimator.Play();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            bloom.BeginDraw();
            GraphicsDevice.Clear(Color.Black);

            if (loading)
            {
                spriteBatch.Begin();
                Vector2 size = loadingFont.MeasureString(warning);
                spriteBatch.DrawString(loadingFont, warning,  new Vector2((GraphicsDevice.Viewport.Width / 2 - (size.X / 2)), 
                    GraphicsDevice.Viewport.Height / 2 - loadingFont.LineSpacing), loadingColor);
                spriteBatch.End();
            }

            if (!loading)
            {    
                spriteBatch.Begin();
                background.Draw(spriteBatch);
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
                cubes.Draw(camera.ViewMatrix(), Projection);                               

                spriteBatch.Begin();
                mediaControl.Draw(spriteBatch, GraphicsDevice);
                spriteBatch.End();
                
                spriteBatch.Begin();
                menu.Draw(spriteBatch, GraphicsDevice);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
