using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ZombieShooter
{
    public class StatusInfo : IVisibleGameEntity
    {
        public enum GunType { MP5, RPG }

        SpriteBatch _spriteBatch;
        SpriteFont _font;

        public GunType Gun { get; set; }
        public int Life { get; set; }
        public int Bullet { get; set; }
        public int Point { get; set; }

        Texture2D _hudIcon;
        Texture2D _mp5Gun;
        Texture2D _rpgGun;

        public StatusInfo(SpriteBatch spriteBatch, ContentManager content)
        {
            _spriteBatch = spriteBatch;
            Gun = GunType.MP5;
            Life = 2;
            Bullet = 1;
            Point = 3;

            LoadContent(content);
        }

        private void LoadContent(ContentManager content)
        {
            _hudIcon = content.Load<Texture2D>(@"level 1\textures\hud icon");
            _mp5Gun = content.Load<Texture2D>(@"level 1\textures\mp5gunicon");
            _rpgGun = content.Load<Texture2D>(@"level 1\textures\rpggunicon");

            _font = content.Load<SpriteFont>(@"level 1\fonts\HUDFont");
        }

        public void Update(GameTime gameTime)
        {
        }

        public void HandleInput(InputManager input)
        {
        }

        public void Draw(GameTime gameTime)
        {
            Texture2D tex = _mp5Gun;
            if (Gun == GunType.RPG)
                tex = _rpgGun;

            _spriteBatch.Begin();
            _spriteBatch.Draw(tex, new Vector2(20, 530 - tex.Height + 30), Color.White);
            _spriteBatch.Draw(_hudIcon, new Vector2(20, 530), Color.White);
            _spriteBatch.DrawString(_font, Bullet.ToString(), new Vector2(161, 536), Color.White);
            _spriteBatch.DrawString(_font, Life.ToString(), new Vector2(161, 572), Color.White);
            _spriteBatch.DrawString(_font, Point.ToString(), new Vector2(161, 606), Color.White);
            _spriteBatch.End();
        }

        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }
    }
}
