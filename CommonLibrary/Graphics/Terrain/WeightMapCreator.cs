using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CommonLibrary.Graphics
{
    class WeightMapCreator
    {
        #region Fields

        Effect _effect;
        GraphicsDevice _graphicsDevice;
        Texture2D _heightMap;
        Vector3 _baseColor;

        RenderTarget2D _renderTarget;

        #endregion

        #region Properties

        public Texture2D WeightMap { get { return _renderTarget; } }

        #endregion

        #region Construction

        public WeightMapCreator(Texture2D heightMap, Vector3 baseColor, 
            Effect effect, GraphicsDevice graphicsDevice)
        {
            _heightMap = heightMap;
            _baseColor = baseColor;
            _effect = effect;
            _graphicsDevice = graphicsDevice;

            _renderTarget = new RenderTarget2D(_graphicsDevice,
                heightMap.Width, _heightMap.Height, false, 
                SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            CreateWeightMap();
        }

        #endregion

        #region Helper Methods

        private void CreateWeightMap()
        {
            SpriteBatch spriteBatch = new SpriteBatch(_graphicsDevice);

            _graphicsDevice.SetRenderTarget(_renderTarget);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

            _effect.Parameters["BaseColor"].SetValue(_baseColor);
            _effect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(_heightMap, Vector2.Zero, Color.White);

            spriteBatch.End();

            _graphicsDevice.SetRenderTarget(null);
            ResetRenderState();
        }

        private void ResetRenderState()
        {
            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        #endregion
    }
}
