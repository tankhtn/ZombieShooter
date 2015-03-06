using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CommonLibrary
{
    public static class EffectHelper
    {
        public static bool SetEffectParameter(Effect effect, string paramName, object val)
        {
            if (effect.Parameters[paramName] == null)
            {
                return false;
            }

            if (val is Vector3)
                effect.Parameters[paramName].SetValue((Vector3)val);
            else if (val is bool)
                effect.Parameters[paramName].SetValue((bool)val);
            else if (val is Matrix)
                effect.Parameters[paramName].SetValue((Matrix)val);
            else if (val is Texture2D)
                effect.Parameters[paramName].SetValue((Texture2D)val);
            else if (val is float)
                effect.Parameters[paramName].SetValue((float)val);

            return true;
        }
    }
}
