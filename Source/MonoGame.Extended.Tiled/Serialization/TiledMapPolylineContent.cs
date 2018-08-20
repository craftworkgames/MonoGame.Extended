using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public class TiledMapPolylineContent
    {
        [XmlAttribute(AttributeName = "points")]
        public string Points { get; set; }
    }
}