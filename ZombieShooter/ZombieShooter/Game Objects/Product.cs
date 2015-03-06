using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ZombieShooter
{
    public class Product : CModel
    {
        int _type;

        public int Type { get { return _type; } }

        public Product(Model model, Vector3 position, Vector3 rotation,
            Vector3 scale, Camera camera, GraphicsDevice graphicsDevice, int type)
            : base(model, position, rotation, scale, camera, graphicsDevice)
        {
            _type = type;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Rotation += new Vector3(0, 0.05f, 0);
        }
    }
}
