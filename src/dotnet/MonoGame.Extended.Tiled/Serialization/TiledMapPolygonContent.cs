using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public class TiledMapPolygonContent
    {
        [XmlAttribute(AttributeName = "points")]
        public string Points { get; set; }
    }
}