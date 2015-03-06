using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonLibrary.Graphics
{
    public class ChaseCamera : Camera
    {
        #region Fields

        float _springiness = 0.15f;

        #endregion

        #region Properties

        /// <summary>
        /// Target that camera look at (not always object)
        /// </summary>
        public Vector3 Target { get; private set; }

        /// <summary>
        /// Object's position
        /// </summary>
        public Vector3 FollowTargetPosition { get; private set; }
        public Vector3 FollowTargetRotation { get; private set; }

        /// <summary>
        /// Vector from object's position to camera's position
        /// </summary>
        public Vector3 PositionOffset { get; set; }
        /// <summary>
        /// Vector from object's position to target that camera look at
        /// </summary>
        public Vector3 TargetOffset { get; set; }

        public Vector3 RelativeCameraRotation { get; set; }

        public float Springiness
        {
            get { return _springiness; }
            set { _springiness = MathHelper.Clamp(value, 0, 1); }
        }

        #endregion

        #region Initialization

        public ChaseCamera(Vector3 positionOffset, Vector3 targetOffset,
            Vector3 relativeCameraRotation, GraphicsDevice graphicsDevice, float? aspectRatio)
            : base(graphicsDevice, aspectRatio)
        {
            PositionOffset = positionOffset;
            TargetOffset = targetOffset;
            RelativeCameraRotation = relativeCameraRotation;
        }

        #endregion

        #region Change Transform

        public void Move(Vector3 newFollowTargetPosition, Vector3 newFollowTargetRotation)
        {
            FollowTargetPosition = newFollowTargetPosition;
            FollowTargetRotation = newFollowTargetRotation;
        }

        public void Rotate(Vector3 rotationChange)
        {
            RelativeCameraRotation += rotationChange;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            Vector3 combinedRotation = FollowTargetRotation + RelativeCameraRotation;
            Matrix rotation = Matrix.CreateFromYawPitchRoll(combinedRotation.Y,
                combinedRotation.X, combinedRotation.Z);
            Vector3 desiredPosition = FollowTargetPosition + Vector3.Transform(PositionOffset, rotation);

            Position = Vector3.Lerp(Position, desiredPosition, Springiness);
            Target = FollowTargetPosition + Vector3.Transform(TargetOffset, rotation);

            Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            View = Matrix.CreateLookAt(Position, Target, up);

            base.Update(gameTime);
        }

        #endregion
    }
}
