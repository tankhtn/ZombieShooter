using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary.Graphics;

namespace GameEngine
{
    public partial class MainScreen : GameScreen
    {
        #region Fields

        ContentManager _content;
        MainGame _mainGame;

        Viewport _mainViewport;
        bool _buildingTerrain = false;

        Dictionary<string, Model> _buildingModels = new Dictionary<string, Model>();
        Model _currentBuilding = null;
        string _currentBuildingName;
        Effect _buildEffect;

        #endregion

        #region Reference Fields

        Camera _camera;
        Terrain _terrain;
        BuildingContainer _buildingContainer;
        DummyInfo _dummyInfo;

        #endregion

        #region Initialization

        public MainScreen(MainGame mainGame)
        {
            _mainGame = mainGame;
            Window = mainGame.Window;
        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _buildingModels.Add("House_1", _content.Load<Model>(@"models\House_1\house_3ds"));
            _buildingModels.Add("Tree_1", _content.Load<Model>(@"models\Tree_1\spruce1_mrealms"));
            _buildingModels.Add("OldBridge", _content.Load<Model>(@"models\OldBridge\3ds file"));
            _buildingModels.Add("Dude", _content.Load<Model>(@"models\Dude\dude"));

            _buildEffect = _content.Load<Effect>(@"effects\BuildingEffect");

            InitializeMainScr();

            /***************** Test ****************/
            BoundingSphereRenderer.Initialize(_mainGame.GraphicsDevice, 45);
            /***************** Test ****************/

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _content.Unload();
        }

        #endregion

        #region Initialization Helpers

        private void InitializeRefFields()
        {
            Vector3 camPos = new Vector3(-100, 2000, 400);
            Vector3 lookAtVector = new Vector3(0, -1, -0.5f);
            float aspectRatio = (float)_mainViewport.Width / _mainViewport.Height;
            _camera = new OverheadCamera(camPos, lookAtVector, 500, 5000, _mainGame.GraphicsDevice, aspectRatio);

            TerrainConfig config = new TerrainConfig()
            {
                HeightMap = _content.Load<Texture2D>(@"textures\terrain"),
                BaseTexture = _content.Load<Texture2D>(@"textures\grass"),
                RockTexture = _content.Load<Texture2D>(@"textures\Rock"),
                WaterTexture = _content.Load<Texture2D>(@"textures\Water"),
                CellSize = 30,
                PatchSize = 32,
                MaxHeight = 1000,
                BaseColor = new Vector3(127.0f / 255),
                TextureTiling = 15,
                Effect = _content.Load<Effect>(@"effects\TerrainEffect"),
                WeightMapEffect = _content.Load<Effect>(@"effects\CreateWeightMap"),
                Camera = _camera,
                Device = _mainGame.GraphicsDevice
            };
            
            _terrain = new Terrain(config);
            
            _buildingContainer = new BuildingContainer(this);
            _dummyInfo = new DummyInfo(_content, ScreenManager.SpriteBatch);
        }

        #endregion

        #region Event Handler

        private void FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            uint result = MessageBox(new IntPtr(0), "Do you save ?", "Save", 3);

            switch (result)
            {
                case 6:
                    // Yes
                    MyXMLWriter xmlWriter = new MyXMLWriter(_buildingContainer.Buildings);
                    xmlWriter.Save("Buildings.xml");
                    break;
                case 7:
                    // No
                    break;
                case 2:
                    // Cancel
                    e.Cancel = true;
                    break;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mainGame.Exit();
        }

