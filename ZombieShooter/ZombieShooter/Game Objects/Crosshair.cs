using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using CommonLibrary;

namespace ZombieShooter
{
    public class Crosshair : CModel
    {
        #region fields

        Level_1 _level;

        #endregion

        #region construction

        public Crosshair(ContentManager content, Camera camera, GraphicsDevice device, Level_1 level)
            : base(content.Load<Model>(@"level 1\models\crosshair\crosshair"),
            new Vector3(0, level.Terrain.BaseHeight + 100, -2000), new Vector3(MathHelper.ToRadians(90), 0, 0), 
            new Vector3(15), camera, device)
        {
            _level = level;
        }

        #endregion

        #region update

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);

            Point mouseCursorPos = input.GetCurrentMousePosition();
            Position = MyMathHelper.ScreenCoord2WorldCoord(
                new Vector2(mouseCursorPos.X, mouseCursorPos.Y), 
                _level.Terrain.BaseHeight + 100, Camera, Device);

            Vector3 dir =
                new Vector3(_level.Player.Position.X, Position.Y,
                    _level.Player.Position.Z) - Position;
            Vector3 baseVec = new Vector3(0, 0, 1);

            dir.Normalize();
            float cos = Vector3.Dot(dir, baseVec);

            if (cos < -1)
                cos = -1;
            if (cos > 1)
                cos = 1;

            float angle = (float)Math.Acos((double)cos);
            if (_level.Player.Position.X <= Position.X)
            {
                angle = -angle;
            }

            Rotation = new Vector3(0, angle + MathHelper.ToRadians(90), 0);
        }

        #endregion

        #region Draw

        protected override void DrawBoundingSphere()
        {
        }

        #endregion
    }
}
