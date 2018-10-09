using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public class TiledMapObjectLayerContent : TiledMapLayerContent
    {
        public TiledMapObjectLayerContent()
            : base(TiledMapLayerType.ObjectLayer)
        {
            Objects = new List<TiledMapObjectContent>();
        }

        [XmlAttribute(AttributeName = "color")]
        public string Color { get; set; }

        [XmlElement(ElementName = "object")]
        public List<TiledMapObjectContent> Objects { get; set; }

        [XmlAttribute(AttributeName = "draworder")]
        public TiledMapObjectDrawOrderContent DrawOrder { get; set; }
        
        public override string ToString()
        {
            return Name;
        }
    }
}