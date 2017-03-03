using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapTerrainContent
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "tile")]
        public string TileId { get; set; }

        [XmlArray("properties"), XmlArrayItem("property")]
        public List<TiledMapPropertyContent> Properties { get; set; }

        public TiledMapTerrainContent()
        {
            Properties = new List<TiledMapPropertyContent>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}