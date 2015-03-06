using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonLibrary
{
    public interface ICollidable
    {
        BoundingSphere BoundingSphere { get; }
    }
}
