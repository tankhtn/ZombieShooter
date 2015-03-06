using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary;
using CommonLibrary.Graphics;

namespace ZombieShooter
{
    public class Level_1 : GameLevel
    {       
        #region Fields

        Dictionary<string, Model> _rawModels = new Dictionary<string, Model>();

        RenderCapture _renderCapture;
        PostProcessor _postprocessor;
        float _dieFactor = 0;

        #endregion

        #region constant

        public int nZombie = 6;
        public int nSpider = 8;
        public int nMonster = 4;

        #endregion

        #region Reference Fields

        Camera _camera;
        //DummyInfo _dummyInfo;
        StatusInfo _statusInfo;

        Terrain _terrain;
        Player _player;
        Crosshair _crosshair;
        ZombieGroup _zombieGroup;
        SpiderGroup _spiderGroup;
        MonsterGroup _monsterGroup;
        ProductGroup _productGroup;

        #endregion

        #region Properties

        public ScreenManager ScrMng { get { return _screenManager; } }
        public Player Player { get { return _player; } }
        public Terrain Terrain { get { return _terrain; } }
        public Crosshair Crosshair { get { return _crosshair; } }
        public Camera Camera { get { return _camera; } }
        public GraphicsDevice Device { get { return _device; } }
        public ContentManager Content { get { return _content; } }
        public ZombieGroup ZombieGroup { get { return _zombieGroup; } }
        public SpiderGroup SpiderGroup { get { return _spiderGroup; } }
        public MonsterGroup MonsterGroup { get { return _monsterGroup; } }
        public ProductGroup ProductGroup { get { return _productGroup; } }

        #endregion

        #region Initialization

        public Level_1(ScreenManager screenManager, GraphicsDevice device)
            : base(screenManager, device)
        {
            _gameTime = 0;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            MyXMLReader xmlReader = new MyXMLReader(TitleContainer.OpenStream(@"Content\level 1\data\Buildings.xml"));

            LoadRawModels();
            InitializeRefFields();

            AddDumpModels(xmlReader.Buildings);

            _invisibleEntities.Add(_camera);
            _visibleEntities.Add(_terrain);
            //_visibleEntities.Add(_dummyInfo);
            _visibleEntities.Add(_statusInfo);
            _visibleEntities.Add(_player);
            _visibleEntities.Add(_crosshair);
            _visibleEntities.Add(_zombieGroup);           
            _visibleEntities.Add(_spiderGroup);
            _visibleEntities.Add(_monsterGroup);
            _visibleEntities.Add(_productGroup);

            _renderCapture = new RenderCapture(_device);
            _postprocessor = new PostProcessor(_content, _device);
        }

        #endregion

        #region Initialization Helpers

        private void AddDumpModels(List<BuildingData> buildingDatas)
        {
            foreach (BuildingData data in buildingDatas)
            {
                CModel model = new CModel(_rawModels[data.Name], 
                    data.Position.Transform, data.Rotation.Transform, 
                    data.Scale.Transform, _camera, _device);
                if (data.Name == "OldBridge")
                    model.Name = data.Name;
                _terrain.AddVisibleEntity(data.Position.Transform.X, data.Position.Transform.Z, model);
            }
        }

        private void LoadRawModels()
        {
            _rawModels.Add("House_1", _content.Load<Model>(@"level 1\models\house_1\house_3ds"));
            _rawModels.Add("Tree_1", _content.Load<Model>(@"level 1\models\tree_1\spruce1_mrealms"));
            _rawModels.Add("OldBridge", _content.Load<Model>(@"level 1\models\old bridge\old bridge"));
        }

        private void InitializeRefFields()
        {
            _camera = new ChaseCamera(
                new Vector3(0, 1000, 750),
                new Vector3(0, 0, 0),
                Vector3.Zero, _device, null);

            TerrainConfig config = new TerrainConfig()
            {
                HeightMap = _content.Load<Texture2D>(@"level 1\textures\terrain"),
                BaseTexture = _content.Load<Texture2D>(@"level 1\textures\grass"),
                RockTexture = _content.Load<Texture2D>(@"level 1\textures\Rock"),
                WaterTexture = _content.Load<Texture2D>(@"level 1\textures\Water"),
                CellSize = 30,
                PatchSize = 32,
                MaxHeight = 1000,
                BaseColor = new Vector3(127.0f / 255),
                TextureTiling = 15,
                Effect = _content.Load<Effect>(@"level 1\effects\TerrainEffect"),
                WeightMapEffect = _content.Load<Effect>(@"level 1\effects\CreateWeightMap"),
                Camera = _camera,
                Device = _device
            };

            _terrain = new Terrain(config);
            //_dummyInfo = new DummyInfo(_content, _spriteBatch);
            _statusInfo = new StatusInfo(_spriteBatch, _content);
            _player = new Player(_content, _camera, _device, this);
            _crosshair = new Crosshair(_content, _camera, _device, this);
            _zombieGroup = new ZombieGroup(nZombie, this);
            _spiderGroup = new SpiderGroup(nSpider, this);
            _monsterGroup = new MonsterGroup(nMonster, this);
            _productGroup = new ProductGroup();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_player.TypeGun == 1)
            {
                _statusInfo.Gun = StatusInfo.GunType.MP5;
                _statusInfo.Bullet = 9999;
            }
            if (_player.TypeGun == 2)
            {
                _statusInfo.Gun = StatusInfo.GunType.RPG;
                _statusInfo.Bullet = _player._rpgGun.nBullet;
            }
            _statusInfo.Life = _player.PlayerHP;
            _statusInfo.Point = _player.PlayerMonney;

            if (_player.PlayerHP <= 500 && _player.PlayerHP >= 0)
                _dieFactor = -((float)_player.PlayerHP) / 500 + 1;
            if (_player.PlayerHP > 500)
                _dieFactor = 0;
            if (_dieFactor > 1)
                _dieFactor = 0;

            _gameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_gameTime >= Global.Level1Time)
            {
                const string message = "Congratulation, You Win !!!";

                EndScreen confirmExitMessageBox = new EndScreen(message);
                confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;


                _screenManager.AddScreen(confirmExitMessageBox);
            }//win
        }

        private void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(_screenManager, false,
                new BackgroundScreen(), new MainMenuScreen());
        }

        public override void UpdateInput(InputManager input)
        {
            base.UpdateInput(input);

            UpdateCamera(input);
        }

        #endregion

        #region Update Helper

        private void UpdateCamera(InputManager input)
        {
            ChaseCamera chaseCam = _camera as ChaseCamera;
            chaseCam.Move(_player.Position, Vector3.Zero);
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _renderCapture.Begin();

            base.Draw(gameTime, spriteBatch);

            _renderCapture.End();

            _postprocessor.Input = _renderCapture.GetTexture();
            EffectHelper.SetEffectParameter(_postprocessor.Effect, "WidthWindow", _device.Viewport.Width);
            EffectHelper.SetEffectParameter(_postprocessor.Effect, "HeightWindow", _device.Viewport.Height);
            EffectHelper.SetEffectParameter(_postprocessor.Effect, "DieFactor", _dieFactor);
            _postprocessor.Draw(null);
        }

        #endregion
    }
}
