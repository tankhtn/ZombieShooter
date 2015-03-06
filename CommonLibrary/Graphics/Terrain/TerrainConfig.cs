using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CommonLibrary.Graphics
{
    public class TerrainConfig
    {
        public Texture2D HeightMap { get; set; }
        public Texture2D BaseTexture { get; set; }
        public Texture2D RockTexture { get; set; }
        public Texture2D WaterTexture { get; set; }
        public float CellSize { get; set; }
        public int PatchSize { get; set; }
        public float MaxHeight { get; set; }
        public Vector3 BaseColor { get; set; }
        public float TextureTiling { get; set; }
        public Effect Effect { get; set; }
        public Effect WeightMapEffect { get; set; }
        public Camera Camera { get; set; }
        public GraphicsDevice Device { get; set; }
    }
}
