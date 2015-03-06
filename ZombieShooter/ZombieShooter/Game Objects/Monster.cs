using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary;
using Microsoft.Xna.Framework.Audio;

namespace ZombieShooter
{
    public class Monster : AnimatedModel
    {
        #region enumeration

        public enum MonsterAiState
        {
            Chasing,
            Caught,
            Wander,
            Die
        }

        #endregion

        #region constant

        const float MaxMonsterSpeed = 6.0f;
        const float MonsterTurnSpeed = 0.10f;
        const float MonsterChaseDistance = 800.0f;
        const float MonsterCaughtDistance = 150.0f;
        const float MonsterHysteresis = 15.0f;

        #endregion

        #region fields

        public MonsterAiState _MonsterState = MonsterAiState.Wander;
        float _MonsterOrientation;
        Vector3 _MonsterWanderDirection;
        public int MonsterHP;
        public int MonsterDam;
        Random rand = new Random();

        BloodParticleSystem _bloodParticles;
        float _bloodParticlesLife = 0.1f;
        float _bloodParticlesAge = 0;
        bool _shooted = false;

        List<IVisibleGameEntity> _visibleEntities = new List<IVisibleGameEntity>();

        Level_1 _level;

        float countTime = 0;

        #endregion

        #region construction

        public Monster(ContentManager content, Camera camera, GraphicsDevice device, Level_1 lvl,
            Vector3 position, Vector3 scale, Vector3 rotation)
            : base(@"level 1\models\Slug_Like monster\SLUG_Like_Monster", position, rotation, scale, camera, device)
        {
            _level = lvl;
            MonsterHP = Global.MonsterHP;
            MonsterDam = Global.MonsterDam;

            LoadContent(content);
            SetAnim(this, 0);

            LoadEntities(lvl.Content, camera, device);
        }

        #endregion

        #region update

        public override void Update(GameTime gameTime)
        {
            if (_MonsterState == MonsterAiState.Die)
            {
                countTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (countTime >= 1.5)
                {
                    if (rand.Next(3) == 0)
                    {
                        int type = rand.Next(4) + 1;
                        Product product = CreateProduct(type);
                        _level.ProductGroup.AddProduct(product);
                    }
                    _level.MonsterGroup.KillMonster(this);
                    _level.MonsterGroup.AddMonster();
                }
            }
            else
            {
                float MonsterChaseThreshold = MonsterChaseDistance;
                float MonsterCaughtThreshold = MonsterCaughtDistance;

                if (_MonsterState == MonsterAiState.Wander)
                {
                    MonsterChaseThreshold -= MonsterHysteresis / 2;
                }
                else if (_MonsterState == MonsterAiState.Chasing)
                {
                    MonsterChaseThreshold += MonsterHysteresis / 2;
                    MonsterCaughtThreshold -= MonsterHysteresis / 2;
                }
                else if (_MonsterState == MonsterAiState.Caught)
                {
                    MonsterCaughtThreshold += MonsterHysteresis / 2;
                }

                float distanceFromCat = Vector3.Distance(Position, _level.Player.Position);
                if (distanceFromCat > MonsterChaseThreshold)
                {
                    if (_MonsterState != MonsterAiState.Wander) SetAnim(this, 0);
                    _MonsterState = MonsterAiState.Wander;
                }
                else if (distanceFromCat > MonsterCaughtThreshold)
                {
                    if (_MonsterState != MonsterAiState.Chasing) SetAnim(this, 0);
                    _MonsterState = MonsterAiState.Chasing;
                }
                else
                {
                    countTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (countTime >= 1.5)
                    {
                        SoundAttack();
                        countTime = -2.5f;
                        _level.Player.AttackPlayer(MonsterDam);
                    }

                    if (_MonsterState != MonsterAiState.Caught) SetAnim(this, 0);
                    _MonsterState = MonsterAiState.Caught;                  
                }

                float currentMonsterSpeed;
                if (_MonsterState == MonsterAiState.Chasing)
                {
                    _MonsterOrientation = TurnToFace(Position, _level.Player.Position, _MonsterOrientation,
                        MonsterTurnSpeed);
                    currentMonsterSpeed = MaxMonsterSpeed;
                }
                else if (_MonsterState == MonsterAiState.Wander)
                {
                    Wander(Position, ref _MonsterWanderDirection, ref _MonsterOrientation,
                        MonsterTurnSpeed);
                    currentMonsterSpeed = .5f * MaxMonsterSpeed;
                }
                else
                {
                    currentMonsterSpeed = 0.0f;
                }

                Vector3 heading = new Vector3(
                    (float)Math.Cos(_MonsterOrientation), 0, (float)Math.Sin(_MonsterOrientation));
                Position = MoveMonster(Position, heading * currentMonsterSpeed);

                float rY = WrapAngle(-_MonsterOrientation);
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
                _bloodParticles.AddParticle(Position - shotVector * 30 + new Vector3(0, 100, 0), Vector3.Zero);
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
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\monster_death2");
                soundEffect.Play();
            }
        }

        public void SoundAttack()
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\monster2_attack");
                soundEffect.Play();
            }
        }

        public void MonsterDie()
        {
            _shooted = true;
            MonsterHP -= _level.Player.Gun.GunDam;
            if (MonsterHP <= 0)
            {
                _MonsterState = MonsterAiState.Die;
                countTime = 0;
                SetAnim(this, 0);
                Position += new Vector3(0, 0, 0);
                Rotation += new Vector3(0, 0, (float)Math.PI / 2.0f - 0.5f);
                SoundDeath();
                _level.Player.AddMoney(Global.MonsterMoney);
            }
        }

        private Vector3 MoveMonster(Vector3 currPos, Vector3 moveAmt)
        {
            Vector3 foreMove = currPos + moveAmt;

            if (_level.Terrain.DetermineTerrainType(foreMove.X, foreMove.Z) == Terrain.TerrainType.Ground)
            {
                Position = foreMove;
                if ((_level.Terrain.CheckCollision(foreMove.X, foreMove.Z, this)) || (MonsterCheckCollision()))
                    return currPos;
                else
                    return foreMove;
            }
            else
                return currPos;
        }

        private bool MonsterCheckCollision()
        {
            foreach (Monster vMonster in _level.MonsterGroup.MonsterList)
            {
                if (vMonster != this)
                    if (MyMathHelper.IsIntersection(BoundingSphere, vMonster.BoundingSphere, 80))
                        return true;
            }

            foreach (Spider vSpider in _level.SpiderGroup._spiderGroup)
            {
                if (MyMathHelper.IsIntersection(BoundingSphere, vSpider.BoundingSphere, 80))
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
                @"level 1\textures\blood2");
            _visibleEntities.Add(_bloodParticles);
        }

        #endregion
    }
}
