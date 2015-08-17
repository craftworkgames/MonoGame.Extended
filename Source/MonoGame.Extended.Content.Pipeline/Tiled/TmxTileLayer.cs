using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxTileLayer : TmxLayer
    {
        public TmxTileLayer()
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
        public TmxData Data { get; set; }
    }
}