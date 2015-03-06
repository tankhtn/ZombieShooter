using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace ZombieShooter
{
    public class MyXMLReader
    {
        public List<BuildingData> Buildings { get; private set; }

        public MyXMLReader(Stream stream)
        {
            XDocument doc = XDocument.Load(stream);

            Buildings = (from building in doc.Descendants("Building")
                         select new BuildingData()
                         {
                             Name = building.Attribute("name").Value,
                             Position = (from pos in building.Descendants("Position")
                                         select new PositionData()
                                         {
                                             Transform = new Vector3(
                                                 float.Parse(pos.Attribute("x").Value),
                                                 float.Parse(pos.Attribute("y").Value),
                                                 float.Parse(pos.Attribute("z").Value))
                                         }).FirstOrDefault(),
                             Rotation = (from rot in building.Descendants("Rotation")
                                         select new RotationData()
                                         {
                                             Transform = new Vector3(
                                                 float.Parse(rot.Attribute("x").Value),
                                                 float.Parse(rot.Attribute("y").Value),
                                                 float.Parse(rot.Attribute("z").Value))
                                         }).FirstOrDefault(),
                             Scale = (from scale in building.Descendants("Scale")
                                         select new ScaleData()
                                         {
                                             Transform = new Vector3(
                                                 float.Parse(scale.Attribute("x").Value),
                                                 float.Parse(scale.Attribute("y").Value),
                                                 float.Parse(scale.Attribute("z").Value))
                                         }).FirstOrDefault()
                         }).ToList();
        }
    }
}
