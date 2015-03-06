using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonLibrary.Graphics
{
    public class MeshTag
    {
        public Vector3 Color;
        public Texture2D Texture;
        public float SpecularPower;
        public Effect CachedEffect = null;

        public MeshTag(Vector3 color, Texture2D texture, float specularPower)
        {
            Color = color;
            Texture = texture;
            SpecularPower = specularPower;
        }
    }
}
