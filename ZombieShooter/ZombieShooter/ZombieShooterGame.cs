using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CommonLibrary;

namespace ZombieShooter
{
    public class ZombieShooterGame : Microsoft.Xna.Framework.Game
    {
        #region fields

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ScreenManager _screenManager;

        #endregion

        #region constructors

        public ZombieShooterGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 650;

            IsMouseVisible = true;

            _screenManager = new ScreenManager(this);

            ActivateFirstScreens(_screenManager);
        }

        #endregion

        #region initialization

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Global.Content = this.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _screenManager.BlankTexture = Content.Load<Texture2D>(@"common\textures\blank");

            /***************** Test ****************/
            BoundingSphereRenderer.Initialize(GraphicsDevice, 45);
            /***************** Test ****************/
        }

        protected override void UnloadContent()
        {
        }

        #endregion

        #region update

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        #endregion

        #region draw

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

        #endregion

        #region helper methods

        private void ActivateFirstScreens(ScreenManager screenManager)
        {
            screenManager.AddScreen(new BackgroundScreen());
            screenManager.AddScreen(new MainMenuScreen());
        }

        #endregion
    }
}
