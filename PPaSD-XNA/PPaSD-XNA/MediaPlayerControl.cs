using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace PPaSD_XNA
{
    class MediaPlayerControl
    {
        //Callbacks
        public delegate void onTrackChanged(bool next);

        //Variables
        private KeyboardState keyState, prevKeyState;
        private Random rand;
        private SpriteFont font;
        private int seed;
        private MouseState mouseStateCurrent, mouseStatePrevious, mouseState;
        private Texture2D play1, pause1, nsong, bsong, shuffle1, soundUp, soundDown, soundUpB, play1B, pause1B, nsongB, bsongB, soundDownB;
        private AudioStreamer audioStream;
        private Vector2 shuffle1Position, play1Position, nsongPosition, bsongPosition, soundUpPosition, soundDownPosition;
        onTrackChanged trackChangedCallback;

        //variables set when pausing & shuffling.
        private bool paused = false;
        private bool shuffle = false;
        private bool sub = false;
        private bool sdb = false;
        private bool plb = false;
        private bool bsb = false;
        private bool nsb = false;
        private string currentSong;

        public MediaPlayerControl(ContentManager Content, onTrackChanged callback)
        {
            //Loading in default resources
            play1 = Content.Load<Texture2D>("Textures//play1");
            play1B = Content.Load<Texture2D>("Textures//play1B");
            pause1 = Content.Load<Texture2D>("Textures//pause");
            pause1B = Content.Load<Texture2D>("Textures//pauseB");
            nsong = Content.Load<Texture2D>("Textures//next2");
            nsongB = Content.Load<Texture2D>("Textures//next2B");
            bsong = Content.Load<Texture2D>("Textures//next");
            bsongB = Content.Load<Texture2D>("Textures//nextB");
            shuffle1 = Content.Load<Texture2D>("Textures//shuffle");
            soundDown = Content.Load<Texture2D>("Textures//soundDown");
            soundDownB = Content.Load<Texture2D>("Textures//soundDownB");
            soundUp = Content.Load<Texture2D>("Textures//soundUp");
            soundUpB = Content.Load<Texture2D>("Textures//soundUpB");
            font = Content.Load<SpriteFont>("Font");

            trackChangedCallback = callback;

            int seed = DateTime.Now.Second;
            rand = new Random(seed);

            audioStream = new AudioStreamer(Content);
            audioStream.LoadContent();

            GetTrackInfo();
            //MediaPlayer.Play(audioStream.SelectSong());
        }

        public void Update(GraphicsDevice device)
        {
            keyState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            mouseStateCurrent = Mouse.GetState();

            shuffle1Position = new Vector2((float)device.Viewport.Width - ((float)device.Viewport.Width * 0.43f), (float)device.Viewport.Height - ((float)device.Viewport.Height * 0.15f));
            play1Position = new Vector2((float)device.Viewport.Width - ((float)device.Viewport.Width * 0.55f), (float)device.Viewport.Height - ((float)device.Viewport.Height * 0.15f));
            nsongPosition = new Vector2(play1Position.X + 60, play1Position.Y);
            bsongPosition = new Vector2(play1Position.X - 60, play1Position.Y);
            soundUpPosition = new Vector2(play1Position.X + 120, play1Position.Y);
            soundDownPosition = new Vector2(play1Position.X - 120, play1Position.Y);

            GetTrackInfo();

            //For continual playing, when song ends chooses next.
            //If on shuffle chooses a random index from the media library
            if (MediaPlayer.State != MediaState.Playing && !paused)
            {
                if (shuffle)
                {
                    seed *= 11;
                    rand = new Random(seed);
                    MediaPlayer.Stop();

                    audioStream.songIdx = rand.Next(0, audioStream.mp3Paths.Length);
                    MediaPlayer.Play(audioStream.SelectSong());
                }

                if (!shuffle)
                {
                    MediaPlayer.Stop();

                    audioStream.songIdx += 1;
                    MediaPlayer.Play(audioStream.SelectSong());
                }
            }

            //Only allows navigation when playing from library.
            //Only one default track, so no need to navigate.

            SkipNext();
            SkipPrevious();
            //ToggleShuffle();
            VolumeControls();

            PlayPause();

            prevKeyState = keyState;
            mouseStatePrevious = mouseStateCurrent;
        }

        //Skip next, stops the player, increasing the index in library by one and plays.
        private void SkipNext()
        {
            if ((mouseState.X < nsongPosition.X + 60 && mouseState.X > nsongPosition.X) && (mouseState.Y > nsongPosition.Y && mouseState.Y < nsongPosition.Y + 60)) //identify mouse over x y posotions for the button
            {
                nsb = true;
                if (MouseLeftClick())
                {
                    SkipLogic(1);
                    trackChangedCallback(true);
                }
            }
            else
            {
                nsb = false;
            }

        }

        //Skip previous, stops the player, decreasing the index in library by one and plays.
        private void SkipPrevious()
        {
            if ((mouseState.X < bsongPosition.X + 60 && mouseState.X > bsongPosition.X) && (mouseState.Y > bsongPosition.Y && mouseState.Y < bsongPosition.Y + 60))//identify mouse over x y posotions for the button
            {
                bsb = true;
                if (MouseLeftClick())
                {
                    SkipLogic(-1);
                    trackChangedCallback(false);
                }
            }
            else
            {
                bsb = false;
            }
        }

        private void SkipLogic(int idxOffset)
        {
            MediaPlayer.Stop();
            audioStream.songIdx += idxOffset;
            MediaPlayer.Play(audioStream.SelectSong());
            paused = false;
        }

        //Checks vs bool paused, sets to the opposite. Play -> Paused, Paused -> Play
        private void PlayPause()
        {
            if ((mouseState.X < play1Position.X + 60 && mouseState.X > play1Position.X) && (mouseState.Y > play1Position.Y && mouseState.Y < play1Position.Y + 60))//identify mouse over x y posotions for the button
            {
                plb = true;
                if (MouseLeftClick())
                {
                    PauseLogic();
                }
            }
            else
            {
                plb = false;
            }
        }

        private void PauseLogic()
        {
            paused = !paused;

            if (paused)
                MediaPlayer.Pause();

            if (!paused)
                MediaPlayer.Resume();
        }

        //Toggles shuffle mode, chooses random index rather than next song.
        //private void ToggleShuffle()
        //{
        //    if ((mouseState.X < 1028 && mouseState.X > 970) && (mouseState.Y > 788 && mouseState.Y < 858))//identify mouse over x y posotions for the button
        //    {
        //        if (MouseLeftClick())
        //        {
        //            ShuffleLogic();
        //        }
        //    }
        //}

        private void VolumeControls()
        {
            if (keyState.IsKeyDown(Keys.O) && !prevKeyState.IsKeyDown(Keys.O))
            {
                MediaPlayer.Volume += 0.1f;
            }
            if ((mouseState.X < soundUpPosition.X + 60 && mouseState.X > soundUpPosition.X) && (mouseState.Y > soundUpPosition.Y && mouseState.Y < soundUpPosition.Y + 60))//identify mouse over x y posotions for the button
            {
                sub = true;
                if (MouseLeftClick())
                {
                    MediaPlayer.Volume += 0.1f;
                }
            }
            else
            {
                sub = false;
            }
            if (keyState.IsKeyDown(Keys.L) && !prevKeyState.IsKeyDown(Keys.L))
            {
                MediaPlayer.Volume -= 0.1f;
            }
            if ((mouseState.X < soundDownPosition.X + 60 && mouseState.X > soundDownPosition.X) && (mouseState.Y > soundDownPosition.Y && mouseState.Y < soundDownPosition.Y + 60))//identify mouse over x y posotions for the button
            {
                sdb = true;
                if (MouseLeftClick())
                {
                    MediaPlayer.Volume -= 0.1f;
                }
            }
            else
            {
                sdb = false;
            }

            MathHelper.Clamp(MediaPlayer.Volume, 0, 1);
        }

        private void ShuffleLogic()
        {
            //shuffle = !shuffle;

            //if (shuffle)
            //{
            //    shuffleIdx = new int[audioStream.mp3Paths.Length - 1];
            //    shuffleIdx[0] = audioStream.songIdx;
            //}

            //else
            //{ shuffleIdx = null; }
        }

        private void GetTrackInfo()
        {
            currentSong = audioStream.songName;
        }

        public bool MouseLeftClick()
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

        //Draws out track information & shuffle info onto screen.
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice device)
        {
            if (currentSong == null)
                currentSong = "";

            spriteBatch.DrawString(font, "Now Playing: " + currentSong, new Vector2(20, 20), Color.White);
            
            if (!paused)
            {
                if (plb == true)
                {
                    spriteBatch.Draw(pause1B, play1Position, Color.White);
                }
                else if (plb == false)
                {
                    spriteBatch.Draw(pause1, play1Position, Color.White);
                }
            }
            else if (paused)
            {
                if (plb == true)
                {
                    spriteBatch.Draw(play1B, play1Position, Color.White);
                }
                else if (plb == false)
                {
                    spriteBatch.Draw(play1, play1Position, Color.White);
                }
            }

           // spriteBatch.Draw(shuffle1, new Vector2(970.0f, 800.0f), Color.White);
            if (nsb == true)
            {
                spriteBatch.Draw(nsongB, nsongPosition, Color.White);
            }
            else if (nsb == false)
            {
                spriteBatch.Draw(nsong, nsongPosition, Color.White);
            }

            if (bsb == true)
            {
                spriteBatch.Draw(bsongB, bsongPosition, Color.White);
            }
            else if (bsb == false)
            {
                spriteBatch.Draw(bsong, bsongPosition, Color.White);
            }

            if (sub == true)
            {
                spriteBatch.Draw(soundUpB, soundUpPosition, Color.White);
            }
            else if (sub == false)
            {
                spriteBatch.Draw(soundUp, soundUpPosition, Color.White);
            }
            
            if (sub == true)
            {
                spriteBatch.Draw(soundUpB, soundUpPosition, Color.White);
            }
            else if (sub == false)
            {
                spriteBatch.Draw(soundUp, soundUpPosition, Color.White);
            }

            if (sdb == true)
            {
                spriteBatch.Draw(soundDownB, soundDownPosition, Color.White);
            }
            else if (sdb == false)
            {
                spriteBatch.Draw(soundDown, soundDownPosition, Color.White);
            }
        }
    }  
}
