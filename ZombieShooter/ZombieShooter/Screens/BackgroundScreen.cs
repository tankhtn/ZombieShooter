using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CommonLibrary;
using CommonLibrary.Graphics;

namespace ZombieShooter
{
    /// <summary>
    /// Là background cho các screen khác
    /// </summary>
    public class BackgroundScreen : GameScreen
    {
        #region Fields

        ContentManager _content;

        #endregion

        #region Initialization

        public BackgroundScreen()
        {
        }

        /// <summary>
        /// Ở mỗi màn hình ta đều có một đối tượng ContentManager, khi mới tạo màn hình ta sẽ load content, và khi 
        /// loại bỏ màn hình ta sẽ unload content. Nếu dùng đối tượng ContentManager chung cho tấc cả màn hình thì
        /// khi một màn hình nào đó bị loại bỏ nhưng content của nó vẫn còn chiếm bộ nhớ.
        /// </summary>
        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _visibleEntities.Add(new CTexture2D(
                _content.Load<Texture2D>(@"menu\textures\background"), 
                Vector2.Zero, null, ScreenManager.SpriteBatch));
        }

        public override void UnloadContent()
        {
            _content.Unload();
        }


        #endregion

        #region Update


        /// <summary>
        /// Mặc dù màn hình này đang bị màn hình khác đè lên nhưng nó vẫn active
        /// (nếu nó không active nó sẽ không được vẽ và điều này vô lí). Do đó chúng
        /// ta luôn pass giá trị 'false' vào biến 'coveredByOtherScreen'
        /// </summary>
        public override void Update(GameTime gameTime,
            bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        #endregion
    }
}
