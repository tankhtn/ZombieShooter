using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonLibrary.Graphics
{
    public abstract class Camera : IInvisibleGameEntity
    {
        #region Fields

        Matrix _view;
        Matrix _projection;

        #endregion

        #region Properties

        protected GraphicsDevice GraphicsDevice { get; set; }

        public Vector3 Position { get; set; }

        public Matrix View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
                GenerateFrustum();
            }
        }

        public Matrix Projection
        {
            get
            {
                return _projection;
            }
            set
            {
                _projection = value;
                GenerateFrustum();
            }
        }

        public BoundingFrustum Frustum { get; private set; }

        #endregion

        #region Initialization

        public Camera(GraphicsDevice graphicsDevice, float? aspectRatio)
        {
            GraphicsDevice = graphicsDevice;
            CreateProjectionMatrix(aspectRatio);
        }

        private void CreateProjectionMatrix(float? aspectRatio)
        {
            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            if (!aspectRatio.HasValue)
            {
                aspectRatio = (float)pp.BackBufferWidth / (float)pp.BackBufferHeight;
            }

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 
                aspectRatio.Value, 1.0f, 10000.0f);
        }

        private void GenerateFrustum()
        {
            Matrix viewProjection = View * Projection;
            Frustum = new BoundingFrustum(viewProjection);
        }

        #endregion

        #region Update

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void HandleInput(InputManager input)
        {
        }

        #endregion

        #region Helper Methods

        public bool BoundingVolumeIsInView(BoundingSphere sphere)
        {
            return (Frustum.Contains(sphere) != ContainmentType.Disjoint);
        }

        public bool BoundingVolumeIsInView(BoundingBox box)
        {
            return (Frustum.Contains(box) != ContainmentType.Disjoint);
        }

        #endregion

    }
}
