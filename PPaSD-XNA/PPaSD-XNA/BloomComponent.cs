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
    public class BloomComponent : DrawableGameComponent
    {

        SpriteBatch spriteBatch;
        Effect bloomExtractEffect;
        Effect bloomCombineEffect;
        Effect gaussianBlurEffect;
        RenderTarget2D sceneRenderTarget;
        RenderTarget2D renderTarget1;
        RenderTarget2D renderTarget2;
        GraphicsDevice device;

        private int bloomSetting = 0;

        KeyboardState keyState, prevKeyState;

        BloomSettings settings;

        Rectangle[] bloomRectangle = new Rectangle[4];
        int screenWidth, screenHeight;
        private MouseState mouseStateCurrent, mouseStatePrevious, mouseState;
        Rectangle mouseRectangle;
        
        public BloomSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public enum IntermediateBuffer
        {
            PreBloom,
            BlurredHorizontally,
            BlurredBothWays,
            FinalResult,
        }

        public IntermediateBuffer ShowBuffer
        {
            get { return showBuffer; }
            set { showBuffer = value; }
        }

        IntermediateBuffer showBuffer = IntermediateBuffer.FinalResult;

        public BloomComponent(Game game, GraphicsDevice device)
            : base(game)
        {
            if (game == null)
                throw new ArgumentNullException("game");

            this.device = device;            
        }

        public void Load()
        {
            BloomRectangle();
            settings = BloomSettings.PresetSettings[bloomSetting];

            bloomExtractEffect = Game.Content.Load<Effect>("BloomExtract");
            bloomCombineEffect = Game.Content.Load<Effect>("BloomCombine");
            gaussianBlurEffect = Game.Content.Load<Effect>("GaussianBlur");

            PresentationParameters pp = device.PresentationParameters;

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            SurfaceFormat format = pp.BackBufferFormat;


            sceneRenderTarget = new RenderTarget2D(device, width, height, false,
                                                   format, pp.DepthStencilFormat, pp.MultiSampleCount,
                                                   RenderTargetUsage.DiscardContents);

            width /= 2;
            height /= 2;

            renderTarget1 = new RenderTarget2D(device, width, height, false, format, DepthFormat.None);
            renderTarget2 = new RenderTarget2D(device, width, height, false, format, DepthFormat.None);
            
            screenWidth = device.Viewport.Width / 3;
            screenHeight = device.Viewport.Height / 4;
        }

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
            sceneRenderTarget.Dispose();
            renderTarget1.Dispose();
            renderTarget2.Dispose();
        }

        public void Update()
        {
            mouseState = Mouse.GetState();
            mouseStateCurrent = Mouse.GetState();
            keyState = Keyboard.GetState();
            mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            if(mouseRectangle.Intersects(bloomRectangle[0]))
            {
                if (MouseLeftClick())
                {  
                    bloomSetting -= 1;

                    if (bloomSetting < 0)
                        bloomSetting = 5;

                    settings = BloomSettings.PresetSettings[bloomSetting];
                }
            }

            if (mouseRectangle.Intersects(bloomRectangle[1]))
            {
                if (MouseLeftClick())
                {
                    bloomSetting += 1;

                    if (bloomSetting > 5)
                        bloomSetting = 0;

                    settings = BloomSettings.PresetSettings[bloomSetting];                    
                }                
            }

            prevKeyState = keyState;
            mouseStatePrevious = mouseStateCurrent;
        }

        public void BloomRectangle()
        {
            int offset2 = 0;

            for (int i = 0; i < 4; i++)
            {
                bloomRectangle[i] = new Rectangle(screenWidth - 52 + offset2, screenHeight + 312, 150, 25);
                offset2 += 180;
            }
        }

        private bool MouseLeftClick()
        {
            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton == ButtonState.Released)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void BeginDraw()
        {
            if (Visible)
            {
                device.SetRenderTarget(sceneRenderTarget);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch = new SpriteBatch(device);

            device.SamplerStates[1] = SamplerState.LinearClamp;

            bloomExtractEffect.Parameters["BloomThreshold"].SetValue(
                Settings.BloomThreshold);

            DrawFullscreenQuad(sceneRenderTarget, renderTarget1,
                               bloomExtractEffect,
                               IntermediateBuffer.PreBloom);

            SetBlurEffectParameters(1.0f / (float)renderTarget1.Width, 0);

            DrawFullscreenQuad(renderTarget1, renderTarget2,
                               gaussianBlurEffect,
                               IntermediateBuffer.BlurredHorizontally);

            SetBlurEffectParameters(0, 1.0f / (float)renderTarget1.Height);

            DrawFullscreenQuad(renderTarget2, renderTarget1,
                               gaussianBlurEffect,
                               IntermediateBuffer.BlurredBothWays);

            device.SetRenderTarget(null);

            EffectParameterCollection parameters = bloomCombineEffect.Parameters;

            parameters["BloomIntensity"].SetValue(Settings.BloomIntensity);
            parameters["BaseIntensity"].SetValue(Settings.BaseIntensity);
            parameters["BloomSaturation"].SetValue(Settings.BloomSaturation);
            parameters["BaseSaturation"].SetValue(Settings.BaseSaturation);

            device.Textures[1] = sceneRenderTarget;

            Viewport viewport = device.Viewport;

            DrawFullscreenQuad(renderTarget1,
                               viewport.Width, viewport.Height,
                               bloomCombineEffect,
                               IntermediateBuffer.FinalResult);
            
        }

        void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget,
                                Effect effect, IntermediateBuffer currentBuffer)
        {
            device.SetRenderTarget(renderTarget);

            DrawFullscreenQuad(texture,
                               renderTarget.Width, renderTarget.Height,
                               effect, currentBuffer);
        }

        void DrawFullscreenQuad(Texture2D texture, int width, int height,
                                Effect effect, IntermediateBuffer currentBuffer)
        {
            if (showBuffer < currentBuffer)
            {
                effect = null;
            }

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();
        }

        void SetBlurEffectParameters(float dx, float dy)
        {
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
            offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];

            int sampleCount = weightsParameter.Elements.Count;

            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            float totalWeights = sampleWeights[0];

            for (int i = 0; i < sampleCount / 2; i++)
            {
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        float ComputeGaussian(float n)
        {
            float theta = Settings.BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }        
    }
}
