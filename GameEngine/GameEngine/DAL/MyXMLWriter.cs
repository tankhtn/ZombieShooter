using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CommonLibrary;

namespace GameEngine
{
    public class MyXMLWriter
    {
        XDocument _doc;
        XElement _root;

        public MyXMLWriter(List<Building> buildings)
        {
            _doc = new XDocument();
            _root = new XElement("Buildings");

            foreach (Building building in buildings)
            {
                BuildingData buildingData = new BuildingData()
                {
                    Name = building.Name,
                    Position = new PositionData() { Transform = building.Position },
                    Rotation = new RotationData() { Transform = building.Rotation },
                    Scale = new ScaleData() { Transform = building.Scale }
                };

                _root.Add(buildingData.Element);
            }
        }

        public void Save(string name)
        {
            _doc.Add(_root);
            _doc.Save(name);
        }
    }
}
