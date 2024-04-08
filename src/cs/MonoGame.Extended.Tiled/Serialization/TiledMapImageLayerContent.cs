using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public class TiledMapImageLayerContent : TiledMapLayerContent
    {
        [XmlAttribute(AttributeName = "x")]
        public int X { get; set; }

        [XmlAttribute(AttributeName = "y")]
        public int Y { get; set; }

        [XmlElement(ElementName = "image")]
        public TiledMapImageContent Image { get; set; }

        [XmlAttribute(AttributeName = "repeatx")]
        public bool RepeatX { get; set; }

        [XmlAttribute(AttributeName = "repeaty")]
        public bool RepeatY { get; set; }

        public TiledMapImageLayerContent()
            : base(TiledMapLayerType.ImageLayer)
        {
            Opacity = 1.0f;
            RepeatX = false;
            RepeatY = false;
            Visible = true;
            Properties = new List<TiledMapPropertyContent>();
        }

        public override string ToString()
        {
            return $"{Name}: {Image}";
        }
    }
}