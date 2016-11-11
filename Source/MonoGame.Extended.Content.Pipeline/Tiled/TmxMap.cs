using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [XmlRoot(ElementName = "map")]
    public class TmxMap
    {
        public TmxMap()
        {
            Properties = new List<TmxProperty>();
            Tilesets = new List<TmxTileset>();
            Layers = new List<TmxLayer>();
        }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "orientation")]
        public TmxOrientation Orientation { get; set; }

        [XmlAttribute(AttributeName = "renderorder")]
        public TmxRenderOrder RenderOrder { get; set; }

        [XmlAttribute(AttributeName = "backgroundcolor")]
        public string BackgroundColor { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }

        [XmlAttribute(AttributeName = "tilewidth")]
        public int TileWidth { get; set; }

        [XmlAttribute(AttributeName = "tileheight")]
        public int TileHeight { get; set; }

        [XmlElement(ElementName = "tileset")]
        public List<TmxTileset> Tilesets { get; set; }

        [XmlElement(ElementName = "layer", Type = typeof(TmxTileLayer))]
        [XmlElement(ElementName = "imagelayer", Type = typeof(TmxImageLayer))]
        [XmlElement(ElementName = "objectgroup", Type = typeof(TmxObjectLayer))]
        public List<TmxLayer> Layers { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TmxProperty> Properties { get; set; }
    }
}