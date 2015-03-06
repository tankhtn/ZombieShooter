using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CommonLibrary;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace ZombieShooter
{
    public class Gun : CModel
    {
        #region fields

        protected Player _player;
        protected Level_1 _level;
        protected Vector3 _gunPosition;
        protected int _gunDam;

        protected bool _shooting = false;

        protected List<IVisibleGameEntity> _visibleEntities = new List<IVisibleGameEntity>();

        #endregion

        #region properties

        public Vector3 GunPosition { get{ return _gunPosition;} }
        public int GunDam { get { return _gunDam; } }

        #endregion

        #region construction

        public Gun(Level_1 lvl, Model model, Texture2D texture, 
            Vector3 rot, Vector3 scale, Camera camera, GraphicsDevice device, Player player)
            : base(model, new Vector3(0,0,0), rot, scale, camera, device)
        {
            _player = player;
            _level = lvl;
            TeturingModel(texture);

            LoadEntities(lvl.Content, camera, device);
        }

        public Gun(Level_1 lvl, Model model, Vector3 rot, Vector3 scale,
            Camera camera, GraphicsDevice device, Player player)
            : this(lvl, model, null, rot, scale, camera, device, player)
        {
        }

        #endregion

        #region update

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            AbsoluteWorld = _player.FindBone("L_Index1").AbsoluteTransform * _player.BaseWorld;
            _gunPosition = AbsoluteWorld.Value.Translation;

            UpdateParticles(gameTime);

            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.Update(gameTime);
            }
        }

        protected virtual void UpdateParticles(GameTime gameTime)
        {
        }

        public void SoundShot(int type)
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                if (type == 1) soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\pistol_shot");
                else soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\nero_shot");
                soundEffect.Play();
            }
        }

        public void SoundShotEmpty()
        {
            if (Global.isMusic)
            {
                SoundEffect soundEffect;
                soundEffect = Global.Content.Load<SoundEffect>(@"music\wav\mouse_error");
                soundEffect.Play();
            }
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            if (input.IsLeftButtonPressed())
            {
                if (((_level.Player.TypeGun == 2) && (_level.Player._rpgGun.nBullet <= 0))) SoundShotEmpty();
                if ((_level.Player.TypeGun == 1) || ((_level.Player.TypeGun == 2) && (_level.Player._rpgGun.nBullet > 0)))
                {
                    SoundShot(_level.Player.TypeGun);

                    _shooting = true;

                    Vector3 shotVector = _level.Crosshair.Position - _gunPosition;
                    shotVector.Normalize();

                    HandleShooting(shotVector);

                    Ray orientShoot = new Ray(_gunPosition, _level.Crosshair.Position - _gunPosition);                
                    float minDistanceZombie = float.MaxValue;

                    Zombie zombieDie = null;
                    foreach (Zombie zombie in _level.ZombieGroup.ZombieList)
                    {
                        if (zombie._zombieState != Zombie.ZombieAiState.Die)
                        if ((MyMathHelper.RayIsIntersection(orientShoot, zombie.BoundingSphere)))
                            if (Vector3.Distance(_gunPosition, zombie.Position) < minDistanceZombie)
                            {
                                zombieDie = zombie;
                                minDistanceZombie = Vector3.Distance(_gunPosition, zombie.Position);
                            }
                    }

                    orientShoot = new Ray(_gunPosition - new Vector3(0, 100, 0), _level.Crosshair.Position - _gunPosition);
                    float minDistanceSpider = float.MaxValue;
                    Spider spiderDie = null;
                    foreach (Spider spider in _level.SpiderGroup._spiderGroup)
                    {
                        if (spider._SpiderState != Spider.SpiderAiState.Die)
                        if ((MyMathHelper.RayIsIntersection(orientShoot, spider.BoundingSphere)))
                            if (Vector3.Distance(_gunPosition, spider.Position) < minDistanceSpider)
                            {
                                spiderDie = spider;
                                minDistanceSpider = Vector3.Distance(_gunPosition, spider.Position);
                            }
                    }

                    orientShoot = new Ray(_gunPosition - new Vector3(0, 100, 0), _level.Crosshair.Position - _gunPosition);
                    float minDistanceMonster = float.MaxValue;
                    Monster monsterDie = null;
                    foreach (Monster monster in _level.MonsterGroup.MonsterList)
                    {
                        if (monster._MonsterState != Monster.MonsterAiState.Die)
                        if ((MyMathHelper.RayIsIntersection(orientShoot, monster.BoundingSphere)))
                            if (Vector3.Distance(_gunPosition, monster.Position) < minDistanceMonster)
                            {
                                monsterDie = monster;
                                minDistanceMonster = Vector3.Distance(_gunPosition, monster.Position);
                            }
                    }

                    int type = 1;
                    float min = minDistanceZombie;
                    if (minDistanceSpider < min)
                    {
                        type = 2;
                        min = minDistanceSpider;
                    }
                    if (minDistanceMonster < min)
                    {
                        type = 3;
                        min = minDistanceMonster;
                    }
                    switch (type)
                    {
                        case 1:
                            if (zombieDie != null) zombieDie.ZombieDie();
                            break;
                        case 2:
                            if (spiderDie != null) spiderDie.SpiderDie();
                            break;
                        case 3:
                            if (monsterDie != null) monsterDie.MonsterDie();
                            break;
                    }
                }
            }

            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.HandleInput(input);
            }
        }

        protected virtual void HandleShooting(Vector3 shotVector)
        {
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
        }

        protected override void DrawBoundingSphere()
        {
        }

        #endregion

        #region Helper

        private void TeturingModel(Texture2D texture)
        {
            if (texture == null)
                return;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Texture = texture;
                    effect.TextureEnabled = true;
                    effect.EnableDefaultLighting();
                }
            }
        }

        protected virtual void LoadEntities(ContentManager content, 
            Camera camera, GraphicsDevice device)
        {
        }

        #endregion
    }
}
