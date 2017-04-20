using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapTilesetTileContent
    {
        [XmlAttribute(AttributeName = "id")]
        public int LocalIdentifier { get; set; }

        [XmlElement(ElementName = "terrain")]
        public TiledMapTerrainContent Terrain { get; set; }

        [XmlAttribute(AttributeName = "probability")]
        public float Probability { get; set; }

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

        public TiledMapTilesetTileContent()
        {
            Probability = 1.0f;
            Properties = new List<TiledMapPropertyContent>();
        }

        public override string ToString()
        {
            return LocalIdentifier.ToString();
        }
    }
}