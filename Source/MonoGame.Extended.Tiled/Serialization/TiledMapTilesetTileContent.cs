using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public class TiledMapTilesetTileContent
    {
        public TiledMapTilesetTileContent()
        {
            Properties = new List<TiledMapPropertyContent>();
            Type = string.Empty;
        }

        [XmlAttribute(AttributeName = "id")]
        public int LocalIdentifier { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "image")]
        public TiledMapImageContent Image { get; set; }

        [XmlArray("objectgroup")]
        [XmlArrayItem("object")]
        public List<TiledMapObjectContent> Objects { get; set; }

        [XmlArray("animation")]
        [XmlArrayItem("frame")]
        public List<TiledMapTilesetTileAnimationFrameContent> Frames { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TiledMapPropertyContent> Properties { get; set; }
        
        public override string ToString()
        {
            return LocalIdentifier.ToString();
        }
    }
}