using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public class TiledMapTileLayerContent : TiledMapLayerContent
    {
        public TiledMapTileLayerContent()
            : base(TiledMapLayerType.TileLayer)
        {
        }

        [XmlAttribute(AttributeName = "x")]
        public int X { get; set; }

        [XmlAttribute(AttributeName = "y")]
        public int Y { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }

        [XmlElement(ElementName = "data")]
        public TiledMapTileLayerDataContent Data { get; set; }

        [XmlIgnore]
        public TiledMapTile[] Tiles { get; set; }
    }
}