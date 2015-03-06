using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CommonLibrary
{
    /// <summary>
    /// Quản lý các screen trong game
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields

        List<GameScreen> _screens = new List<GameScreen>();
        List<GameScreen> _screensToUpdate = new List<GameScreen>();

        InputManager _input = new InputManager();

        SpriteBatch _spriteBatch;
        Texture2D _blankTexture;

        bool _isInitialized;

        #endregion

        #region Properties

        /// <summary>
        /// SpriteBatch dùng chung cho tấc cả các screen để mỗi screen không cần phải tạo SpriteBatch.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        public Texture2D BlankTexture
        {
            get { return _blankTexture; }
            set { _blankTexture = value; }
        }

        #endregion

        #region Initialization

        public ScreenManager(Game game)
            : base(game)
        {
            Game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            _isInitialized = true;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (GameScreen screen in _screens)
            {
                screen.LoadContent();
            }
        }

        protected override void UnloadContent()
        {
            foreach (GameScreen screen in _screens)
            {
                screen.UnloadContent();
            }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            _input.Update(gameTime);

            // Tạo ra một bản copy cho ScreenList và ta sẽ update các screen trong
            // list copy này. Bởi vì khi một screen update nó có thể tự remove nó ra khỏi
            // screen list. Và nếu chúng ta update từng screen trong screen list thì có thể gây lỗi.
            _screensToUpdate.Clear();

            foreach (GameScreen screen in _screens)
                _screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            while (_screensToUpdate.Count > 0)
            {
                GameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];

                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.Active)
                {
                    // Cho phép màn hình đang actice nhận input, các màn hình khác
                    // không nhận được input
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(_input);
                        otherScreenHasFocus = true;
                    }

                    // Thông báo cho các màn hình khác là đã có màn hình active rồi,
                    // để các màn hình khác unactive
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        #endregion

        #region Public Methods

        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            if (_isInitialized)
            {
                screen.LoadContent();
            }

            _screens.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            if (_isInitialized)
            {
                screen.UnloadContent();
            }

            _screens.Remove(screen);
            _screensToUpdate.Remove(screen);
        }

        public GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }


        /// <summary>
        /// Dùng để làm mờ đi màn hình đằng sau màn hình pop-up
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            _spriteBatch.Begin();

            _spriteBatch.Draw(_blankTexture, 
                new Rectangle(0, 0, viewport.Width, viewport.Height), 
                Color.Black * alpha);

            _spriteBatch.End();
        }

        public void ShowMouse()
        {
            Game.IsMouseVisible = true;
        }

        public void HideMouse()
        {
            Game.IsMouseVisible = false;
        }

        #endregion
    }
}