        private void terrainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_buildingTerrain)
            {
                InitializeBuildingTerrain();
                _buildingTerrain = true;
            }
        }

        private void SubmitTransform(object sender, TransfromArgs e)
        {
            UCEditTransform transform = sender as UCEditTransform;
            _buildingContainer.ChangeSelectedBuidingTransform(e.Transform, transform.TransformName);
        }

        private void BuildClicked(object sender, System.EventArgs e)
        {
            UCBuildingButton buildingBtn = sender as UCBuildingButton;
            if (buildingBtn == null)
                return;

            _currentBuildingName = buildingBtn.Name;
            _currentBuilding = _buildingModels[buildingBtn.Name];
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (!_buildingTerrain)
                return;

            _dummyInfo.TotalPatches = _terrain.TotalPatches;
            _dummyInfo.RenderedPatches = _terrain.RenderedPatches;
        }

        public override void HandleInput(InputManager input)
        {
            if (!_buildingTerrain)
                return;

            if (IsWithinViewport(input.GetCurrentMousePosition()))
            {
                UpdateScene(input);
            }
            else
            {
                UpdateControls(input);
            }
            
            base.HandleInput(input);
        }

        #endregion

        #region Update Helpers

        private void UpdateCamera(InputManager input)
        {
            int wheelDelta = input.GetWheelDelta();
            OverheadCamera camera = _camera as OverheadCamera;

            if (wheelDelta != 0)
            {
                camera.Zoom((float)wheelDelta / 5);
            }

            if (input.IsHoldingLeftMouse)
            {
                Vector2 delta = input.GetMouseMoveDelta();
                delta *= 3;
                camera.Move(new Vector3(-delta.X, 0, -delta.Y));
            }
        }

        private void UpdateScene(InputManager input)
        {
            flowLayoutPanelRight.AutoScroll = false;
            UpdateCamera(input);

            if (input.IsRightButtonReleased())
            {
                AddBuilding(new Vector2(
                    input.GetCurrentMousePosition().X, 
                    input.GetCurrentMousePosition().Y));
            }

            if (input.IsLeftButtonPressed())
            {
                _buildingContainer.CheckSelectingBuilding(new Vector2(
                    input.GetCurrentMousePosition().X,
                    input.GetCurrentMousePosition().Y),
                    _camera, _mainGame.GraphicsDevice);
            }
        }

        private void UpdateControls(InputManager input)
        {
            transform1.ReceiveKeyboard(input.CheckPressDigit());
            transform2.ReceiveKeyboard(input.CheckPressDigit());
            transform3.ReceiveKeyboard(input.CheckPressDigit());

            flowLayoutPanelRight.AutoScroll = true;
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            if (!_buildingTerrain)
                return;

            _mainGame.GraphicsDevice.Viewport = _mainViewport;
            base.Draw(gameTime);
        }

        #endregion

        #region Helper Methods

        private bool IsWithinViewport(Point point)
        {
            return _mainViewport.Bounds.Contains(point);
        }

        private void CreateMainViewport()
        {
            _mainViewport = _mainGame.GraphicsDevice.Viewport;

            _mainViewport.X = panelLeft.Size.Width;
            _mainViewport.Y = panelLeft.Location.Y;
            _mainViewport.Width = flowLayoutPanelRight.Location.X - _mainViewport.X;
            _mainViewport.Height = panelLeft.Size.Height;
        }

        private void InitializeMainScr()
        {
            BuildForm();
            panelLeft.Visible = false;
            flowLayoutPanelRight.Visible = false;
        }

        private void InitializeBuildingTerrain()
        {
            panelLeft.Visible = true;
            flowLayoutPanelRight.Visible = true;

            CreateMainViewport();
            InitializeRefFields();

            _invisibleEntities.Add(_camera);
            _visibleEntities.Add(_terrain);
            _visibleEntities.Add(_buildingContainer);
            _visibleEntities.Add(_dummyInfo);
        }

        private void AddBuilding(Vector2 screenCoord)
        {
            if (_currentBuilding == null)
                return;

            Vector3 worldCoord = MyMathHelper.ScreenCoord2WorldCoord(
                screenCoord, _terrain.BaseHeight, _camera, _mainGame.GraphicsDevice);

            CModel cModel = new CModel(_currentBuilding,
                worldCoord, Vector3.Zero, new Vector3(1),
                _camera, _mainGame.GraphicsDevice);

            cModel.SetModelEffect(_buildEffect, true);

            _buildingContainer.AddBuilding(new Building(cModel, _currentBuildingName));
        }

        #endregion

        #region Public Methods

        public void ShowTransform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            transform1.SetText(position.X.ToString(), position.Y.ToString(), position.Z.ToString());
            transform2.SetText(rotation.X.ToString(), rotation.Y.ToString(), rotation.Z.ToString());
            transform3.SetText(scale.X.ToString(), scale.Y.ToString(), scale.Z.ToString());
        }

        #endregion
    }
}
