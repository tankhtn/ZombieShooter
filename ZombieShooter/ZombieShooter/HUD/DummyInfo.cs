using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CommonLibrary.Graphics;

namespace ZombieShooter
{
    public class DummyInfo : IVisibleGameEntity
    {
        List<IVisibleGameEntity> _visibleEntities = new List<IVisibleGameEntity>();

        UITextBlock _renderedPatchesCnt;

        string _terrainType = "";

        public int TotalPatches { get; set; }
        public int RenderedPatches { get; set; }
        public Terrain.TerrainType TerrainType
        {
            set
            {
                _terrainType = value.ToString();
            }
        }

        public DummyInfo(ContentManager content, SpriteBatch spriteBatch)
        {
            TotalPatches = 0;
            RenderedPatches = 0;

            _renderedPatchesCnt = new UITextBlock("tb", new Vector2(10), "",
                content.Load<SpriteFont>(@"level 1\fonts\HUDFont"), Color.White, spriteBatch);
            _visibleEntities.Add(_renderedPatchesCnt);
        }

        public void Update(GameTime gameTime)
        {
            StringBuilder temp = new StringBuilder("");
            temp.Append(string.Format("Rendered {0}/{1} patches", RenderedPatches, TotalPatches));
            temp.Append("\n");
            temp.Append(string.Format("Player in {0}", _terrainType));

            _renderedPatchesCnt.Text = temp.ToString();

            foreach (IVisibleGameEntity entity in _visibleEntities)
                entity.Update(gameTime);
        }

        public void HandleInput(InputManager input)
        {
            foreach (IVisibleGameEntity entity in _visibleEntities)
                entity.HandleInput(input);
        }

        public void Draw(GameTime gameTime)
        {
            foreach (IVisibleGameEntity entity in _visibleEntities)
                entity.Draw(gameTime);
        }

        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }
    }
}
