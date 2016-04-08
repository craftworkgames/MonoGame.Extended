using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxPolygon
    {
        [XmlAttribute(AttributeName = "points")]
        public string Points { get; set; }
    }
}