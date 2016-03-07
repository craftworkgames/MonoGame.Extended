using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxPolyline
    {
        [XmlAttribute(AttributeName = "points")]
        public string Points { get; set; }
    }
}