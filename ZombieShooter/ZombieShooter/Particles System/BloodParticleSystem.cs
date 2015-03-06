using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace ZombieShooter
{
    class BloodParticleSystem : ParticleSystem
    {
        public BloodParticleSystem(GraphicsDevice device, ContentManager content, Camera camera,
            string bloodTextureName)
            : base(device, content, camera, bloodTextureName)
        {
        }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            //settings.TextureName = @"level 1\textures\blood2";
            settings.TextureName = _assetName;

            settings.MaxParticles = 50;

            settings.Duration = TimeSpan.FromSeconds(2);
            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 20;
            settings.MaxHorizontalVelocity = 30;

            settings.MinVerticalVelocity = -20;
            settings.MaxVerticalVelocity = 20;

            settings.EndVelocity = 0;

            settings.MinColor = Color.DarkGray;
            settings.MaxColor = Color.Gray;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 7;
            settings.MaxStartSize = 7;

            settings.MinEndSize = 70;
            settings.MaxEndSize = 140;

        }
    }
}
