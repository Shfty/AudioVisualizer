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
    public struct VertexPositionNormalColor : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Color Color;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

        public VertexPositionNormalColor(Vector3 Position, Vector3 Normal, Color Color)
        {
            this.Position = Position;
            this.Normal = Normal;
            this.Color = Color;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }

    class Cube
    {
        const int number_of_vertices = 8;
        const int number_of_indices = 36;
        private int seed;
        private bool edge;

        Vector3 bounds = new Vector3(0.5f, 1f, 0.5f);

        GraphicsDevice graphics;
        ContentManager content;

        Effect cubeEffect;

        Matrix World;
        Matrix worldInverseTransposeMatrix;

        public Vector3 position = Vector3.Zero;

        VertexPositionNormalColor[] cubeVertices;
        UInt16[] cubeIndices;

        VertexBuffer vertices;

        Vector3 LightDirection = new Vector3(1, -1, -1);
        Vector4 LightColor = new Vector4(0.8f, 0.8f, 0.8f, 1);

        public Cube(GraphicsDevice graphics, ContentManager content, int seed, bool edge)
        {
            this.graphics = graphics;
            this.content = content;
            this.seed = seed;
            this.edge = edge;

            cubeEffect = content.Load<Effect>("CubeShader");

            World = Matrix.Identity;

            CreateCubeVertexBuffer();
            CreateCubeIndexBuffer();
            CalculateNormals();
        }

        void CreateCubeVertexBuffer()
        {
            cubeVertices = new VertexPositionNormalColor[number_of_vertices];

            cubeVertices[0].Position = new Vector3(-bounds.X, -bounds.Y, -bounds.Z);
            cubeVertices[1].Position = new Vector3(-bounds.X, -bounds.Y, bounds.Z);
            cubeVertices[2].Position = new Vector3(bounds.X, -bounds.Y, bounds.Z);
            cubeVertices[3].Position = new Vector3(bounds.X, -bounds.Y, -bounds.Z);
            cubeVertices[4].Position = new Vector3(-bounds.X, bounds.Y, -bounds.Z);
            cubeVertices[5].Position = new Vector3(-bounds.X, bounds.Y, bounds.Z);
            cubeVertices[6].Position = new Vector3(bounds.X, bounds.Y, bounds.Z);
            cubeVertices[7].Position = new Vector3(bounds.X, bounds.Y, -bounds.Z);

            for (int i = 0; i < number_of_vertices; i++)
            {
                if (!edge)
                    cubeVertices[i].Color = SetColour();

                if (edge)
                    cubeVertices[i].Color = new Color(255, 255, 255, 255);
            }

            vertices = new VertexBuffer(graphics, VertexPositionNormalColor.VertexDeclaration, number_of_vertices, BufferUsage.WriteOnly);
            vertices.SetData<VertexPositionNormalColor>(cubeVertices);
        }

        private Color SetColour()
        {
            Color cubeColor = new Color();

            Random color = new Random(seed);

            switch (color.Next(1, 9))
            {
                case 1:
                    cubeColor = new Color(154, 205, 50, 255);
                    break;
                case 2:
                    cubeColor = new Color(20, 255, 20, 255);
                    break;
                case 3:
                    cubeColor = new Color(255, 60, 150, 255);
                    break;
                case 4:
                    cubeColor = new Color(120, 80, 255, 255);
                    break;
                case 5:
                    cubeColor = new Color(100, 190, 255, 255);
                    break;
                case 6:
                    cubeColor = new Color(20, 230, 230, 255);
                    break;
                case 7:
                    cubeColor = new Color(255, 140, 0, 255);
                    break;
                case 8:
                    cubeColor = new Color(255, 200, 0, 255);
                    break;
            }

            return cubeColor;
        }

        IndexBuffer indices;

        void CreateCubeIndexBuffer()
        {
            cubeIndices = new UInt16[number_of_indices];

            //bottom face
            cubeIndices[0] = 0;
            cubeIndices[1] = 2;
            cubeIndices[2] = 3;
            cubeIndices[3] = 0;
            cubeIndices[4] = 1;
            cubeIndices[5] = 2;

            //top face
            cubeIndices[6] = 4;
            cubeIndices[7] = 6;
            cubeIndices[8] = 5;
            cubeIndices[9] = 4;
            cubeIndices[10] = 7;
            cubeIndices[11] = 6;

            //front face
            cubeIndices[12] = 5;
            cubeIndices[13] = 2;
            cubeIndices[14] = 1;
            cubeIndices[15] = 5;
            cubeIndices[16] = 6;
            cubeIndices[17] = 2;

            //back face
            cubeIndices[18] = 0;
            cubeIndices[19] = 7;
            cubeIndices[20] = 4;
            cubeIndices[21] = 0;
            cubeIndices[22] = 3;
            cubeIndices[23] = 7;

            //left face
            cubeIndices[24] = 0;
            cubeIndices[25] = 4;
            cubeIndices[26] = 1;
            cubeIndices[27] = 1;
            cubeIndices[28] = 4;
            cubeIndices[29] = 5;

            //right face
            cubeIndices[30] = 2;
            cubeIndices[31] = 6;
            cubeIndices[32] = 3;
            cubeIndices[33] = 3;
            cubeIndices[34] = 6;
            cubeIndices[35] = 7;

            indices = new IndexBuffer(graphics, IndexElementSize.SixteenBits, number_of_indices, BufferUsage.WriteOnly);
            indices.SetData<UInt16>(cubeIndices);

        }

        private void CalculateNormals()
        {
            for (int i = 0; i < cubeIndices.Length / 3; i++)
            {
                int index1 = cubeIndices[i * 3];
                int index2 = cubeIndices[i * 3 + 1];
                int index3 = cubeIndices[i * 3 + 2];

                Vector3 side1 = cubeVertices[index1].Position - cubeVertices[index3].Position;
                Vector3 side2 = cubeVertices[index1].Position - cubeVertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                cubeVertices[index1].Normal = normal;
                cubeVertices[index2].Normal = normal;
                cubeVertices[index3].Normal = normal;
            }

            for (int i = 0; i < cubeVertices.Length; i++)
            {
                cubeVertices[i].Normal.Normalize();
            }
        }

        public void Draw(Matrix View, Matrix Projection)
        {
            graphics.SetVertexBuffer(vertices);
            graphics.Indices = indices;

            World = Matrix.CreateTranslation(position);
            worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(World));

            cubeEffect.CurrentTechnique = cubeEffect.Techniques["Technique1"];
            cubeEffect.Parameters["WVP"].SetValue(World * View * Projection);
            cubeEffect.Parameters["Line"].SetValue(0.1f);
            cubeEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
            cubeEffect.Parameters["DiffuseLightDirection"].SetValue(LightDirection);
            cubeEffect.Parameters["DiffuseLightColor"].SetValue(LightColor);

            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, number_of_vertices, 0, number_of_indices / 3);
            }
        }
    }
}
