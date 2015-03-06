using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CommonLibrary.Graphics
{
    public class RenderCapture
    {
        RenderTarget2D renderTarget;
        GraphicsDevice graphicsDevice;

        public RenderCapture(GraphicsDevice GraphicsDevice)
        {
            this.graphicsDevice = GraphicsDevice;

            renderTarget = new RenderTarget2D(GraphicsDevice,
                GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, SurfaceFormat.Color, DepthFormat.Depth24);
        }

        public void Begin()
        {
            graphicsDevice.SetRenderTarget(renderTarget);
        }

        public void End()
        {
            graphicsDevice.SetRenderTarget(null);
        }

        public Texture2D GetTexture()
        {
            return renderTarget;
        }
    }
}
