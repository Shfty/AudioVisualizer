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
    class Cubes
    {
        //Standard System Stuff
        GraphicsDevice graphics;
        ContentManager content;

        //Wave Sim
        WaveSettings waveSettings;
        WaveSimulation waveSimulation;

        //Cube geometry settings
        public const float CUBE_SIZE = 1;
        const int NUMBER_OF_VERTICES = 8;
        const int NUMBER_OF_INDICES = 36;

        //Cube colours
        Color[,] cubeGridColours;

        //Additional cube data
        int gameObjectCount = 0;
        int additionalCubeCount;
        CubeWord[] words;
        int wordIndex;
        Director director;

        VertexPositionNormal[] vertices;
        ushort[] indices;
        InstanceDataVertex[] vertexData;

        //Buffers
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        DynamicVertexBuffer instancedVertexBuffer;

        //Rendering
        Effect cubeEffect;
        Vector3 LightDirection = new Vector3(1, -1, -1);
        Vector4 LightColor = new Vector4(0.8f, 0.8f, 0.8f, 1);

        public Cubes(GraphicsDevice graphics, ContentManager content)
        {
            this.graphics = graphics;
            this.content = content;

            cubeEffect = content.Load<Effect>("InstanceShader");

            waveSettings = new WaveSettings();
            waveSimulation = new WaveSimulation(waveSettings, content, graphics);
            

            CreateCubeVertexBuffer();
            CreateCubeIndexBuffer();
            CalculateNormals();

            int gridCubeCount = (int)waveSimulation.Dimensions().X * (int)waveSimulation.Dimensions().Y;

            Random random = new Random();
            words = new CubeWord[5];

            words[0] = new CubeWord(
                            gameObjectCount++,
                            new Vector3(0, 10, 0),
                            Quaternion.CreateFromYawPitchRoll(0, 0, 0),
                            "bob marley was right",
                            new Color[] { Color.Red, Color.Green, Color.Yellow }
                        );

            words[1] = new CubeWord(
                            gameObjectCount++,
                            new Vector3(0, 10, 0),
                            Quaternion.CreateFromYawPitchRoll(0, 0, 0),
                            "baguette",
                            new Color[] { Color.Blue, Color.White, Color.Red }
                        );

            words[2] = new CubeWord(
                            gameObjectCount++,
                            new Vector3(0, 10, 0),
                            Quaternion.CreateFromYawPitchRoll(0, 0, 0),
                            "bad weather",
                            new Color[] { Color.Red, Color.White, Color.Blue }
                        );

            words[3] = new CubeWord(
                            gameObjectCount++,
                            new Vector3(0, 10, 0),
                            Quaternion.CreateFromYawPitchRoll(0, 0, 0),
                            "chrome",
                            new Color[] { Color.White, Color.Gray, Color.DarkGray }
                        );

            words[4] = new CubeWord(
                            gameObjectCount++,
                            new Vector3(0, 10, 0),
                            Quaternion.CreateFromYawPitchRoll(0, 0, 0),
                            "smarties",
                            new Color[] { Color.Red, Color.Green, Color.Yellow, Color.Blue, Color.Purple, Color.Orange, Color.Pink }
                        );

            wordIndex = random.Next(words.Length);

            director = new Director();
            List<Command> commandList = new List<Command>();

            Command testCommand;
            testCommand.milliseconds = 2000;
            testCommand.position = new Vector3(0, 15, 0);
            testCommand.rotation = Quaternion.Identity;
            testCommand.scale = Vector3.One;

            Command testCommand2;
            testCommand2.milliseconds = 1000;
            testCommand2.position = new Vector3(0, 15, 0);
            testCommand2.rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), 90);
            testCommand2.scale = Vector3.One;

            Command testCommand3;
            testCommand3.milliseconds = 500;
            testCommand3.position = new Vector3(0, 15, 0);
            testCommand3.rotation = Quaternion.Identity;
            testCommand3.scale = Vector3.One;

            Command testCommand4;
            testCommand4.milliseconds = 250;
            testCommand4.position = new Vector3(0, 15, 0);
            testCommand4.rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), -90);
            testCommand4.scale = Vector3.One;

            commandList.Add(testCommand);
            commandList.Add(testCommand2);
            commandList.Add(testCommand3);
            commandList.Add(testCommand4);

            GameObject word = (GameObject)words[wordIndex];
            word.Loop = true;

            director.AddObject(ref word, commandList);
            director.Play();

            additionalCubeCount = 0;
            for (int i = 0; i < words[wordIndex].Letters.Length; i++ )
            {
                if (words[wordIndex].Letters[i] == null)
                {
                    continue;
                }

                additionalCubeCount += words[wordIndex].Letters[i].GetData().Length;
            }

            //Setup fixed-size data/colour containers
            vertexData = new InstanceDataVertex[gridCubeCount + additionalCubeCount];
            GenerateRandomColours();

            //Setup cube grid
            for (int x = 0; x < (int)waveSimulation.Dimensions().X; x++)
            {
                for (int z = 0; z < (int)waveSimulation.Dimensions().Y; z++)
                {
                    bool edge = (x == 0 || x == (int)waveSimulation.Dimensions().X - 1) || (z == 0 || z == (int)waveSimulation.Dimensions().Y - 1);

                    float xPos = x - ((int)waveSimulation.Dimensions().X / 2);
                    float zPos = z - ((int)waveSimulation.Dimensions().Y / 2);

                    vertexData[(z * (int)waveSimulation.Dimensions().X) + x] =
                        new InstanceDataVertex(
                            Matrix.CreateTranslation(new Vector3(xPos, 0.0f, zPos)),
                            cubeGridColours[x, z]
                        );
                }
            }

            //Setup letters
            int indexOffset = 0;

            for(int l = 0; l < words[wordIndex].Letters.Length; l++)
            {
                if (words[wordIndex].Letters[l] == null)
                {
                    continue;
                }

                for (int i = 0; i < words[wordIndex].Letters[l].GetData().Length; i++)
                {
                    vertexData[gridCubeCount + indexOffset] = words[wordIndex].Letters[l].GetData()[i];
                    indexOffset++;
                }
            }

            // Create instanced VertexBuffer with some temporary data
            instancedVertexBuffer = new DynamicVertexBuffer(graphics, InstanceDataVertex.VertexDeclaration, vertexData.Length, BufferUsage.None);
            instancedVertexBuffer.SetData<InstanceDataVertex>(vertexData);
        }

        public void Update(VisualizationData visualizer, GraphicsDevice device)
        {
            waveSimulation.Update(visualizer, device);

            int gridCubeCount = (int)waveSimulation.Dimensions().X * (int)waveSimulation.Dimensions().Y;

            //Do we need to recalculate after a size change?
            if (vertexData.Count() != gridCubeCount + additionalCubeCount)
            {
                vertexData = new InstanceDataVertex[gridCubeCount + additionalCubeCount];
                instancedVertexBuffer = new DynamicVertexBuffer(graphics, InstanceDataVertex.VertexDeclaration, vertexData.Length, BufferUsage.None);
                GenerateRandomColours();
            }

            //Position cube grid
            for (int x = 0; x < (int)waveSimulation.Dimensions().X; x++)
            {
                for (int z = 0; z < (int)waveSimulation.Dimensions().Y; z++)
                {
                    float xPos = x - ((int)waveSimulation.Dimensions().X / 2);
                    float zPos = z - ((int)waveSimulation.Dimensions().Y / 2);

                    vertexData[(z * (int)waveSimulation.Dimensions().X) + x] =
                        new InstanceDataVertex(
                            Matrix.CreateScale(1.0f, 1.0f + waveSimulation.Heights()[x, z], 1.0f) * Matrix.CreateTranslation(new Vector3(xPos, 0.0f, zPos)),
                            cubeGridColours[x, z]
                        );
                }
            }

            //Position letters
            director.Update();
            foreach (GameObject gameObject in director.Objects())
            {
                if (gameObject.Id == words[wordIndex].Id)
                {
                    words[wordIndex].Position = gameObject.Position;
                    words[wordIndex].Rotation = gameObject.Rotation;
                    words[wordIndex].Scale = gameObject.Scale;
                    words[wordIndex].Update();
                    break;
                }
            }

            //Account for wave height
            Vector3 adjustedY = words[wordIndex].Position + new Vector3(0, waveSimulation.AverageHeight(), 0);
            words[wordIndex].Position = adjustedY;

            //Load word cubes into buffer
            int indexOffset = 0;
            for (int l = 0; l < words[wordIndex].Letters.Length; l++)
            {
                if (words[wordIndex].Letters[l] == null)
                {
                    continue;
                }

                for (int i = 0; i < words[wordIndex].Letters[l].GetData().Length; i++)
                {
                    vertexData[gridCubeCount + indexOffset] = words[wordIndex].Letters[l].GetData()[i];
                    indexOffset++;
                }
            }

            instancedVertexBuffer.SetData<InstanceDataVertex>(vertexData);
        }

        public void Draw(Matrix View, Matrix Projection)
        {
            cubeEffect.CurrentTechnique = cubeEffect.Techniques["InstancePositionColour"];
            cubeEffect.Parameters["View"].SetValue(View);
            cubeEffect.Parameters["Projection"].SetValue(Projection);

            graphics.SetVertexBuffers(vertexBuffer, new VertexBufferBinding(instancedVertexBuffer, 0, 1));
            graphics.Indices = indexBuffer;
            graphics.RasterizerState = RasterizerState.CullCounterClockwise;
            graphics.BlendState = BlendState.Opaque;
            graphics.DepthStencilState = DepthStencilState.Default;

            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3, instancedVertexBuffer.VertexCount);
        }

        void CreateCubeVertexBuffer()
        {
            vertices = new VertexPositionNormal[NUMBER_OF_VERTICES];

            float halfSize = CUBE_SIZE / 2;
            vertices[0].Position = new Vector3(-halfSize, -halfSize, -halfSize);
            vertices[1].Position = new Vector3(-halfSize, -halfSize, halfSize);
            vertices[2].Position = new Vector3(halfSize, -halfSize, halfSize);
            vertices[3].Position = new Vector3(halfSize, -halfSize, -halfSize);
            vertices[4].Position = new Vector3(-halfSize, halfSize, -halfSize);
            vertices[5].Position = new Vector3(-halfSize, halfSize, halfSize);
            vertices[6].Position = new Vector3(halfSize, halfSize, halfSize);
            vertices[7].Position = new Vector3(halfSize, halfSize, -halfSize);

            vertexBuffer = new VertexBuffer(graphics, VertexPositionNormal.VertexDeclaration, NUMBER_OF_VERTICES, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormal>(vertices);
        }

        private void GenerateRandomColours()
        {
            Random random = new Random();
            cubeGridColours = new Color[(int)waveSimulation.Dimensions().X, (int)waveSimulation.Dimensions().Y];

            for (int x = 0; x < (int)waveSimulation.Dimensions().X; x++)
            {
                for (int z = 0; z < (int)waveSimulation.Dimensions().Y; z++)
                {
                    bool edge = (x == 0 || x == (int)waveSimulation.Dimensions().X - 1) || (z == 0 || z == (int)waveSimulation.Dimensions().Y - 1);

                    cubeGridColours[x, z] = RandomColour(random.Next(0, 9000), edge);
                }
            }
        }

        private Color RandomColour(int randNum, bool edge)
        {
            if (edge)
            {
                return Color.White;
            }

            Color cubeColor = new Color();

            switch (randNum % 8)
            {
                case 0:
                    cubeColor = new Color(154, 205, 50, 255);
                    break;
                case 1:
                    cubeColor = new Color(20, 255, 20, 255);
                    break;
                case 2:
                    cubeColor = new Color(255, 60, 150, 255);
                    break;
                case 3:
                    cubeColor = new Color(120, 80, 255, 255);
                    break;
                case 4:
                    cubeColor = new Color(100, 190, 255, 255);
                    break;
                case 5:
                    cubeColor = new Color(20, 230, 230, 255);
                    break;
                case 6:
                    cubeColor = new Color(255, 140, 0, 255);
                    break;
                case 7:
                    cubeColor = new Color(255, 200, 0, 255);
                    break;
            }

            return cubeColor;
        }

        void CreateCubeIndexBuffer()
        {
            indices = new UInt16[NUMBER_OF_INDICES];

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

            indexBuffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, NUMBER_OF_INDICES, BufferUsage.WriteOnly);
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
    }
}
