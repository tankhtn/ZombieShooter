using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CommonLibrary;

namespace ZombieShooter
{
    /// <summary>
    /// Loading Screen là màn hình xuất hiện giữa menu và gameplay screen, biểu
    /// thị cho quá trình load content.
    /// </summary>
    public class LoadingScreen : GameScreen
    {
        #region Fields

        ContentManager _content;

        bool _loadingIsSlow;
        bool _otherScreensAreGone = true;

        GameScreen[] _screensToLoad;

        SpriteFont _font;

        #endregion

        #region Initialization

        /// <summary>
        /// Chúng ta activate Loading Screen thông qua phương thức Load chứ không thông
        /// qua constructor
        /// </summary>
        private LoadingScreen(bool loadingIsSlow, GameScreen[] screensToLoad)
        {
            this._loadingIsSlow = loadingIsSlow;
            this._screensToLoad = screensToLoad;
        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _font = _content.Load<SpriteFont>(@"menu\fonts\MenuFont");
            ScreenManager.HideMouse();

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _content.Unload();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates màn hình Loading
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
            params GameScreen[] screensToLoad)
        {
            // Loại bỏ hết các màn hình đang có
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            LoadingScreen loadingScreen = new LoadingScreen(loadingIsSlow, screensToLoad);

            screenManager.AddScreen(loadingScreen);
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime,
            bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (_otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in _screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen);
                    }
                }

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1))
            {
                _otherScreensAreGone = true;
            }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (_loadingIsSlow)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

                const string message = "Loading...";

                // Center the text in the viewport.
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = _font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                ScreenManager.FadeBackBufferToBlack(1);

                // Draw the text.
                spriteBatch.Begin();
                spriteBatch.DrawString(_font, message, textPosition, Color.White);
                spriteBatch.End();
            }
        }

        #endregion
    }
}
