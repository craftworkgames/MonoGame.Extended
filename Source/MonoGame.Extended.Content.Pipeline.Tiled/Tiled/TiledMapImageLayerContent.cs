using System.Collections.Generic;
using System.Xml.Serialization;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapImageLayerContent : TiledMapLayerContent
    {
        [XmlAttribute(AttributeName = "x")]
        public int X { get; set; }

        [XmlAttribute(AttributeName = "y")]
        public int Y { get; set; }

        [XmlElement(ElementName = "image")]
        public TiledMapImageContent Image { get; set; }

        public TiledMapImageLayerContent()
            : base(TiledMapLayerType.ImageLayer)
        {
            Opacity = 1.0f;
            Visible = true;
            Properties = new List<TiledMapPropertyContent>();
        }

        public override string ToString()
        {
            return $"{Name}: {Image}";
        }
    }
}