﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace ZombieShooter
{
    /// <summary>
    /// Custom particle system for leaving smoke trails behind the rocket projectiles.
    /// </summary>
    class ProjectileTrailParticleSystem : ParticleSystem
    {
        public ProjectileTrailParticleSystem(GraphicsDevice device, ContentManager content, Camera camera)
            : base(device, content, camera)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = @"level 1\textures\smoke";

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(3);

            settings.DurationRandomness = 1.5f;

            settings.EmitterVelocitySensitivity = 0.1f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1;

            /*settings.MinVerticalVelocity = -1;
            settings.MaxVerticalVelocity = 1;*/
            settings.MinVerticalVelocity = -10;
            settings.MaxVerticalVelocity = 10;

            settings.MinColor = new Color(64, 96, 128, 255);
            settings.MaxColor = new Color(255, 255, 255, 128);

            settings.MinRotateSpeed = -4;
            settings.MaxRotateSpeed = 4;

            /*settings.MinStartSize = 1;
            settings.MaxStartSize = 3;*/

            settings.MinStartSize = 10;
            settings.MaxStartSize = 20;

            settings.MinEndSize = 4;
            settings.MaxEndSize = 11;
        }
    }
}
