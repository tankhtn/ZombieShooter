using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonLibrary
{
    public interface IVisibleGameEntity : ICollidable
    {
        void Update(GameTime gameTime);
        void HandleInput(InputManager input);
        void Draw(GameTime gameTime);
    }
}
