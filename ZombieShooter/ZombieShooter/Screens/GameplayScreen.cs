using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using CommonLibrary;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ZombieShooter
{
    public class GameplayScreen : GameScreen
    {
        #region Fields

        GameLevel _currLevel;

        #endregion

        #region Initialization

        public GameplayScreen()
        {
            
        }

        public override void LoadContent()
        {
            if (Global.isMusic)
            {
                Song song = Global.Content.Load<Song>(@"music\Music\music1");
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(song);
            }

            if (Global.isDifficult) Difficult();

            _currLevel = new Level_1(ScreenManager, ScreenManager.GraphicsDevice);
            _currLevel.LoadContent();

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            // Thread.Sleep(2000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            ScreenManager.HideMouse();
        }

        public void Difficult()
        {
            Global.ZombieHP += 200;
            Global.SpiderHP += 200;
            Global.MonsterHP += 200;

            Global.ZombieDam += 50;
            Global.SpiderDam += 50;
            Global.MonsterDam += 50;
        }

        public override void UnloadContent()
        {
            _currLevel.UnloadContent();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime,
            bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (IsActive)
            {
                // Update gameplay exclude controlling
                _currLevel.Update(gameTime);
            }
        }

        #endregion

        #region handle input

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputManager input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.IsPauseGame())
            {
                const string message = "Are you sure you want to quit this game?";

                MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);
                confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

                ScreenManager.AddScreen(confirmQuitMessageBox);
            }
            else
            {
                // Otherwise do controlling gameplay.
                _currLevel.UpdateInput(input);
            }
        }

        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false,
                new BackgroundScreen(), new MainMenuScreen());
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                Color.CornflowerBlue, 0, 0);

            _currLevel.Draw(gameTime, ScreenManager.SpriteBatch);
        }

        #endregion

        #region helper methods

        #endregion
    }
}
