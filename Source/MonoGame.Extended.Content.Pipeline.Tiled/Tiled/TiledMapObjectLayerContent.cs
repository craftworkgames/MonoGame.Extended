using System.Collections.Generic;
using System.Xml.Serialization;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapObjectLayerContent : TiledMapLayerContent
    {
        [XmlAttribute(AttributeName = "color")]
        public string Color { get; set; }

        [XmlElement(ElementName = "object")]
        public List<TiledMapObjectContent> Objects { get; set; }

        [XmlAttribute(AttributeName = "draworder")]
        public TiledMapObjectDrawOrderContent DrawOrder { get; set; }

        public TiledMapObjectLayerContent()
            : base(TiledMapLayerType.ObjectLayer)
        {
            Objects = new List<TiledMapObjectContent>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}