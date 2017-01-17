#region

using System.Xml.Serialization;

#endregion

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapPolygonContent
    {
        [XmlAttribute(AttributeName = "points")]
        public string Points { get; set; }
    }
}