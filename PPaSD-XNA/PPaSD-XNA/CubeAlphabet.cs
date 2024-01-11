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
    static class CubeAlphabet
    {
        public const int SIZE_SQUARED = 25; //5x5

        public static int[] GetLetter(char letter)
        {
            switch (letter)
            {
                case 'a':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        1, 1, 1, 1, 1,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1
                    };
                case 'b':
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        1, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        1, 1, 1, 1, 0
                    };
                case 'c':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 1,
                        1, 0, 0, 0, 0,
                        1, 0, 0, 0, 0,
                        1, 0, 0, 0, 0,
                        0, 1, 1, 1, 1
                    };
                case 'd':
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        1, 1, 1, 1, 0
                    };
                case 'e':
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 1,
                        1, 0, 0, 0, 0,
                        1, 1, 1, 0, 0,
                        1, 0, 0, 0, 0,
                        1, 1, 1, 1, 1
                    };
                case 'f':
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 1,
                        1, 0, 0, 0, 0,
                        1, 1, 1, 0, 0,
                        1, 0, 0, 0, 0,
                        1, 0, 0, 0, 0
                    };
                case 'g':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 1,
                        1, 0, 0, 0, 0,
                        1, 0, 0, 1, 1,
                        1, 0, 0, 0, 1,
                        0, 1, 1, 1, 1
                    };
                case 'h':
                    return new int[SIZE_SQUARED] {
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        1, 1, 1, 1, 1,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1
                    };
                case 'i':
                    return new int[SIZE_SQUARED] {
                        0, 0, 1, 0, 0,
                        0, 0, 1, 0, 0,
                        0, 0, 1, 0, 0,
                        0, 0, 1, 0, 0,
                        0, 0, 1, 0, 0
                    };
                case 'j':
                    return new int[SIZE_SQUARED] {
                        0, 0, 0, 0, 1,
                        0, 0, 0, 0, 1,
                        0, 0, 0, 0, 1,
                        0, 0, 0, 0, 1,
                        1, 1, 1, 1, 0
                    };
                case 'k':
                    return new int[SIZE_SQUARED] {
                        1, 0, 0, 0, 1,
                        1, 0, 0, 1, 0,
                        1, 1, 1, 0, 0,
                        1, 0, 0, 1, 0,
                        1, 0, 0, 0, 1
                    };
                case 'l':
                    return new int[SIZE_SQUARED] {
                        1, 0, 0, 0, 0,
                        1, 0, 0, 0, 0,
                        1, 0, 0, 0, 0,
                        1, 0, 0, 0, 0,
                        1, 1, 1, 1, 1
                    };
                case 'm':
                    return new int[SIZE_SQUARED] {
                        1, 0, 0, 0, 1,
                        1, 1, 0, 1, 1,
                        1, 0, 1, 0, 1,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1
                    };
                case 'n':
                    return new int[SIZE_SQUARED] {
                        1, 0, 0, 0, 1,
                        1, 1, 0, 0, 1,
                        1, 0, 1, 0, 1,
                        1, 0, 0, 1, 1,
                        1, 0, 0, 0, 1
                    };
                case 'o':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        0, 1, 1, 1, 0
                    };
                case 'p':
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        1, 1, 1, 1, 0,
                        1, 0, 0, 0, 0,
                        1, 0, 0, 0, 0
                    };
                case 'q':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 1, 1,
                        0, 1, 1, 1, 1
                    };
                case 'r':
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        1, 1, 1, 1, 0,
                        1, 0, 0, 1, 0,
                        1, 0, 0, 0, 1
                    };
                case 's':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 1,
                        1, 0, 0, 0, 0,
                        0, 1, 1, 1, 0,
                        0, 0, 0, 0, 1,
                        1, 1, 1, 1, 0
                    };
                case 't':
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 1,
                        0, 0, 1, 0, 0,
                        0, 0, 1, 0, 0,
                        0, 0, 1, 0, 0,
                        0, 0, 1, 0, 0
                    };
                case 'u':
                    return new int[SIZE_SQUARED] {
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        0, 1, 1, 1, 0
                    };
                case 'v':
                    return new int[SIZE_SQUARED] {
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        0, 1, 0, 1, 0,
                        0, 1, 0, 1, 0,
                        0, 0, 1, 0, 0
                    };
                case 'w':
                    return new int[SIZE_SQUARED] {
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        1, 0, 1, 0, 1,
                        1, 0, 1, 0, 1,
                        0, 1, 0, 1, 0
                    };
                case 'x':
                    return new int[SIZE_SQUARED] {
                        1, 0, 0, 0, 1,
                        0, 1, 0, 1, 0,
                        0, 0, 1, 0, 0,
                        0, 1, 0, 1, 0,
                        1, 0, 0, 0, 1
                    };
                case 'y':
                    return new int[SIZE_SQUARED] {
                        1, 0, 0, 0, 1,
                        0, 1, 0, 1, 0,
                        0, 0, 1, 0, 0,
                        0, 0, 1, 0, 0,
                        0, 0, 1, 0, 0
                    };
                case 'z':
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 1,
                        0, 0, 0, 1, 0,
                        0, 0, 1, 0, 0,
                        0, 1, 0, 0, 0,
                        1, 1, 1, 1, 1
                    };
                case '0':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        1, 0, 0, 0, 1,
                        0, 1, 1, 1, 0
                    };
                case '1':
                    return new int[SIZE_SQUARED] {
                        0, 0, 1, 0, 0,
                        0, 1, 1, 0, 0,
                        1, 0, 1, 0, 0,
                        0, 0, 1, 0, 0,
                        0, 1, 1, 1, 0
                    };
                case '2':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        0, 0, 1, 1, 0,
                        0, 1, 0, 0, 0,
                        1, 1, 1, 1, 1
                    };
                case '3':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        0, 0, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        0, 1, 1, 1, 0
                    };
                case '4':
                    return new int[SIZE_SQUARED] {
                        0, 0, 1, 1, 0,
                        0, 1, 0, 1, 0,
                        1, 0, 0, 1, 0,
                        1, 1, 1, 1, 1,
                        0, 0, 0, 1, 0
                    };
                case '5':
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 1,
                        1, 0, 0, 0, 0,
                        1, 1, 1, 1, 0,
                        0, 0, 0, 0, 1,
                        1, 1, 1, 1, 0
                    };
                case '6':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 0,
                        1, 0, 0, 0, 0,
                        1, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        0, 1, 1, 1, 0
                    };
                case '7':
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 1,
                        0, 0, 0, 0, 1,
                        0, 0, 0, 1, 0,
                        0, 0, 1, 0, 0,
                        0, 0, 1, 0, 0
                    };
                case '8':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        0, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        0, 1, 1, 1, 0
                    };
                case '9':
                    return new int[SIZE_SQUARED] {
                        0, 1, 1, 1, 0,
                        1, 0, 0, 0, 1,
                        0, 1, 1, 1, 1,
                        0, 0, 0, 0, 1,
                        0, 1, 1, 1, 0
                    };
                default:
                    return new int[SIZE_SQUARED] {
                        1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1
                    };
            }
        }
    }
}
