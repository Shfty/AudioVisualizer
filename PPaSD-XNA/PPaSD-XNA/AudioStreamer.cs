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
using System.Reflection;

namespace PPaSD_XNA
{
    class AudioStreamer
    {
        public int songIdx;
        private Random rand;
        private ContentManager Content;
        private Song[] defaultSongs;
        public string[] filePaths;
        public string[] mp3Paths, wavPaths, wmaPaths, flacPaths, m4aPaths, aacPaths;
        public string[] songNames;
        public string songName;
        public bool fromFile;
        public Song song;

        int i;

        public AudioStreamer(ContentManager Content)
        {
            this.Content = Content;
        }

        public void LoadContent()
        {
            try
            {
                string directory = ".\\Music";

                mp3Paths = GetDirectories(directory, "*.mp3");
                wavPaths = GetDirectories(directory, "*wav");
                wmaPaths = GetDirectories(directory, "*wma");
                flacPaths = GetDirectories(directory, "*flac");
                m4aPaths = GetDirectories(directory, "*m4a");
                aacPaths = GetDirectories(directory, "*aac");

                filePaths = new string[mp3Paths.Length + wavPaths.Length + wmaPaths.Length +
                    flacPaths.Length + m4aPaths.Length + aacPaths.Length];

                MergeFileNames(mp3Paths);
                MergeFileNames(wavPaths);
                MergeFileNames(wmaPaths);
                MergeFileNames(flacPaths);
                MergeFileNames(m4aPaths);
                MergeFileNames(aacPaths);

                songNames = new string[filePaths.Length];

                for (int i = 0; i < filePaths.Length; i++)
                {
                    songNames[i] = filePaths[i].Replace(".mp3", "");
                    songNames[i] = songNames[i].Replace(".wav", "");
                    songNames[i] = songNames[i].Replace(".wma", "");
                    songNames[i] = songNames[i].Replace(".flac", "");
                    songNames[i] = songNames[i].Replace(".m4a", "");
                    songNames[i] = songNames[i].Replace(".aac", "");
                    songNames[i] = songNames[i].Replace(directory + "\\", "");
                }

                rand = new Random(DateTime.Now.Second);
                songIdx = rand.Next(0, filePaths.Length - 1);
                fromFile = true;
            }
            catch
            {
                LoadDefault();
            }

            if (filePaths.Length == 0)
            {
                LoadDefault();
            }
        }

        private void LoadDefault()
        {
            fromFile = false;

            defaultSongs = new Song[4];
            
            defaultSongs[0] = Content.Load<Song>("Music\\Cut and Run");
            defaultSongs[1] = Content.Load<Song>("Music\\Discipline");
            defaultSongs[2] = Content.Load<Song>("Music\\Echoplex");
            defaultSongs[3] = Content.Load<Song>("Music\\Rose_Vale");

            songNames = new string[4];

            songNames[0] = "Cut and Run";
            songNames[1] = "Discipline";
            songNames[2] = "Echoplex";
            songNames[3] = "Rose Vale";

            rand = new Random(DateTime.Now.Second);
            songIdx = rand.Next(0, defaultSongs.Length - 1);
        }

        private string[] GetDirectories(string directory, string fileType)
        {
            string[] paths;

            try { paths = Directory.GetFiles(directory, fileType, SearchOption.AllDirectories); }
            catch { paths = new string[0]; }

            return paths;
        }

        private void MergeFileNames(string[] paths)
        {
            for (int j = 0; j < paths.Length; j++)
            {
                filePaths[i] = paths[j];
                i++;
            }
        }

        public Song SelectSong()
        {
            if (fromFile)
            {
                var constructor = typeof(Song).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new[] { typeof(string), typeof(string), typeof(int) }, null);

                if (songIdx > filePaths.Length - 1)
                    songIdx = 0;

                if (songIdx < 0)
                    songIdx = filePaths.Length - 1;

                song = (Song)constructor.Invoke(new object[] { songNames[songIdx], filePaths[songIdx], 0 });
                songName = songNames[songIdx];
            }

            if(!fromFile)
            {
                if (songIdx > defaultSongs.Length - 1)
                    songIdx = 0;

                if (songIdx < 0)
                    songIdx = defaultSongs.Length - 1;

                song = defaultSongs[songIdx];
                songName = songNames[songIdx];
            }

            return song;
        }
    }
}

