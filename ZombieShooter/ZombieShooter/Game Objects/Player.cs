using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace ZombieShooter
{
    public class Player : AnimatedModel
    {
        #region Fields

        Level_1 _level;
        List<IVisibleGameEntity> _visibleEntities = new List<IVisibleGameEntity>();
        Gun gun;
        public MP5Gun _mp5Gun;
        public RPGGun _rpgGun;
        public int PlayerHP;
        float _speed;
        float countTime = 0;
        bool isSpeedUp = false;
        public int TypeGun;
        public int PlayerMonney;

        #endregion

        #region Properties

        public Gun Gun { get { return gun; } }

        public float Speed { get { return _speed; } set { _speed = value; } }

        public override Matrix BaseWorld
        {
            get
            {
                if (PlayingAnimation)
                {
                    return Matrix.CreateScale(Scale) *
                        Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                        Matrix.CreateTranslation(Position);
                }
                else
                {
                    return Matrix.CreateScale(Scale) *
                        Matrix.CreateFromYawPitchRoll(Rotation.Y + MathHelper.ToRadians(180), Rotation.X, Rotation.Z) *
                        Matrix.CreateTranslation(Position);
                }
            }
        }

        #endregion

        #region Construction

        public Player(ContentManager content, Camera camera, GraphicsDevice device, Level_1 level)
            : base(@"level 1\models\dude\dude",
            new Vector3(500, 498, 50), new Vector3(0, 0, 0), new Vector3(2.0f), camera, device)
        {
            _level = level;
            PlayerHP = Global.PlayerHP;
            _speed = 8.0f;
            PlayerMonney = 0;

            LoadContent(content);

            LoadEntities(content, camera, device);
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (isSpeedUp)
            {
                Speed = 15.0f;
                countTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (countTime >= 7)
                {
                    countTime = 0;
                    Speed = 8.0f;
                    isSpeedUp = false;
                }
            }

            CheckProduct();

            base.Update(gameTime);

            RotatePlayer();

            Bone armBone = FindBone("L_UpperArm");
            armBone.SetCompleteTransform(Matrix.CreateRotationZ(MathHelper.ToRadians(-65)));

            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.Update(gameTime);
            }
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);

            Vector3 moveAmt = Vector3.Zero;
            if (input.IsMoveUp())
                moveAmt.Z = -1.0f;
            if (input.IsMoveDown())
                moveAmt.Z = 1.0f;
            if (input.IsMoveLeft())
                moveAmt.X = -1.0f;
            if (input.IsMoveRight())
                moveAmt.X = 1.0f;
            if (input.GetWheelDelta() != 0)
            {
                if (TypeGun == 1)
                {
                    ChangeGun(_rpgGun);
                    TypeGun = 2;
                }
                else
                {
                    ChangeGun(_mp5Gun);
                    TypeGun = 1;
                }
            }


            if (moveAmt.Length() > 0.0f && !PlayingAnimation)
                SetAnim(this, 0);

            if (input.IsStopMove() && PlayingAnimation)
                StopClip();

            moveAmt *= Speed;
            Position = MovePlayer(Position, moveAmt);

            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.HandleInput(input);
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.Draw(gameTime);
            }

            BoundingSphereRenderer.Draw(BoundingSphere, _camera.View, _camera.Projection);
        }

        #endregion

        #region Helper Methods

        public void SoundHeart()
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\take_healh");
                soundEffect.Play();
            }
        }

        public void SoundMoney()
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\drop_item");
                soundEffect.Play();
            }
        }

        public void SoundSpeed()
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\bomb_activate");
                soundEffect.Play();
            }
        }

        public void SoundBullet()
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\ammo_pickup");
                soundEffect.Play();
            }
        }

        public void SoundDeath()
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\male_death");
                soundEffect.Play();
            }
        }

        public void CheckProduct()
        {
            if (_level.ProductGroup != null)
                for (int i = _level.ProductGroup._productList.Count - 1; i >= 0; i--)
                {
                    if (MyMathHelper.IsIntersection(_level.ProductGroup._productList[i].BoundingSphere, BoundingSphere, 0))
                    {
                        switch (_level.ProductGroup._productList[i].Type)
                        {
                            case 1: //speed
                                countTime = 0;
                                isSpeedUp = true;
                                SoundSpeed();
                                break;
                            case 2: //money
                                PlayerMonney += 100;
                                SoundMoney();
                                break;
                            case 3: //bullet
                                _rpgGun.nBullet += 50;
                                SoundBullet();
                                break;
                            case 4: //heart
                                PlayerHP += 200;
                                if (PlayerHP > Global.MaxPlayerHP)
                                    PlayerHP = Global.MaxPlayerHP;
                                SoundHeart();
                                break;
                        }
                        _level.ProductGroup.RemoveProduct(_level.ProductGroup._productList[i]);
                    }
                }
        }

        public void AttackPlayer(int nHP)
        {
            PlayerHP -= nHP;
            if (PlayerHP <= 0)
            {
                SoundDeath();

                const string message = "Opps, You Lose !!!";

                EndScreen confirmExitMessageBox = new EndScreen(message);
                confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;


                _level.ScrMng.AddScreen(confirmExitMessageBox);
            }
        }

        private void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(_level.ScrMng, false,
                new BackgroundScreen(), new MainMenuScreen());
        }

        public void AddMoney(int money)
        {
            PlayerMonney += money;
        }

        private Vector3 MovePlayer(Vector3 currPos, Vector3 moveAmt)
        {
            Vector3 foreMove = currPos + moveAmt;

            if (_level.Terrain.DetermineTerrainType(foreMove.X, foreMove.Z) == Terrain.TerrainType.Ground)
            {
                Position = foreMove;
                if (_level.Terrain.CheckCollision(foreMove.X, foreMove.Z, this))
                    return currPos;
                else
                    return foreMove;
            }
            else
                return currPos;
        }

        private void RotatePlayer()
        {
            Vector3 dir = _level.Crosshair.Position -
                new Vector3(Position.X, _level.Crosshair.Position.Y, Position.Z);
            Vector3 baseVec = new Vector3(0, 0, -1);

            dir.Normalize();
            float cos = Vector3.Dot(dir, baseVec);

            if (cos < -1)
                cos = -1;
            if (cos > 1)
                cos = 1;

            float angle = (float)Math.Acos((double)cos);
            if (_level.Crosshair.Position.X <= Position.X)
            {
                angle = -angle;
            }

            Rotation = new Vector3(0, -angle, 0);
        }

        private void LoadEntities(ContentManager content, Camera camera, GraphicsDevice device)
        {
            _mp5Gun = new MP5Gun(_level,
                content.Load<Model>(@"level 1\models\MP5 gun\mp5"),
                content.Load<Texture2D>(@"level 1\models\MP5 gun\mp5_tex"),
                new Vector3(MathHelper.Pi, MathHelper.PiOver2, 0), new Vector3(0.65f),
                camera, device, this);
            _rpgGun = new RPGGun(_level,
                content.Load<Model>(@"level 1\models\RPG gun\RPG gun"),
                new Vector3(0, MathHelper.PiOver2, MathHelper.Pi), new Vector3(0.05f),
                camera, device, this);

            gun = _mp5Gun;
            _visibleEntities.Add(gun);
            TypeGun = 1;
        }

        public void ChangeGun(Gun newGun)
        {
            _visibleEntities.Remove(gun);
            gun = newGun;
            _visibleEntities.Add(newGun);
        }

        #endregion
    }
}
