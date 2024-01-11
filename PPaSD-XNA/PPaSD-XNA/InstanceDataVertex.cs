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
    public struct InstanceDataVertex
    {
        public Matrix World;
        public Color Colour;

        public InstanceDataVertex(Matrix world, Color colour)
        {
            World = world;
            Colour = colour;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
            //Colour Data
                 new VertexElement(sizeof(float) * 16, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            // World Matrix Data
                 new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5),
                 new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 6),
                 new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 7),
                 new VertexElement(sizeof(float) * 12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 8)
            );
    }

}
