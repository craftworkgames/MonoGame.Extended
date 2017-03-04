using System.Xml.Serialization;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapTileLayerContent : TiledMapLayerContent
    {
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

        public TiledMapTileLayerContent() 
            : base(TiledMapLayerType.TileLayer)
        {
        }
    }
}