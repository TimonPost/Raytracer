using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Raytracer.Raytracer;

namespace Raytracer
{
    class Cube {

        private VertexPositionColor[] vertices;
        private int[] indices;

        private Color color;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private GraphicsDevice graphicsDevice;

        private BasicEffect bEffect;

        public Cube(Color color, GraphicsDevice device, Camera camera)
        {
            graphicsDevice = device;
            this.color = color;
            SetUpVertices();
            SetUpIndices();

            bEffect = new BasicEffect(graphicsDevice);
            bEffect.VertexColorEnabled = true;
            bEffect.World = Matrix.CreateScale(new Vector3(0.1f, 0.1f, 0.1f));
            bEffect.Projection = Matrix.CreatePerspectiveFieldOfView(camera.Vfov, camera.Aspect, 0.1f, 100f);
            bEffect.View = Matrix.Identity;
        }

        public void Render()
        {
            foreach (EffectPass pass in bEffect.CurrentTechnique.Passes)
            {
                // This is the all-important line that sets the effect, and all of its settings, on the graphics device
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length,
                    indices, 0, 12);
            }
        }

        /// <summary>
        /// Sets up the vertices for a cube using 8 unique vertices.
        /// Build order is front to back, left to up to right to down.
        /// </summary>
        private void SetUpVertices()
        {
            
            vertices = new VertexPositionColor[8];

            //front left bottom corner
            vertices[0] = new VertexPositionColor(new Vector3(0, 0, 0), color);
            //front left upper corner
            vertices[1] = new VertexPositionColor(new Vector3(0, 1, 0), color);
            //front right upper corner
            vertices[2] = new VertexPositionColor(new Vector3(1, 1, 0), color);
            //front lower right corner
            vertices[3] = new VertexPositionColor(new Vector3(1, 0, 0), color);
            //back left lower corner
            vertices[4] = new VertexPositionColor(new Vector3(0, 0, -1), color);
            //back left upper corner
            vertices[5] = new VertexPositionColor(new Vector3(0, 1, -1), color);
            //back right upper corner
            vertices[6] = new VertexPositionColor(new Vector3(1, 1, -1), color);
            //back right lower corner
            vertices[7] = new VertexPositionColor(new Vector3(1, 0, -1), color);

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 8, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);
        }

        /// <summary>
        /// Sets up the indices for a cube. Has 36 positions that match up
        /// to the element numbers of the vertices created earlier.
        /// Valid range is 0-7 for each value.
        /// </summary>
        private void SetUpIndices()
        {
            indices = new int[36];

            //Front face
            //bottom right triangle
            indices[0] = 0;
            indices[1] = 3;
            indices[2] = 2;
            //top left triangle
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 0;
            //back face
            //bottom right triangle
            indices[6] = 4;
            indices[7] = 7;
            indices[8] = 6;
            //top left triangle
            indices[9] = 6;
            indices[10] = 5;
            indices[11] = 4;
            //Top face
            //bottom right triangle
            indices[12] = 1;
            indices[13] = 2;
            indices[14] = 6;
            //top left triangle
            indices[15] = 6;
            indices[16] = 5;
            indices[17] = 1;
            //bottom face
            //bottom right triangle
            indices[18] = 4;
            indices[19] = 7;
            indices[20] = 3;
            //top left triangle
            indices[21] = 3;
            indices[22] = 0;
            indices[23] = 4;
            //left face
            //bottom right triangle
            indices[24] = 4;
            indices[25] = 0;
            indices[26] = 1;
            //top left triangle
            indices[27] = 1;
            indices[28] = 5;
            indices[29] = 4;
            //right face
            //bottom right triangle
            indices[30] = 3;
            indices[31] = 7;
            indices[32] = 6;
            //top left triangle
            indices[33] = 6;
            indices[34] = 2;
            indices[35] = 3;

            indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, 36, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }
    }
}
