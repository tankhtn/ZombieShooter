using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ZombieShooter
{
    public class MP5Gun : Gun
    {
        #region fields


        //ParticleSystem _explosion;
        float _explosionLife = 0.7f;
        float _explosionAge = 0;


        int _fps = 10;
        float _spf;
        Point _currentSprite = new Point(0, 0);
        int _numSpriteWidth = 4;
        int _numSpriteHeight = 2;

        float _elapsedTime = 0;
        Billboard _gunExplosion;

        #endregion

        #region construction

        public MP5Gun(Level_1 lvl, Model model, Texture2D texture,
            Vector3 rot, Vector3 scale, Camera camera, GraphicsDevice device, Player player)
            : base(lvl, model, texture, rot, scale, camera, device, player)
        {
            _spf = 1.0f / _fps;
            _gunDam = Global.ShortGunDam;
        }

        public MP5Gun(Level_1 lvl, Model model, Vector3 rot, Vector3 scale,
            Camera camera, GraphicsDevice device, Player player)
            : base(lvl, model, rot, scale, camera, device, player)
        {
            _spf = 1.0f / _fps;
            _gunDam = Global.ShortGunDam;
        }

        #endregion

        #region update and draw

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_elapsedTime >= _spf)
            {
                _currentSprite.X++;
                if (_currentSprite.X >= _numSpriteWidth)
                {
                    _currentSprite.X = 0;
                    _currentSprite.Y++;
                    if (_currentSprite.Y >= _numSpriteHeight)
                        _currentSprite.Y = 0;
                }
                _gunExplosion.OffsetU = ((float)_currentSprite.X) / _numSpriteWidth;
                _gunExplosion.OffsetV = ((float)_currentSprite.Y) / _numSpriteHeight;
                _elapsedTime -= _spf;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ChaseCamera chaseCam = Camera as ChaseCamera;

            base.Draw(gameTime);
        }

        protected override void UpdateParticles(GameTime gameTime)
        {
            base.UpdateParticles(gameTime);

            if (!_shooting)
                return;

            Vector3 shootVector = _level.Crosshair.Position - _gunPosition;
            shootVector.Normalize();
            shootVector *= 50;

            _explosionAge += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_explosionAge <= _explosionLife)
            {
                //_explosion.AddParticle(_gunPosition + shootVector, Vector3.Zero);
                _gunExplosion.Position = _gunPosition + shootVector;
                _gunExplosion.Show = true;
            }
            else
            {
                _shooting = false;
                _explosionAge = 0;
                _gunExplosion.Show = false;
            }
        }

        #endregion

        #region Helper

        protected override void LoadEntities(ContentManager content, Camera camera, GraphicsDevice device)
        {
            base.LoadEntities(content, camera, device);
            /*
            _explosion = new ExplosionParticleSystem(device, content, camera);
            _visibleEntities.Add(_explosion);
             * */

            _gunExplosion = new Billboard(device, content,
                content.Load<Texture2D>(@"level 1\textures\gun explosion"),
                camera, new Vector2(100, 100));
            _visibleEntities.Add(_gunExplosion);
        }

        #endregion
    }
}
