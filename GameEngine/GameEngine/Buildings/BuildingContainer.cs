using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public class BuildingContainer : IVisibleGameEntity
    {
        #region Fields

        List<Building> _buildings = new List<Building>();
        MainScreen _mainScreen;
        Building _selectedBuilding = null;

        #endregion

        #region Properties

        public List<Building> Buildings { get { return _buildings; } }

        #endregion

        #region Construction

        public BuildingContainer(MainScreen mainScreen)
        {
            _mainScreen = mainScreen;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            foreach (Building building in _buildings)
            {
                building.Update(gameTime);
            }
        }

        public void HandleInput(InputManager input)
        {
            foreach (Building building in _buildings)
            {
                building.HandleInput(input);
            }
        }

        #endregion

        #region Draw

        public void Draw(GameTime gameTime)
        {
            foreach (Building building in _buildings)
            {
                building.Draw(gameTime);
            }
        }

        #endregion

        #region Public Methods

        public void AddBuilding(Building building)
        {
            building.Container = this;
            _buildings.Add(building);
        }

        public void CheckSelectingBuilding(Vector2 scrCoord,
            Camera camera, GraphicsDevice device)
        {
            Ray ray = MyMathHelper.CreateRayCasting(scrCoord, camera, device);

            foreach (Building building in _buildings)
            {
                if (building.CheckSelecting(ray))
                    break;
            }
        }

        public void SelectBuiding(Building selectedBuilding)
        {
            _selectedBuilding = selectedBuilding;
            foreach (Building building in _buildings)
            {
                building.Selected = false;
            }
            selectedBuilding.Selected = true;

            _mainScreen.ShowTransform(
                selectedBuilding.Model.Position, 
                selectedBuilding.Model.Rotation, 
                selectedBuilding.Model.Scale);
        }

        public void ChangeSelectedBuidingTransform(Vector3 transform, string transformType)
        {
            if (_selectedBuilding == null)
                return;

            _selectedBuilding.ChangeTransform(transform, transformType);
        }

        #endregion

        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }
    }
}
