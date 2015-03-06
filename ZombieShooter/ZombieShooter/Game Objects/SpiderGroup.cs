using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework;
using CommonLibrary.Graphics;

namespace ZombieShooter
{
    public class SpiderGroup : IVisibleGameEntity
    {
        int nSpider;
        public List<Spider> _spiderGroup;
        Level_1 _level;
        Random _rand = new Random();

        public SpiderGroup(int nSpider, Level_1 lvl)
        {
            _level = lvl;
            this.nSpider = nSpider;
            _spiderGroup = new List<Spider>();

            for (int i = 0; i < nSpider; i++) AddSpider();                 
        }

        public void AddSpider()
        {
            while (true)
            {
                float x = _level.Player.Position.X + _rand.Next(1500) - _rand.Next(1500);
                float z = _level.Player.Position.Z + _rand.Next(1000) - _rand.Next(1000);
                Spider spider = new Spider(_level.Content, _level.Camera, _level.Device, _level,
                    new Vector3(x, _level.Player.Position.Y, z), new Vector3(5), Vector3.Zero);
                if (_level.Terrain.DetermineTerrainType(x, z) == Terrain.TerrainType.Ground)
                {
                    if ((!_level.Terrain.CheckCollision(x, z, spider)) && (!SpiderGroupCheckCollision(spider)))
                    {
                        _spiderGroup.Add(spider);
                        break;
                    }
                }
            }
        }

        private bool SpiderGroupCheckCollision(Spider spider)
        {
            foreach (Spider vSpider in _spiderGroup)
            {
                if (MyMathHelper.IsIntersection(spider.BoundingSphere, vSpider.BoundingSphere, 80))
                    return true;
            }

            if (_level.ZombieGroup != null)
                foreach (Zombie vZombie in _level.ZombieGroup.ZombieList)
                {
                    if (MyMathHelper.IsIntersection(spider.BoundingSphere, vZombie.BoundingSphere, 80))
                        return true;
                }

            if (_level.MonsterGroup != null)
                foreach (Monster vMonster in _level.MonsterGroup.MonsterList)
                {
                    if (MyMathHelper.IsIntersection(spider.BoundingSphere, vMonster.BoundingSphere, 80))
                        return true;
                }

            return false;
        }

        public void killSpider(Spider Spider)
        {
            _spiderGroup.Remove(Spider);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _spiderGroup.Count; i++)
                _spiderGroup[i].Update(gameTime);
        }


        public void HandleInput(InputManager input)
        {
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < _spiderGroup.Count; i++)
                _spiderGroup[i].Draw(gameTime);
        }

        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }
    }
}
