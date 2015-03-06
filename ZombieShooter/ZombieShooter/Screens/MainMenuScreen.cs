using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CommonLibrary;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ZombieShooter
{
    public class MainMenuScreen : MenuScreen
    {
        #region Fields

        SoundEffect MO, MC;
        List<bool> saveState;

        #endregion

        #region Initialization

        public MainMenuScreen()
            : base("Main Menu")
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (Global.isMusic)
            {
                MO = _content.Load<SoundEffect>(@"music\Wav\mouse_over");
                MC = _content.Load<SoundEffect>(@"music\Wav\mouse_click");
                Global.MenuSong = _content.Load<Song>(@"music\Music\menu");
                MediaPlayer.Play(Global.MenuSong);
            }

            UIButton playBtn = new UIButton("btn_play", new Vector2(512, 380),
                _content.Load<Texture2D>(@"menu\textures\newgame1"),
                _content.Load<Texture2D>(@"menu\textures\newgame2"),
                ScreenManager.SpriteBatch);
            playBtn.Clicked += PlayGameMenuEntrySelected;
            UIButton optionBtn = new UIButton("btn_option", new Vector2(512, 450),
                _content.Load<Texture2D>(@"menu\textures\options1"),
                _content.Load<Texture2D>(@"menu\textures\options2"),
                ScreenManager.SpriteBatch);
            optionBtn.Clicked += OptionSelected;
            UIButton exitBtn = new UIButton("btn_exit", new Vector2(512, 520),
                _content.Load<Texture2D>(@"menu\textures\exit1"),
                _content.Load<Texture2D>(@"menu\textures\exit2"),
                ScreenManager.SpriteBatch);
            exitBtn.Clicked += OnCancel;

            _visibleEntities.Add(playBtn);
            _visibleEntities.Add(optionBtn);
            _visibleEntities.Add(exitBtn);

            saveState = new List<bool>();
            for (int i = 0; i < _visibleEntities.Count; i++)
                saveState.Add(((UIButton)_visibleEntities[i]).MouseHover);

            ScreenManager.ShowMouse();
        }

        #endregion

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            for (int i = 0; i < _visibleEntities.Count; i++)
                if ((((UIButton)_visibleEntities[i]).MouseHover) && (saveState[i] == false)) SoundMouseHover();
            for (int i = 0; i < _visibleEntities.Count; i++)
                saveState[i] = ((UIButton)_visibleEntities[i]).MouseHover;
        }

        public void SoundMouseHover()
        {
            if (Global.isMusic)
                MO.Play();
        }

        public void SoundMouseClick()
        {
            if (Global.isMusic)
                MC.Play();
        }

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        private void PlayGameMenuEntrySelected(object sender, UIWidgetArgs e)
        {
            SoundMouseClick();
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
        }

        private void OptionSelected(object sender, UIWidgetArgs e)
        {
            SoundMouseClick();
            //ExitScreen();
            ScreenManager.AddScreen(new OptionMenuScreen());
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(object sender, UIWidgetArgs e)
        {
            SoundMouseClick();
            const string message = "Are you sure you want to exit this game?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        private void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        #endregion
    }
}
