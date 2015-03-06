using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Media;

namespace ZombieShooter
{
    public class OptionMenuScreen : MenuScreen
    {
        UICheckBox sound, diff;
        public OptionMenuScreen()
            : base("Option")
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _visibleEntities.Add(new CTexture2D(_content.Load<Texture2D>(@"menu\textures\option"), new Vector2(465, 100), null, ScreenManager.SpriteBatch));

            sound=new UICheckBox(new Vector2(576, 250), "Sound", _font,
                _content.Load<Texture2D>(@"menu\textures\checkbox_bound"),
                _content.Load<Texture2D>(@"menu\textures\checkbox_tick"), Global.isMusic, ScreenManager.SpriteBatch);
            sound.Clicked += ConfigSound_Clicked;
            diff=new UICheckBox(new Vector2(554, 300), "Difficult", _font,
                _content.Load<Texture2D>(@"menu\textures\checkbox_bound"),
                _content.Load<Texture2D>(@"menu\textures\checkbox_tick"), Global.isDifficult, ScreenManager.SpriteBatch);
            diff.Clicked += Configdiff_Clicked;
            _visibleEntities.Add(sound);
            _visibleEntities.Add(diff);


            ScreenManager.ShowMouse();
        }

        private void ConfigSound_Clicked(object sender, EventArgs e)
        {
            UICheckBox cb = sender as UICheckBox;

            if (!cb.Checked)
            {
                Global.isMusic = false;
                MediaPlayer.Pause();
            }
            else
            {
                Global.isMusic = true;
                Global.MenuSong = _content.Load<Song>(@"music\Music\menu");
                MediaPlayer.Play(Global.MenuSong);
            }
        }

        private void Configdiff_Clicked(object sender, EventArgs e)
        {
            UICheckBox cb = sender as UICheckBox;

            if (!cb.Checked)
            {
                Global.isDifficult = false;
            }
            else
            {
                Global.isDifficult = true;
            }
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);

            if (input.IsMenuCancel())
            {
                ExitScreen();
            }
        }
    }
}
