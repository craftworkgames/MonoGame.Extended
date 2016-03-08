using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxTerrain
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TmxProperty> Properties { get; set; }

        [XmlAttribute(AttributeName = "tile")]
        public string TileId { get; set; }

        public TmxTerrain()
        {
            Properties = new List<TmxProperty>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}