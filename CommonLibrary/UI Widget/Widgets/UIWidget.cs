using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonLibrary
{
    public class UIWidget : IVisibleGameEntity
    {
        #region Properties

        public string ID { get; protected set; }

        public bool Visible { get; set; }
        public bool Disabled { get; set; }

        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        #endregion

        #region Fields

        protected SpriteBatch SpriteBatch;

        protected bool IsMouseHover;
        protected bool IsFocus = false;

        #endregion

        #region Events

        public delegate void ClickHandle(object sender, UIWidgetArgs e);
        public event ClickHandle Clicked;

        #endregion

        #region Constructor

        public UIWidget(string id, Vector2 position, SpriteBatch spriteBatch)
        {
            ID = id;
            Position = position;
            SpriteBatch = spriteBatch;

            Visible = true;
        }

        #endregion

        #region Overrided Methods

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void HandleInput(InputManager input)
        {
            if (!Visible)
                return;

            CheckHover(input);

            if (IsMouseHover)
            {
                if (input.IsLeftButtonReleased())
                {
                    if (Clicked != null)
                    {
                        Clicked(this, new UIWidgetArgs(ID, input.GetCurrentMousePosition()));
                    }
                }
            }

            CheckFocus(input);
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        #endregion

        #region Helper Methods

        public void CheckHover(InputManager input)
        {
            IsMouseHover = CheckIsMouseHover(input.GetCurrentMousePosition());
        }

        private void CheckFocus(InputManager input)
        {
            if (input.IsLeftButtonReleased())
            {
                IsFocus = IsMouseHover;
            }
        }

        private bool CheckIsMouseHover(Point mousePos)
        {
            return Bounds.Contains(mousePos);
        }

        #endregion

        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }
    }
}
