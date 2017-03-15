using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [XmlRoot(ElementName = "map")]
    public class TiledMapContent
    {
        [XmlIgnore]
        public string Name { get; set; }

        [XmlIgnore]
        public string FilePath { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "orientation")]
        public TiledMapOrientationContent Orientation { get; set; }

        [XmlAttribute(AttributeName = "renderorder")]
        public TiledMapTileDrawOrderContent RenderOrder { get; set; }

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
        public List<TiledMapTilesetContent> Tilesets { get; set; }

        [XmlElement(ElementName = "layer", Type = typeof(TiledMapTileLayerContent))]
        [XmlElement(ElementName = "imagelayer", Type = typeof(TiledMapImageLayerContent))]
        [XmlElement(ElementName = "objectgroup", Type = typeof(TiledMapObjectLayerContent))]
        public List<TiledMapLayerContent> Layers { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TiledMapPropertyContent> Properties { get; set; }

        public TiledMapContent()
        {
            Properties = new List<TiledMapPropertyContent>();
            Tilesets = new List<TiledMapTilesetContent>();
            Layers = new List<TiledMapLayerContent>();
        }
    }
}