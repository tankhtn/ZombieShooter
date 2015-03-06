using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework;
using CommonLibrary.Graphics;

namespace ZombieShooter
{
    public class MonsterGroup : IVisibleGameEntity
    {
        #region fields

        int _nMonster;
        Level_1 _level;
        Random _rand = new Random();

        #endregion

        #region Properties

        public List<Monster> MonsterList { get; set; }
        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region construction

        public MonsterGroup(int nMonster, Level_1 lvl)
        {
            _level = lvl;
            _nMonster = nMonster;
            MonsterList = new List<Monster>();

            for (int i = 0; i < _nMonster; i++) AddMonster();                  
        }

        #endregion

        #region update

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < MonsterList.Count; i++)
                MonsterList[i].Update(gameTime);
        }

        public void HandleInput(InputManager input)
        {
        }

        #endregion

        #region draw

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < MonsterList.Count; i++)
                MonsterList[i].Draw(gameTime);
        }

        #endregion

        #region helper

        public void AddMonster()
        {
            while (true)
            {
                float x = _level.Player.Position.X + _rand.Next(1500) - _rand.Next(1500);
                float z = _level.Player.Position.Z + _rand.Next(1000) - _rand.Next(1000);
                Monster monster = new Monster(_level.Content, _level.Camera, _level.Device, _level,
                    new Vector3(x, _level.Player.Position.Y, z), new Vector3(0.7f), Vector3.Zero);
                if (_level.Terrain.DetermineTerrainType(x, z) == Terrain.TerrainType.Ground)
                {
                    if ((!_level.Terrain.CheckCollision(x, z, monster)) && (!MonsterGroupCheckCollision(monster)))
                    {
                        MonsterList.Add(monster);
                        break;
                    }
                }
            }
        }

        private bool MonsterGroupCheckCollision(Monster monster)
        {
            foreach (Monster vMonster in MonsterList)
            {
                if (MyMathHelper.IsIntersection(monster.BoundingSphere, vMonster.BoundingSphere, 80))
                    return true;
            }

            if (_level.ZombieGroup != null)
                foreach (Zombie vZombie in _level.ZombieGroup.ZombieList)
                {
                    if (MyMathHelper.IsIntersection(monster.BoundingSphere, vZombie.BoundingSphere, 80))
                        return true;
                }

            if (_level.SpiderGroup != null)
                foreach (Spider vSpider in _level.SpiderGroup._spiderGroup)
                {
                    if (MyMathHelper.IsIntersection(monster.BoundingSphere, vSpider.BoundingSphere, 80))
                        return true;
                }

            return false;
        }

        public void KillMonster(Monster Monster)
        {
            MonsterList.Remove(Monster);
        }

        #endregion
    }
}

