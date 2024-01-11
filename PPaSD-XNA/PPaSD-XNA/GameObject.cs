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
    class GameObject
    {
        public int Id { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        //Animation
        public bool Playing { get; set; }
        public bool Loop { get; set; }
        public int CommandIndex { get; set; }
        public int CommandStartMs { get; set; }

        public GameObject(int id, Vector3 position, Quaternion rotation)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            Scale = new Vector3(1, 1, 1);
            Playing = false;
            Loop = false;
        }

        public GameObject(int id, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Playing = false;
            Loop = false;
        }
    }
}
