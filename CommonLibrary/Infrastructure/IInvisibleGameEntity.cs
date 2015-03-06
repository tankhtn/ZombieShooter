using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonLibrary
{
    public interface IInvisibleGameEntity
    {
        void Update(GameTime gameTime);
        void HandleInput(InputManager input);
    }
}
