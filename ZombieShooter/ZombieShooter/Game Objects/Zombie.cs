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
    public class Zombie : AnimatedModel
    {
        #region enumeration

        public enum ZombieAiState
        {
            Chasing,
            Caught,
            Wander,
            Die
        }

        #endregion

        #region constant

        const float MaxZombieSpeed = 4.0f;
        const float ZombieTurnSpeed = 0.10f;
        const float ZombieChaseDistance = 800.0f;
        const float ZombieCaughtDistance = 150.0f;
        const float ZombieHysteresis = 15.0f;

        #endregion

        #region fields

        public ZombieAiState _zombieState = ZombieAiState.Wander;
        float _zombieOrientation;
        Vector3 _zombieWanderDirection;
        public int ZombieHP;
        public int ZombieDam;
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

        public Zombie(ContentManager content, Camera camera, GraphicsDevice device, Level_1 lvl,
            Vector3 position, Vector3 scale, Vector3 rotation)
            : base(@"level 1\models\zombie\new_thin_zombie", position, rotation, scale, camera, device)
        {
            _level = lvl;
            ZombieHP = Global.ZombieHP;
            ZombieDam = Global.ZombieDam;

            LoadContent(content);
            SetAnim(this, 17);

            LoadEntities(lvl.Content, camera, device);
        }

        #endregion

        #region update

        public override void Update(GameTime gameTime)
        {
            if (_zombieState == ZombieAiState.Die)
            {
                countTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (countTime >= 1.5)
                {
                    if (rand.Next(5) == 0)
                    {
                        int type = rand.Next(4) + 1;
                        Product product = CreateProduct(type);
                        _level.ProductGroup.AddProduct(product);
                    }
                    _level.ZombieGroup.KillZombie(this);
                    _level.ZombieGroup.AddZombie();
                }
            }
            else
            {
                float zombieChaseThreshold = ZombieChaseDistance;
                float zombieCaughtThreshold = ZombieCaughtDistance;

                if (_zombieState == ZombieAiState.Wander)
                {
                    zombieChaseThreshold -= ZombieHysteresis / 2;
                }
                else if (_zombieState == ZombieAiState.Chasing)
                {
                    zombieChaseThreshold += ZombieHysteresis / 2;
                    zombieCaughtThreshold -= ZombieHysteresis / 2;
                }
                else if (_zombieState == ZombieAiState.Caught)
                {
                    zombieCaughtThreshold += ZombieHysteresis / 2;
                }

                float distanceFromCat = Vector3.Distance(Position, _level.Player.Position);
                if (distanceFromCat > zombieChaseThreshold)
                {
                    if (_zombieState != ZombieAiState.Wander) SetAnim(this, 17);
                    _zombieState = ZombieAiState.Wander;
                }
                else if (distanceFromCat > zombieCaughtThreshold)
                {
                    if (_zombieState != ZombieAiState.Chasing) SetAnim(this, 15);
                    _zombieState = ZombieAiState.Chasing;
                }
                else
                {
                    countTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (countTime >= 1)
                    {
                        SoundAttack();
                        countTime = -0.6f;
                        _level.Player.AttackPlayer(ZombieDam);
                    }

                    if (_zombieState != ZombieAiState.Caught) SetAnim(this, 4);
                    _zombieState = ZombieAiState.Caught;              
                }

                float currentZombieSpeed;
                if (_zombieState == ZombieAiState.Chasing)
                {
                    _zombieOrientation = TurnToFace(Position, _level.Player.Position, _zombieOrientation,
                        ZombieTurnSpeed);
                    currentZombieSpeed = MaxZombieSpeed;
                }
                else if (_zombieState == ZombieAiState.Wander)
                {
                    Wander(Position, ref _zombieWanderDirection, ref _zombieOrientation,
                        ZombieTurnSpeed);
                    currentZombieSpeed = .5f * MaxZombieSpeed;
                }
                else
                {
                    currentZombieSpeed = 0.0f;
                }

                Vector3 heading = new Vector3(
                    (float)Math.Cos(_zombieOrientation), 0, (float)Math.Sin(_zombieOrientation));
                Position = MoveZombie(Position, heading * currentZombieSpeed);

                float rY = WrapAngle(-_zombieOrientation);
                Rotation = new Vector3(Rotation.X, rY, Rotation.Z);
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
                _bloodParticles.AddParticle(Position - shotVector * 20 + new Vector3(0, 100, 0), Vector3.Zero);
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
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\monster_death");
                soundEffect.Play();
            }
        }

        public void SoundAttack()
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\monster1_attack");
                soundEffect.Play();
            }
        }

        public void ZombieDie()
        {
            _shooted = true;
            ZombieHP -= _level.Player.Gun.GunDam;
            if (ZombieHP <= 0)
            {
                _zombieState = ZombieAiState.Die;
                countTime = 0;
                SetAnim(this, 7);
                SoundDeath();
                _level.Player.AddMoney(Global.ZombieMoney);
            }
        }

        private Vector3 MoveZombie(Vector3 currPos, Vector3 moveAmt)
        {
            Vector3 foreMove = currPos + moveAmt;

            if (_level.Terrain.DetermineTerrainType(foreMove.X, foreMove.Z) == Terrain.TerrainType.Ground)
            {
                Position = foreMove;
                if ((_level.Terrain.CheckCollision(foreMove.X, foreMove.Z, this)) || (ZombieCheckCollision()))
                    return currPos;
                else
                    return foreMove;
            }
            else
                return currPos;
        }

        private bool ZombieCheckCollision()
        {
            foreach (Zombie vZombie in _level.ZombieGroup.ZombieList)
            {
                if (vZombie != this)
                    if (MyMathHelper.IsIntersection(BoundingSphere, vZombie.BoundingSphere, 80))
                        return true;
            }

            foreach (Spider vSpider in _level.SpiderGroup._spiderGroup)
            {
                if (MyMathHelper.IsIntersection(BoundingSphere, vSpider.BoundingSphere, 80))
                    return true;
            }

            foreach (Monster vMonster in _level.MonsterGroup.MonsterList)
            {
                if (MyMathHelper.IsIntersection(BoundingSphere, vMonster.BoundingSphere, 80))
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
