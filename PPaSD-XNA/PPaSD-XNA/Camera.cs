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
    class Camera : GameObject
    {
        Vector3 Target { get; set; }

        public Camera(int id, Vector3 position, Vector3 target) : base(id, position, Quaternion.Identity)
        {
            Target = target;
        }

        public Matrix ViewMatrix()
        {
            return Matrix.CreateLookAt(
                Position,
                Position + Vector3.Transform(Vector3.Forward, Rotation),
                Vector3.Up
            );
        }
    }
}
