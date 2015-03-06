using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonLibrary.Graphics
{
    public class OverheadCamera : Camera
    {
        #region Fields

        Vector3 _lookAtVector;
        Vector3 _targetPoint;

        float _minHeight, _maxHeight;

        #endregion

        #region Properties

        #endregion

        #region Initialization

        public OverheadCamera(Vector3 position, Vector3 lookAtVector,
            float minHeight, float maxHeight,
            GraphicsDevice graphicsDevice, float? aspectRatio)
            : base(graphicsDevice, aspectRatio)
        {
            Position = position;
            _lookAtVector = lookAtVector;
            _minHeight = minHeight;
            _maxHeight = maxHeight;
        }

        #endregion

        #region Change Transforms

        public void Zoom(float amt)
        {
            Vector3 unitVector = Vector3.Normalize(_lookAtVector);

            if (CheckHeight((Position + amt * unitVector).Y, _minHeight, _maxHeight))
                Position += amt * unitVector;
        }

        private bool CheckHeight(float height, float minHeight, float maxHeight)
        {
            if (height >= minHeight && height <= maxHeight)
                return true;
            else
                return false;
        }

        public void Move(Vector3 amt)
        {
            Position += amt;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            _targetPoint = Position + _lookAtVector;

            Vector3 side = Vector3.Cross(_lookAtVector, Vector3.Up);
            Vector3 up = Vector3.Cross(side, _lookAtVector);

            View = Matrix.CreateLookAt(Position, _targetPoint, up);

            base.Update(gameTime);
        }

        #endregion
    }
}
