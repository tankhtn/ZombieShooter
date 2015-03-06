using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary;
using SkinnedModelData;
using Microsoft.Xna.Framework.Audio;

namespace ZombieShooter
{
    public class Spider : AnimatedModel
    {
        #region enumeration

        public enum SpiderAiState
        {
            Chasing,
            Caught,
            Wander,
            Die
        }

        #endregion

        #region constant

        const float MaxSpiderSpeed = 3.0f;
        const float SpiderTurnSpeed = 0.10f;
        const float SpiderChaseDistance = 800.0f;
        const float SpiderCaughtDistance = 200.0f;
        const float SpiderHysteresis = 15.0f;

        #endregion

        #region fields

        public SpiderAiState _SpiderState = SpiderAiState.Wander;
        float _SpiderOrientation;
        Vector3 _SpiderWanderDirection;
        Level_1 _level;
        Model spiderWalk, spiderAttack;
        ModelExtra walkExtra, attackExtra;
        public int SpiderHP;
        public int SpiderDam;
        Random rand = new Random();

        BloodParticleSystem _bloodParticles;
        float _bloodParticlesLife = 0.1f;
        float _bloodParticlesAge = 0;
        bool _shooted = false;

        List<IVisibleGameEntity> _visibleEntities = new List<IVisibleGameEntity>();

        float countTime = 0; 

        #endregion

        #region construction

        public Spider(ContentManager content, Camera camera, GraphicsDevice device, Level_1 lvl,
            Vector3 position, Vector3 scale, Vector3 rotation)
            : base(@"level 1\models\Spider\Spider\Walk", position, rotation, scale, camera, device)
        {
            _level = lvl;
            SpiderHP = Global.SpiderHP;
            SpiderDam = Global.SpiderDam;

            LoadContent(content);

            spiderWalk = content.Load<Model>(@"level 1\models\Spider\Spider\Walk");
            walkExtra = spiderWalk.Tag as ModelExtra;
            spiderAttack = content.Load<Model>(@"level 1\models\Spider\Spider\Attack");
            attackExtra = spiderAttack.Tag as ModelExtra;

            _model = spiderWalk;
            _modelExtra = walkExtra;
            SetAnim(this, 0);

            LoadEntities(lvl.Content, camera, device);
        }

        #endregion

        #region update

        public override void Update(GameTime gameTime)
        {
            if (_SpiderState == SpiderAiState.Die)
            {
                countTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (countTime >= 1.5)
                {
                    if (rand.Next(7) == 0)
                    {
                        int type = rand.Next(4) + 1;
                        Product product = CreateProduct(type);
                        _level.ProductGroup.AddProduct(product);
                    }
                    _level.SpiderGroup.killSpider(this);
                    _level.SpiderGroup.AddSpider();
                }
            }
            else
            {
                float SpiderChaseThreshold = SpiderChaseDistance;
                float SpiderCaughtThreshold = SpiderCaughtDistance;

                if (_SpiderState == SpiderAiState.Wander)
                {
                    SpiderChaseThreshold -= SpiderHysteresis / 2;
                }
                else if (_SpiderState == SpiderAiState.Chasing)
                {
                    SpiderChaseThreshold += SpiderHysteresis / 2;
                    SpiderCaughtThreshold -= SpiderHysteresis / 2;
                }
                else if (_SpiderState == SpiderAiState.Caught)
                {
                    SpiderCaughtThreshold += SpiderHysteresis / 2;
                }

                float distanceFromCat = Vector3.Distance(Position, _level.Player.Position);
                if (distanceFromCat > SpiderChaseThreshold)
                {
                    if (_SpiderState != SpiderAiState.Wander)
                    {
                        _model = spiderWalk;
                        _modelExtra = walkExtra;
                        SetAnim(this, 0);
                    }
                    _SpiderState = SpiderAiState.Wander;
                }
                else if (distanceFromCat > SpiderCaughtThreshold)
                {
                    if (_SpiderState != SpiderAiState.Chasing)
                    {
                        _model = spiderWalk;
                        _modelExtra = walkExtra;
                        SetAnim(this, 0);
                    }
                    _SpiderState = SpiderAiState.Chasing;
                }
                else
                {
                    countTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (countTime >= 1)
                    {
                        SoundAttack();
                        countTime = -1;
                        _level.Player.AttackPlayer(SpiderDam);
                    }

                    if (_SpiderState != SpiderAiState.Caught)
                    {
                        _model = spiderAttack;
                        _modelExtra = attackExtra;
                        SetAnim(this, 0);
                    }
                    _SpiderState = SpiderAiState.Caught;                  
                }

                float currentSpiderSpeed;
                if (_SpiderState == SpiderAiState.Chasing)
                {
                    _SpiderOrientation = TurnToFace(Position, _level.Player.Position, _SpiderOrientation,
                        SpiderTurnSpeed);
                    currentSpiderSpeed = MaxSpiderSpeed;
                }
                else if (_SpiderState == SpiderAiState.Wander)
                {
                    Wander(Position, ref _SpiderWanderDirection, ref _SpiderOrientation,
                        SpiderTurnSpeed);
                    currentSpiderSpeed = .5f * MaxSpiderSpeed;
                }
                else
                {
                    currentSpiderSpeed = 0.0f;
                }

                Vector3 heading = new Vector3(
                    (float)Math.Cos(_SpiderOrientation), 0, (float)Math.Sin(_SpiderOrientation));
                Position = MoveSpider(Position, heading * currentSpiderSpeed);

                float rY = WrapAngle(-_SpiderOrientation);
                Rotation = new Vector3(Rotation.X, rY + 1.5f, Rotation.Z);
            }

            UpdateParticles(gameTime);

            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.Update(gameTime);
            }

