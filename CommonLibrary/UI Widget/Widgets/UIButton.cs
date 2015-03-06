using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CommonLibrary
{
    public class UIButton : UIWidget
    {
        #region Fields

        Texture2D _texture;
        Texture2D _texturePress;
        Texture2D _currTexture;

        #endregion

        public bool MouseHover { get { return IsMouseHover; } }

        #region Constructor

        public UIButton(string id, Vector2 position, Texture2D texture, Texture2D texturePress,
            SpriteBatch spriteBatch)
            : base(id, position, spriteBatch)
        {
            _texture = texture;
            _texturePress = texturePress;
            _currTexture = _texture;

            Width = texture.Width;
            Height = texture.Height;
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            base.Update(gameTime);

            _currTexture = _texture;

            if (IsMouseHover)
            {
                _currTexture = _texturePress;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            SpriteBatch.Begin();
            SpriteBatch.Draw(_currTexture, Position, null, Color.White,
                0, Vector2.Zero, 1, SpriteEffects.None, 1);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
