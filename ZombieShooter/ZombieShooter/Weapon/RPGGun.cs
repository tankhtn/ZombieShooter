using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ZombieShooter
{
    public class RPGGun : Gun
    {
        #region fields

        Projectile _projectile = null;

        ParticleSystem _explosion;
        ParticleSystem _explosionSmoke;
        ParticleSystem _projectileTrail;
        public int nBullet;

        #endregion

        #region construction

        public RPGGun(Level_1 lvl, Model model, Texture2D texture, 
            Vector3 rot, Vector3 scale, Camera camera, GraphicsDevice device, Player player)
            : base(lvl, model, texture, rot, scale, camera, device, player)
        {
            _gunDam = Global.RocketDam;
            nBullet = 0;
        }

        public RPGGun(Level_1 lvl, Model model, Vector3 rot, Vector3 scale,
            Camera camera, GraphicsDevice device, Player player)
            : base(lvl, model, rot, scale, camera, device, player)
        {
            _gunDam = Global.RocketDam;
            nBullet = 0;
        }

        #endregion

        #region update

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_projectile != null)
            {
                if (!_projectile.Update(gameTime))
                    _projectile = null;
            }
        }

        protected override void HandleShooting(Vector3 shotVector)
        {
            base.HandleShooting(shotVector);

            //shotVector *= 500;
            shotVector *= 1000;
            _projectile = new Projectile(_explosion, _explosionSmoke, _projectileTrail,
                    _gunPosition, shotVector);
        }

        #endregion

        #region Helper

        protected override void LoadEntities(ContentManager content, Camera camera, GraphicsDevice device)
        {
            base.LoadEntities(content, camera, device);

            _explosion = new ExplosionParticleSystem(device, content, camera);
            _explosionSmoke = new ExplosionSmokeParticleSystem(device, content, camera);
            _projectileTrail = new ProjectileTrailParticleSystem(device, content, camera);

            _visibleEntities.Add(_explosion);
            _visibleEntities.Add(_explosionSmoke);
            _visibleEntities.Add(_projectileTrail);
        }

        #endregion
    }
}