            base.Update(gameTime);
        }

        private void UpdateParticles(GameTime gameTime)
        {
            if (!_shooted)
                return;

            _bloodParticlesAge += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_bloodParticlesAge <= _bloodParticlesLife)
            {
                Vector3 shotVector = this.Position - _level.Player.Gun.GunPosition;
                shotVector.Normalize();
                _bloodParticles.AddParticle(Position - shotVector * 50, Vector3.Zero);
            }
            else
            {
                _shooted = false;
                _bloodParticlesAge = 0;
            }
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);

            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.HandleInput(input);
            }
        }

        #endregion

        #region draw

        public override void Draw(GameTime gameTime)
        {
            //BoundingSphereRenderer.Draw(BoundingSphere, _camera.View, _camera.Projection);

            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        #endregion

        #region helper methods

        public Product CreateProduct(int type)
        {
            Product product = null;

            switch (type)
            {
                case 1:
                    product = new Product(Global.Content.Load<Model>(@"level 1\models\shoe\shoesmini_fbx"),
                            Position + new Vector3(0, 50, 0), new Vector3(0, 1.0f, 0), new Vector3(0.3f), _camera, _device, type);
                    break;
                case 2:
                    product = new Product(Global.Content.Load<Model>(@"level 1\models\cherries\cherry"),
                            Position + new Vector3(0, 50, 0), new Vector3((float)Math.PI, 0, (float)Math.PI - 0.5f), new Vector3(1f), _camera, _device, type);
                    break;
                case 3:
                    product = new Product(Global.Content.Load<Model>(@"level 1\models\bullet\45_Bullet"),
                            Position + new Vector3(0, -70, 0), new Vector3(0, 0.0f, 0), new Vector3(20f), _camera, _device, type);
                    break;
                case 4:
                    product = new Product(Global.Content.Load<Model>(@"level 1\models\heart\Cupid Heart"),
                            Position + new Vector3(0, 50, 0), new Vector3(0, 0.5f, 0), new Vector3(0.1f), _camera, _device, type);
                    break;
            }

            return product;
        }

        public void SoundDeath()
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\monster5_death");
                soundEffect.Play();
            }
        }

        public void SoundAttack()
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\monster5_attack");
                soundEffect.Play();
            }
        }

        public void SpiderDie()
        {
            _shooted = true;
            SpiderHP -= _level.Player.Gun.GunDam;
            if (SpiderHP <= 0)
            {
                _SpiderState = SpiderAiState.Die;
                countTime = 0;
                SetAnim(this, 0);
                Position += new Vector3(0, 100, 0);
                Rotation = new Vector3(Rotation.X + (float)Math.PI, Rotation.Y, Rotation.Z);
                SoundDeath();
                _level.Player.AddMoney(Global.SpriderMoney);
            }
        }

        private Vector3 MoveSpider(Vector3 currPos, Vector3 moveAmt)
        {
            Vector3 foreMove = currPos + moveAmt;

            if (_level.Terrain.DetermineTerrainType(foreMove.X, foreMove.Z) == Terrain.TerrainType.Ground)
            {
                Position = foreMove;
                if ((_level.Terrain.CheckCollision(foreMove.X, foreMove.Z, this)) || (SpiderCheckCollision()))
                    return currPos;
                else
                    return foreMove;
            }
            else
                return currPos;
        }

        private bool SpiderCheckCollision()
        {
            foreach (Spider vSpider in _level.SpiderGroup._spiderGroup)
            {
                if (vSpider != this)
                    if (MyMathHelper.IsIntersection(BoundingSphere, vSpider.BoundingSphere, 80))
                        return true;
            }

            foreach (Monster vMonster in _level.MonsterGroup.MonsterList)
            {
                if (MyMathHelper.IsIntersection(BoundingSphere, vMonster.BoundingSphere, 80))
                    return true;
            }

            foreach (Zombie vZombie in _level.ZombieGroup.ZombieList)
            {
                if (MyMathHelper.IsIntersection(BoundingSphere, vZombie.BoundingSphere, 80))
                    return true;
            }

            return false;
        }

        private void Wander(Vector3 position, ref Vector3 wanderDirection,
            ref float orientation, float turnSpeed)
        {
            wanderDirection.X +=
                MathHelper.Lerp(-.25f, .25f, MyMathHelper.GetRandomFloat());
            wanderDirection.Z +=
                MathHelper.Lerp(-.25f, .25f, MyMathHelper.GetRandomFloat());

            if (wanderDirection != Vector3.Zero)
            {
                wanderDirection.Normalize();
            }

            orientation = TurnToFace(position, position + wanderDirection, orientation,
                .25f * turnSpeed);

            Vector3 screenCenter = _level.Player.Position;

            float distanceFromScreenCenter = Vector3.Distance(screenCenter, position);
            float MaxDistanceFromScreenCenter =
                Math.Min(screenCenter.Z, screenCenter.X);

            float normalizedDistance =
                distanceFromScreenCenter / MaxDistanceFromScreenCenter;

            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance *
                turnSpeed;

            orientation = TurnToFace(position, screenCenter, orientation,
                turnToCenterSpeed);
        }

        private static float TurnToFace(Vector3 position, Vector3 faceThis,
            float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float z = faceThis.Z - position.Z;

            float desiredAngle = (float)Math.Atan2(z, x);

            float difference = WrapAngle(desiredAngle - currentAngle);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return WrapAngle(currentAngle + difference);
        }

        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        private void LoadEntities(ContentManager content, Camera camera, GraphicsDevice device)
        {
            _bloodParticles = new BloodParticleSystem(device, content, camera,
                @"level 1\textures\green blood");
            _visibleEntities.Add(_bloodParticles);
        }

        #endregion
    }
}


