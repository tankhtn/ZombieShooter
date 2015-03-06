using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary;
using Microsoft.Xna.Framework.Content;

namespace ZombieShooter
{
    /// <summary>
    /// Lớp cha cho các menu screen như main menu, option, ...
    /// </summary>
    public abstract class MenuScreen : GameScreen
    {
        #region Fields

        protected ContentManager _content;

        string _menuTitle;
        protected SpriteFont _font;

        #endregion

        #region Initialization

        public MenuScreen(string menuTitle)
        {
            this._menuTitle = menuTitle;
        }

        public override void LoadContent()
        {
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _font = _content.Load<SpriteFont>(@"menu\fonts\MenuFont");
            Vector2 titlePosition = new Vector2((graphics.Viewport.Width - _font.MeasureString(_menuTitle).X) / 2, 80);

            /*_visibleEntities.Add(new UITextBlock("textBlock_MenuTitle", titlePosition, 
                _menuTitle, _font, Color.White, ScreenManager.SpriteBatch));*/

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _content.Unload();
        }

        #endregion

        #region Handle Input

        public override void Update(GameTime gameTime,
            bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (IsActive)
                ScreenManager.ShowMouse();
        }

        /// <summary>
        /// Handler dùng để loại bỏ màn hình menu khi user thoát khỏi menu
        /// </summary>
        protected virtual void OnCancel(object sender, UIWidgetArgs e)
        {
            ExitScreen();
        }

        #endregion
    }
}
