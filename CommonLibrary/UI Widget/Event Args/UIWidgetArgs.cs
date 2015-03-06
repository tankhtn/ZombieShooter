using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonLibrary
{
    public class UIWidgetArgs : EventArgs
    {
        #region Properties

        public Point Location { get; private set; }
        public string ID { get; private set; }

        #endregion

        #region Constructor

        public UIWidgetArgs(string id, Point location)
        {
            ID = id;
            Location = location;
        }

        #endregion
    }
}
