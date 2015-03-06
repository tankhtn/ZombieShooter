using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CommonLibrary
{
    public class BuildingData : IData
    {
        public string Name { get; set; }

        public PositionData Position { get; set; }
        public RotationData Rotation { get; set; }
        public ScaleData Scale { get; set; }

        public XElement Element
        {
            get
            {
                XElement element = new XElement("Building");

                element.Add(new XAttribute("name", Name));
                element.Add(Position.Element);
                element.Add(Rotation.Element);
                element.Add(Scale.Element);

                return element;
            }
        }
    }
}
