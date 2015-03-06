using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CommonLibrary.Graphics
{
    public class Billboard : IVisibleGameEntity
    {
        #region fields

        VertexBuffer verts;
        IndexBuffer ints;
        VertexPositionTexture[] particles;
        int[] indices;

        Vector2 billboardSize;
        Texture2D texture;

        GraphicsDevice graphicsDevice;
        Effect effect;
        Camera camera;

        public bool EnsureOcclusion = true;

        #endregion

        #region properties

        public Vector3 Up { get; set; }
        public Vector3 Right { get; set; }
        public Vector3 Position { get; set; }
        public float OffsetU { get; set; }
        public float OffsetV { get; set; }
        public bool Show { get; set; }

        #endregion

        #region construction

        public Billboard(GraphicsDevice graphicsDevice, ContentManager content, Texture2D texture,
            Camera camera, Vector2 billboardSize)
        {
            this.billboardSize = billboardSize;
            this.graphicsDevice = graphicsDevice;
            this.texture = texture;
            this.camera = camera;
            effect = content.Load<Effect>(@"level 1\effects\BillboardEffect");

            Position = Vector3.Zero;

            GenerateVertices();
        }

        #endregion

        #region update

        public void Update(GameTime gameTime)
        {
        }

        #endregion

        #region draw

        public void Draw(GameTime gameTime)
        {
            if (!Show)
            {
                return;
            }

            // Set the vertex and index buffer to the graphics card
            graphicsDevice.SetVertexBuffer(verts);
            graphicsDevice.Indices = ints;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            SetEffectParameters();
            if (EnsureOcclusion)
            {
                drawOpaquePixels();
                drawTransparentPixels();
            }
            else
            {
                graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                effect.Parameters["AlphaTest"].SetValue(false);
                drawBillboards();
            }
            // Reset render states
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            // Un-set the vertex and index buffer
            graphicsDevice.SetVertexBuffer(null);
            graphicsDevice.Indices = null;
        }

        void SetEffectParameters()
        {
            effect.Parameters["ParticleTexture"].SetValue(texture);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["Size"].SetValue(billboardSize / 2f);
            effect.Parameters["Up"].SetValue(Up);
            effect.Parameters["Side"].SetValue(Right);
            effect.Parameters["Position"].SetValue(Position);
            effect.Parameters["offsetU"].SetValue(OffsetU);
            effect.Parameters["offsetV"].SetValue(OffsetV);
            effect.CurrentTechnique.Passes[0].Apply();
        }

        void drawOpaquePixels()
        {
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            effect.Parameters["AlphaTest"].SetValue(true);
            effect.Parameters["AlphaTestGreater"].SetValue(true);
            drawBillboards();
        }
        void drawTransparentPixels()
        {
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            effect.Parameters["AlphaTest"].SetValue(true);
            effect.Parameters["AlphaTestGreater"].SetValue(false);
            drawBillboards();
        }
        void drawBillboards()
        {
            effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }

        #endregion

        #region helper

        private void GenerateVertices()
        {
            particles = new VertexPositionTexture[4];
            indices = new int[6];

            int x = 0;

            particles[0] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 0));
            particles[1] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 1));
            particles[2] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 1));
            particles[3] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 0));

            indices[x++] = 0;
            indices[x++] = 3;
            indices[x++] = 2;
            indices[x++] = 2;
            indices[x++] = 1;
            indices[x++] = 0;

            verts = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
            verts.SetData<VertexPositionTexture>(particles);

            ints = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, 6, BufferUsage.WriteOnly);
            ints.SetData<int>(indices);
        }

        #endregion

        #region unused

        public void HandleInput(InputManager input)
        {
        }

        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
