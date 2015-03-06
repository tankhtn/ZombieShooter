using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CommonLibrary.Graphics
{
    public class CTexture2D : IVisibleGameEntity
    {
        #region Fields

        Texture2D _texture;
        Vector2 _position;

        Effect _effect;
        SpriteBatch _spriteBatch;

        float _elapsedTimeSinceBegin = 0;

        #endregion

        #region Initialization

        public CTexture2D(Texture2D texture, Vector2 position,
            Effect effect, SpriteBatch spriteBatch)
        {
            _texture = texture;
            _position = position;
            _effect = effect;
            _spriteBatch = spriteBatch;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            _elapsedTimeSinceBegin += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void HandleInput(InputManager input)
        {
        }

        #endregion

        #region Draw

        public void Draw(GameTime gameTime)
        {
            BlendState blendState = BlendState.AlphaBlend;
            if (_effect != null)
                blendState = BlendState.Opaque;

            _spriteBatch.Begin(SpriteSortMode.Immediate, blendState);
            if (_effect != null)
            {
                _effect.Parameters["ElapsedTimeSinceBegin"].SetValue(_elapsedTimeSinceBegin);
                _effect.CurrentTechnique.Passes[0].Apply();
            }
            _spriteBatch.Draw(_texture, _position, Color.White);
            _spriteBatch.End();
        }

        #endregion

        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }
    }
}
