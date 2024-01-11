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
    class BlockParticle : PhysicsObject
    {
        const int number_of_vertices = 8;
        const int number_of_indices = 36;
        private Vector3 bounds = new Vector3(0.5f, 0.5f, 0.5f);

        VertexPositionNormal[] vertices;
        ushort[] indices;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        Matrix World;
        Effect blockEffect;

        public BlockParticle(GraphicsDevice device, Vector3 position, Vector3 velocity, Vector3 acceleration)
        {
            AddVertices(device);
            AddIndices(device);
            CalculateNormals();

            this.Position = position;
            this.Velocity = velocity;
            this.Acceleration = acceleration;
        }

        public void Update(GameTime gameTime)
        {
            UpdatePhysics(gameTime);

            this.Acceleration.Z -= 0.1f;
        }

        private void AddVertices(GraphicsDevice device)
        {
            vertices = new VertexPositionNormal[number_of_vertices];

            vertices[0].Position = new Vector3(-bounds.X, -bounds.Y, -bounds.Z);
            vertices[1].Position = new Vector3(-bounds.X, -bounds.Y, bounds.Z);
            vertices[2].Position = new Vector3(bounds.X, -bounds.Y, bounds.Z);
            vertices[3].Position = new Vector3(bounds.X, -bounds.Y, -bounds.Z);
            vertices[4].Position = new Vector3(-bounds.X, bounds.Y, -bounds.Z);
            vertices[5].Position = new Vector3(-bounds.X, bounds.Y, bounds.Z);
            vertices[6].Position = new Vector3(bounds.X, bounds.Y, bounds.Z);
            vertices[7].Position = new Vector3(bounds.X, bounds.Y, -bounds.Z);

            vertexBuffer = new VertexBuffer(device, VertexPositionNormal.VertexDeclaration, number_of_vertices, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormal>(vertices);
        }

        private void AddIndices(GraphicsDevice device)
        {
            indices = new UInt16[number_of_indices];

            //bottom face
            indices[0] = 0;
            indices[1] = 2;
            indices[2] = 3;
            indices[3] = 0;
            indices[4] = 1;
            indices[5] = 2;

            //top face
            indices[6] = 4;
            indices[7] = 6;
            indices[8] = 5;
            indices[9] = 4;
            indices[10] = 7;
            indices[11] = 6;

            //front face
            indices[12] = 5;
            indices[13] = 2;
            indices[14] = 1;
            indices[15] = 5;
            indices[16] = 6;
            indices[17] = 2;

            //back face
            indices[18] = 0;
            indices[19] = 7;
            indices[20] = 4;
            indices[21] = 0;
            indices[22] = 3;
            indices[23] = 7;

            //left face
            indices[24] = 0;
            indices[25] = 4;
            indices[26] = 1;
            indices[27] = 1;
            indices[28] = 4;
            indices[29] = 5;

            //right face
            indices[30] = 2;
            indices[31] = 6;
            indices[32] = 3;
            indices[33] = 3;
            indices[34] = 6;
            indices[35] = 7;

            indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, number_of_indices, BufferUsage.WriteOnly);
            indexBuffer.SetData<ushort>(indices);
        }

        private void CalculateNormals()
        {
            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal = normal;
                vertices[index2].Normal = normal;
                vertices[index3].Normal = normal;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal.Normalize();
            }
        }

        public void Draw(Matrix View, Matrix Projection, GraphicsDevice device)
        {
            World = Matrix.CreateTranslation(Position);

            blockEffect.CurrentTechnique = blockEffect.Techniques["InstancePositionColour"];
            blockEffect.Parameters["View"].SetValue(View);
            blockEffect.Parameters["Projection"].SetValue(Projection);
            blockEffect.Parameters["World"].SetValue(World);

            device.SetVertexBuffers(vertexBuffer);
            device.Indices = indexBuffer;
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;

            foreach (EffectPass pass in blockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
        }
    }
}
