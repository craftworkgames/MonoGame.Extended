using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxObjectLayer : TmxLayer
    {
        public TmxObjectLayer()
        {
            Objects = new List<TmxObject>();
        }

        public override string ToString()
        {
            return Name;
        }

        [XmlAttribute(AttributeName = "color")]
        public string Color { get; set; }

        [XmlElement(ElementName = "object")]
        public List<TmxObject> Objects { get; set; }
    }
}