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
    class CubeWord : GameObject
    {
        public const int SPACE_SIZE = 1;

        public CubeLetter[] Letters { get; set; }

        char[] characters;
        Vector3 offset;

        public CubeWord(int id, Vector3 position, Quaternion rotation, string inString, Color[] colours) : base(id, position, rotation)
        {
            characters = inString.ToCharArray();

            float lettersWidth = CubeLetter.WIDTH * (characters.Length - 1);
            float spacesWidth = SPACE_SIZE * (characters.Length - 2);
            float halfWidth = (lettersWidth + spacesWidth) / 2;

            offset = new Vector3(-halfWidth, 0, 0);

            short colourIndex = 0;

            Letters = new CubeLetter[characters.Length];

            for(int i = 0; i < characters.Length; i++)
            {
                if (i > 0)
                {
                    offset.X += CubeLetter.WIDTH + SPACE_SIZE;
                }

                if(characters[i] == ' ') {
                    continue;
                }

                Letters[i] = new CubeLetter(Id, Position, Rotation, characters[i], colours[colourIndex % colours.Length], offset);
                colourIndex++;
            }
        }

        public void Update()
        {
            for (int i = 0; i < Letters.Length; i++)
            {
                if (Letters[i] != null)
                {
                    Letters[i].Position = Position;
                    Letters[i].Rotation = Rotation;
                    Letters[i].Scale = Scale;
                    Letters[i].Update();
                }
            }
        }
    }
}
