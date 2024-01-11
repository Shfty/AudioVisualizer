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
    class CubeLetter : GameObject
    {
        public const int WIDTH = 5;

        public Vector3 WorldOffset { get; set; }
        public Color Colour { get; set; }

        float localOffset = WIDTH / 2;

        int[] pixelData;
        InstanceDataVertex[] vertexData;

        public CubeLetter(int id, Vector3 position, Quaternion rotation, char letter, Color colour, Vector3 worldOffset)
            : base(id, position, rotation)
        {
            Colour = colour;
            WorldOffset = worldOffset;

            pixelData = CubeAlphabet.GetLetter(letter);

            vertexData = new InstanceDataVertex[pixelData.Length];

            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < WIDTH; y++)
                {
                    if (pixelData[x + (y * WIDTH)] == 1)
                    {
                        Vector3 offset = new Vector3((x - localOffset + WorldOffset.X), (-y + localOffset + WorldOffset.Y), WorldOffset.Z);

                        vertexData[x + (y * WIDTH)] =
                            new InstanceDataVertex(
                                Matrix.CreateTranslation(offset) *
                                Matrix.CreateFromQuaternion(Rotation) *
                                Matrix.CreateTranslation(Position) *
                                Matrix.CreateScale(Scale),
                                Colour
                            );
                    }
                }
            }
        }

        public void Update()
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < WIDTH; y++)
                {
                    if (pixelData[x + (y * WIDTH)] == 1)
                    {
                        Vector3 offset = new Vector3((x - localOffset + WorldOffset.X), (-y + localOffset + WorldOffset.Y), WorldOffset.Z);

                        vertexData[x + (y * WIDTH)].World = 
                                Matrix.CreateTranslation(offset) *
                                Matrix.CreateFromQuaternion(Rotation) *
                                Matrix.CreateTranslation(Position) *
                                Matrix.CreateScale(Scale);
                        vertexData[x + (y * WIDTH)].Colour = Colour;
                    }
                }
            }
        }

        public InstanceDataVertex[] GetData()
        {
            return vertexData;
        }
    }
}
