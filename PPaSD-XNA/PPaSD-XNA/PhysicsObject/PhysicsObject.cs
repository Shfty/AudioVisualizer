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
    class PhysicsObject
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Velocity = Vector3.Zero;
        public Vector3 Acceleration = Vector3.Zero;

        public void UpdatePhysics(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 newPos = Position + dt * Velocity;
            Vector3 newVel = Velocity + dt * Acceleration;

            Position = newPos;
            Velocity = newVel;
            Acceleration = Vector3.Zero;
        }
    }
}
