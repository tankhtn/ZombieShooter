using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CommonLibrary.Graphics
{
    public class PostProcessor
    {
        public Texture2D Input { get; set; }
        public Effect Effect { get { return effect; } }

        protected Effect effect;
        protected GraphicsDevice graphicsDevice;
        protected static SpriteBatch spriteBatch;

        Texture2D _bloodTexture;

        public PostProcessor(ContentManager content, GraphicsDevice graphicsDevice)
        {
            effect = content.Load<Effect>(@"level 1\effects\PostProcessingEffect");
            _bloodTexture = content.Load<Texture2D>(@"level 1\textures\die blood");

            if (spriteBatch == null)
                spriteBatch = new SpriteBatch(graphicsDevice);
            this.graphicsDevice = graphicsDevice;
        }

        public virtual void Draw(string technique)
        {
            EffectHelper.SetEffectParameter(effect, "BloodTexture", _bloodTexture);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            if (technique != null)
                effect.CurrentTechnique = effect.Techniques[technique];
            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(Input, Vector2.Zero, Color.White);

            spriteBatch.End();

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}
