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
    class Menu
    {
        private Texture2D backgroundTexture, slider1, sliderBox1, slider2, sliderBox2, slider3, sliderBox3, slider4, sliderBox4;
        int screenWidth, screenHeight;        

        Vector2[] sliders = new Vector2[4];
        Vector2[] rectangles = new Vector2[4];
        Vector2 fontPosition;

        Rectangle[] sliderRectangle = new Rectangle[4];
        Rectangle[] textRectangle = new Rectangle[4];
        Rectangle screenRectangle;
        Rectangle mouseRectangle;

        public string textString;
        public string[] textMenu;
        public string[] textMenu2;
        private SpriteFont spriteFont;        

        private MouseState mouseStateCurrent, mouseStatePrevious, mouseState;
        private KeyboardState keyState;

        private Color backgroundColor;

        static Random randomize = new Random();

        Vector2 position;
        Color[] data, data2, data3, data4, data5, data6, data7, data8;

        public bool active;

        public float Damping; //How much energy should waves have? (must be 0-1)
        public float WaveFactor; //Min: -0.529f, Max: 0.529f.
        public Vector2 GridOffset;

        int bloomSetting = 0;
        BloomSettings bloomSettings;

        BloomComponent bloomComponent;
        
        public Menu(ContentManager Content, GraphicsDevice device)
        {
            MenuBackground(device);
            Update(device);
            Sliders(device);            

            spriteFont = Content.Load<SpriteFont>("Font");

            textMenu = new string[] { "Wave Factor", "Damping", "Grid Offset X", "Grid Offset Y", "Bloom Effect:" };
            //textMenu = new string[] { "Wave Factor", "Damping", "Grid Offset X", "Grid Offset Y" };
            textString = "MENU";

            position = new Vector2(screenWidth / 3, screenHeight / 4);

            int offset = 0;

            for (int i = 0; i < 4; i++)
            {
                sliders[i] = new Vector2(position.X + 20, position.Y + offset);

                offset += 50;
            }

            //Create Top Slider
            sliderBox1 = new Texture2D(device, 100, 15);
            data = new Color[100 * 15];
            for (int j = 0; j < data.Length; ++j) data[j] = Color.DarkGray;
            sliderBox1.SetData(data);

            //Creates Top DragBox

            slider1 = new Texture2D(device, 10, 15);
            data2 = new Color[10 * 15];
            for (int j = 0; j < data2.Length; ++j) data2[j] = Color.Black;
            slider1.SetData(data2);

            //Creates Bottom Slider
            sliderBox2 = new Texture2D(device, 100, 15);
            data3 = new Color[100 * 15];
            for (int j = 0; j < data3.Length; ++j) data3[j] = Color.DarkGray;
            sliderBox2.SetData(data3);

            //Creates Bottom DragBox
            slider2 = new Texture2D(device, 10, 15);
            data4 = new Color[10 * 15];
            for (int j = 0; j < data4.Length; ++j) data4[j] = Color.Black;
            slider2.SetData(data4);

            //Creates Bottom Slider
            sliderBox3 = new Texture2D(device, 100, 15);
            data5 = new Color[100 * 15];
            for (int j = 0; j < data5.Length; ++j) data5[j] = Color.DarkGray;
            sliderBox3.SetData(data5);

            //Creates Bottom DragBox
            slider3 = new Texture2D(device, 10, 15);
            data6 = new Color[10 * 15];
            for (int j = 0; j < data6.Length; ++j) data6[j] = Color.Black;
            slider3.SetData(data6);

            //Creates Bottom Slider
            sliderBox4 = new Texture2D(device, 100, 15);
            data7 = new Color[100 * 15];
            for (int j = 0; j < data7.Length; ++j) data7[j] = Color.DarkGray;
            sliderBox4.SetData(data7);

            //Creates Bottom DragBox
            slider4 = new Texture2D(device, 10, 15);
            data8 = new Color[10 * 15];
            for (int j = 0; j < data8.Length; ++j) data8[j] = Color.Black;
            slider4.SetData(data8);
        }    

        private static Color GetRandomColor()
        {

            byte ColorR = (byte)randomize.Next(100, 255);

            byte ColorG = (byte)randomize.Next(100, 255);

            byte ColorB = (byte)randomize.Next(100, 255);

            return new Color(ColorR, ColorG, ColorB);
        }

        public void Update(GraphicsDevice device)
        {
            mouseState = Mouse.GetState();
            mouseStateCurrent = Mouse.GetState();
            keyState = Keyboard.GetState();            
            mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            
            bloomSettings = BloomSettings.PresetSettings[bloomSetting];
            
            textMenu2 = new string[] { "< " + bloomSettings.BloomName + " >" };
 
            Sliders(device);
            
            if (mouseRectangle.Intersects(screenRectangle))
            {
                screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                fontPosition = new Vector2(screenWidth - 18, screenHeight / 2);
                active = true;
            }
            else
            {
                screenRectangle = new Rectangle(-screenWidth + 20, 0, screenWidth, screenHeight);
                fontPosition = new Vector2(screenWidth / 6 - 43, screenHeight / 2);
                active = false;
            }

            if (mouseRectangle.Intersects(sliderRectangle[0]) && active == true)
            {
                if (mouseStateCurrent.LeftButton == ButtonState.Pressed)
                {
                    sliderRectangle[0].X = mouseRectangle.X;
                    sliders[0].X = sliderRectangle[0].X;
                }
            }

            if (mouseRectangle.Intersects(sliderRectangle[1]) && active == true)
            {                
                if (mouseStateCurrent.LeftButton == ButtonState.Pressed)
                {
                    sliderRectangle[1].X = mouseRectangle.X;
                    sliders[1].X = sliderRectangle[1].X;                  
                }
            }

            if (mouseRectangle.Intersects(sliderRectangle[2]) && active == true)
            {
                if (mouseStateCurrent.LeftButton == ButtonState.Pressed)
                {
                    sliderRectangle[2].X = mouseRectangle.X;
                    sliders[2].X = sliderRectangle[2].X;
                }
            }

            if (mouseRectangle.Intersects(sliderRectangle[3]) && active == true)
            {
                if (mouseStateCurrent.LeftButton == ButtonState.Pressed)
                {
                    sliderRectangle[3].X = mouseRectangle.X;
                    sliders[3].X = sliderRectangle[3].X;
                }
            }
            
            if (mouseRectangle.Intersects(textRectangle[0]) && active == true)
            {
                if (MouseLeftClick())
                {
                    bloomSetting -= 1;

                    if (bloomSetting < 0)
                        bloomSetting = 5;

                    bloomSettings = BloomSettings.PresetSettings[bloomSetting];
                }
            }
            if (mouseRectangle.Intersects(textRectangle[1]) && active == true)
            {
                if (MouseLeftClick())
                {
                    bloomSetting += 1;

                    if (bloomSetting > 5)
                        bloomSetting = 0;

                    bloomSettings = BloomSettings.PresetSettings[bloomSetting];
                }
            }

            WaveFactor = (sliders[0].X - rectangles[0].X) * .00529f;
            Damping = (sliders[1].X - rectangles[1].X) * .0091f;

            GridOffset = new Vector2((sliders[2].X - rectangles[2].X) / .1f, (sliders[3].X - rectangles[3].X) / .1f);

            mouseStatePrevious = mouseStateCurrent;
        }

        private void MenuBackground(GraphicsDevice device)
        {
            screenWidth = device.Viewport.Width / 3;
            screenHeight = device.Viewport.Height;
            backgroundColor = GetRandomColor();

            Color[] backgroundColors = new Color[screenWidth * screenHeight];

            for (int x = 0; x < screenWidth; x++)
            {
                for (int y = 0; y < screenHeight; y++)
                {
                    backgroundColors[x + y * screenWidth] = backgroundColor * 0.5f;
                }
            }

            for (int x = screenWidth-20; x < screenWidth; x++)
            {
                for (int y = 0; y < screenHeight; y++)
                {
                    backgroundColors[x + y * screenWidth] = Color.Black;                    
                }
            }
            backgroundTexture = new Texture2D(device, screenWidth, screenHeight, false, SurfaceFormat.Color);
            backgroundTexture.SetData(backgroundColors);
        }

        public void Sliders(GraphicsDevice device)
        {
            int offset = 0;
            int offset2 = 0;

            for (int i = 0; i < 4; i++)
            {
                rectangles[i] = new Vector2(position.X-25, position.Y + offset);
                sliderRectangle[i] = new Rectangle((int)position.X-25, (int)position.Y + offset, 100, 15);
                offset += 50;

                sliders[i].Y = rectangles[i].Y;
            }
            for (int i = 0; i < 4; i++)
            {
                textRectangle[i] = new Rectangle((int)position.X - 140 + offset2, (int)position.Y + offset, 150, 25);
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

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice device)
        {
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);

            //device.Clear(GetRandomColor());

            DrawSliders(spriteBatch);

            DrawMenuStrings(spriteBatch, 3, position);

            spriteBatch.DrawString(spriteFont, textString, fontPosition, Color.White, -1.57079633f, Vector2.Zero, .6f, SpriteEffects.None, 0);
        }

        private void DrawMenuStrings(SpriteBatch spriteBatch, int alignment, Vector2 pos)
        {
            if (active == true)
            {
                for (int i = 0; i < textMenu.Length; i++)
                {
                    spriteBatch.DrawString(
                    spriteFont,
                    textMenu[i],
                    new Vector2(pos.X - ((spriteFont.MeasureString(textMenu[i]).X / alignment))+5, (position.Y + (50 * i)) - 35),
                    Color.Black);
                }
                
                for (int i = 0; i < textMenu2.Length; i++)
                {                    
                    spriteBatch.DrawString(
                    spriteFont,
                    textMenu2[i],
                    new Vector2(pos.X - ((spriteFont.MeasureString(textMenu2[i]).X / alignment)), (position.Y + (50 * i)) +200),
                    Color.White);
                }
            }
        }

        private void DrawSliders(SpriteBatch spriteBatch)
        {
            if (active == true)
            {
                spriteBatch.Draw(sliderBox1, rectangles[0], Color.White);
                spriteBatch.Draw(slider1, sliders[0], Color.White);

                spriteBatch.Draw(sliderBox2, rectangles[1], Color.White);
                spriteBatch.Draw(slider2, sliders[1], Color.White);

                spriteBatch.Draw(sliderBox3, rectangles[2], Color.White);
                spriteBatch.Draw(slider3, sliders[2], Color.White);

                spriteBatch.Draw(sliderBox4, rectangles[3], Color.White);
                spriteBatch.Draw(slider4, sliders[3], Color.White);
            }
        }
    }
}
