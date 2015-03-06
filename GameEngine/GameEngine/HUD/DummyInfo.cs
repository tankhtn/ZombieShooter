using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public class DummyInfo : IVisibleGameEntity
    {
        List<IVisibleGameEntity> _visibleEntities = new List<IVisibleGameEntity>();

        UITextBlock _renderedPatchesCnt;

        public int TotalPatches { get; set; }
        public int RenderedPatches { get; set; }

        public DummyInfo(ContentManager content, SpriteBatch spriteBatch)
        {
            TotalPatches = 0;
            RenderedPatches = 0;

            _renderedPatchesCnt = new UITextBlock("tb", new Vector2(10), "",
                content.Load<SpriteFont>(@"fonts\HUDFont"), Color.White, spriteBatch);
            _visibleEntities.Add(_renderedPatchesCnt);
        }

        public void Update(GameTime gameTime)
        {
            _renderedPatchesCnt.Text = string.Format("Rendered {0}/{1} patches", RenderedPatches, TotalPatches);

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
