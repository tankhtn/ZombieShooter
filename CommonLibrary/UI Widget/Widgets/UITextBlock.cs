using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonLibrary
{
    public class UITextBlock : UIWidget
    {
        #region Fields

        string _text;
        SpriteFont _font;
        Color _color;

        #endregion

        #region Properties

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        #endregion

        #region Constructor

        public UITextBlock(string id, Vector2 position,
            string text, SpriteFont font, Color color,
            SpriteBatch spriteBatch)
            : base(id, position, spriteBatch)
        {
            _text = text;
            _font = font;
            _color = color;

            Width = (int)font.MeasureString(text).X;
            Height = (int)font.MeasureString(text).Y;
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            SpriteBatch.Begin();
            SpriteBatch.DrawString(_font, _text, Position, _color);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
