using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework;
using CommonLibrary.Graphics;

namespace ZombieShooter
{
    public class ZombieGroup : IVisibleGameEntity
    {
        #region fields

        int _nZombie;
        Level_1 _level;
        Random _rand = new Random();

        #endregion

        #region Properties

        public List<Zombie> ZombieList { get; set; }
        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region construction

        public ZombieGroup(int nZombie, Level_1 lvl)
        {
            _level = lvl;
            _nZombie = nZombie;
            ZombieList = new List<Zombie>();
            for (int i = 0; i < _nZombie; i++) AddZombie();                  
        }

        #endregion

        #region update

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < ZombieList.Count; i++)
                ZombieList[i].Update(gameTime);
        }

        public void HandleInput(InputManager input)
        {
        }

        #endregion

        #region draw

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < ZombieList.Count; i++)
                ZombieList[i].Draw(gameTime);
        }

        #endregion

        #region helper

        public void AddZombie()
        {
            while (true)
            {
                float x = _level.Player.Position.X + _rand.Next(1500) - _rand.Next(1500);
                float z = _level.Player.Position.Z + _rand.Next(1000) - _rand.Next(1000);
                Zombie zombie = new Zombie(_level.Content, _level.Camera, _level.Device, _level,
                        new Vector3(x, _level.Player.Position.Y, z), new Vector3(20), Vector3.Zero);
                if (_level.Terrain.DetermineTerrainType(x, z) == Terrain.TerrainType.Ground)
                {
                    if ((!_level.Terrain.CheckCollision(x, z, zombie)) && (!ZombieGroupCheckCollision(zombie)))
                    {
                        ZombieList.Add(zombie);
                        break;
                    }
                }
            }
        }

        private bool ZombieGroupCheckCollision(Zombie zombie)
        {
            foreach (Zombie vZombie in ZombieList)
            {
                if (MyMathHelper.IsIntersection(zombie.BoundingSphere, vZombie.BoundingSphere, 80))
                    return true;
            }

            if (_level.SpiderGroup != null)
            foreach (Spider vSpider in _level.SpiderGroup._spiderGroup)
            {
                if (MyMathHelper.IsIntersection(zombie.BoundingSphere, vSpider.BoundingSphere, 80))
                    return true;
            }

            if (_level.MonsterGroup != null)
            foreach (Monster vMonster in _level.MonsterGroup.MonsterList)
            {
                if (MyMathHelper.IsIntersection(zombie.BoundingSphere, vMonster.BoundingSphere, 80))
                    return true;
            }

            return false;
        }

        public void KillZombie(Zombie zombie)
        {
            ZombieList.Remove(zombie);
        }

        #endregion
    }
}
