using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine
{
    public class Building : IVisibleGameEntity
    {
        #region Fields

        CModel _model;
        
        #endregion

        #region Properties

        public string Name { get; set; }

        public bool Selected { get; set; }
        public BuildingContainer Container { get; set; }
        public CModel Model { get { return _model; } }

        public Vector3 Position { get { return Model.Position; } }
        public Vector3 Rotation { get { return Model.Rotation; } }
        public Vector3 Scale { get { return Model.Scale; } }

        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Construction

        public Building(CModel model, string name)
        {
            Name = name;
            _model = model;
            Selected = false;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            _model.Update(gameTime);
        }

        public void HandleInput(InputManager input)
        {
            _model.HandleInput(input);
        }

        #endregion

        #region Draw

        public void Draw(GameTime gameTime)
        {
            _model.SetModelEffectParameter("SelectedBuilding", Selected);
            _model.Draw(gameTime);
            BoundingSphereRenderer.Draw(_model.BoundingSphere, _model.Camera.View, _model.Camera.Projection);
        }

        #endregion

        #region Helper Methods
        
        public bool CheckSelecting(Ray ray)
        {
            if (MyMathHelper.IsIntersection(ray, _model.BoundingSphere))
            {
                Selected = true;
                Container.SelectBuiding(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ChangeTransform(Vector3 transform, string transformType)
        {
            switch (transformType)
            {
                case "Position":
                    _model.Position = transform;
                    break;
                case "Rotation":
                    _model.Rotation = transform;
                    break;
                case "Scale":
                    _model.Scale = transform;
                    break;
            }
        }

        #endregion
    }
}
