using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonLibrary
{
    public class UICheckBox : UIWidget
    {
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Texture2D CheckBoxBound { get; set; }
        public Texture2D CheckBoxTick { get; set; }

        public bool Checked { get; set; }

        public UICheckBox(Vector2 position, string text, SpriteFont font,
            Texture2D checkBoxBound, Texture2D checkBoxTick, bool isChecked, SpriteBatch spriteBatch)
            : base("", position, spriteBatch)
        {
            Text = text;
            Font = font;
            CheckBoxBound = checkBoxBound;
            CheckBoxTick = checkBoxTick;

            Checked = isChecked;

            Width = (int)font.MeasureString(text).X + (int)(1.5f * checkBoxBound.Width);
            Height = checkBoxBound.Height;

            Clicked += UICheckBox_Clicked;
        }

        private void UICheckBox_Clicked(object sender, EventArgs e)
        {
            Checked = !Checked;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            /**********************              Vẽ text               **********************/
            
            Vector2 textPos = new Vector2(Position.X,
                Position.Y + (Bounds.Height - Font.MeasureString(Text).Y) / 2);

            SpriteBatch.DrawString(Font, Text, textPos, Color.White);

            /**********************              Vẽ text               **********************/

            /**********************              Vẽ khung bên ngoài check box               **********************/

            Color checkBoxBoundColor = Color.White;
            if (IsMouseHover)
            {
                checkBoxBoundColor = new Color(141, 252, 158);
            }

            Vector2 checkBoxBoundPos = new Vector2(Position.X + Width - CheckBoxBound.Width, Position.Y);

            SpriteBatch.Draw(CheckBoxBound, checkBoxBoundPos, checkBoxBoundColor);

            /**********************              Vẽ khung bên ngoài check box               **********************/

            /**********************              Vẽ phần check bên trong               **********************/

            if (Checked)
            {
                Vector2 checkBoxTickPos = checkBoxBoundPos + new Vector2(5, 5);
                SpriteBatch.Draw(CheckBoxTick, checkBoxTickPos, Color.White);
            }

            /**********************              Vẽ phần check bên trong               **********************/

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
