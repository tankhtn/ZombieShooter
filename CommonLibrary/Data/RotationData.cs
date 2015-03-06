using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace CommonLibrary
{
    public class RotationData : IData
    {
        public Vector3 Transform { get; set; }

        public XElement Element
        {
            get
            {
                XElement element = new XElement("Rotation");

                element.Add(new XAttribute("x", Transform.X));
                element.Add(new XAttribute("y", Transform.Y));
                element.Add(new XAttribute("z", Transform.Z));

                return element;
            }
        }
    }
}
