using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CommonLibrary.Graphics
{
    public class TerrainPatch
    {
        #region Fields

        /* There are two approaches:
          (1) Every patch contain vertex data. There is only a vertex buffer for all patches.
              When draw particular patch, we set vertex data of that patch for vertex buffer.
          (2) Every patch contain a vertex buffer. When draw particular patch, we set vertex buffer 
              of that patch to device.
           
           Approach (1) is not good because modifying vertex buffer's data is a expensive operation.
           So, we choose approach (2) */
        VertexBuffer _vertexBuffer;

        List<IVisibleGameEntity> _visibleEntities = new List<IVisibleGameEntity>();

        public List<IVisibleGameEntity> VisibleEntities { get { return _visibleEntities; } }

        #endregion

        #region Construction

        public TerrainPatch(VertexPositionNormalTexture[] patchVertices, int nVertices,
            GraphicsDevice device)
        {
            _vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture),
                nVertices, BufferUsage.WriteOnly);

            _vertexBuffer.SetData<VertexPositionNormalTexture>(patchVertices);
        }

        #endregion

        #region Draw

        public void Draw(int nVertices, int nIndices, GraphicsDevice device, GameTime gameTime)
        {
            device.SetVertexBuffer(_vertexBuffer);

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                nVertices, 0, nIndices / 3);

            foreach (IVisibleGameEntity entity in _visibleEntities)
                entity.Draw(gameTime);
        }

        #endregion

        #region Helper

        public void AddVisibleEntity(IVisibleGameEntity entity)
        {
            _visibleEntities.Add(entity);
        }

        #endregion
    }
}
