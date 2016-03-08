using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxObjectGroup
    {
        [XmlAttribute(AttributeName = "color")]
        public string Color { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "object")]
        public List<TmxObject> Objects { get; set; }

        [XmlAttribute(AttributeName = "opacity")]
        public float Opacity { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TmxProperty> Properties { get; set; }

        [XmlAttribute(AttributeName = "visible")]
        public bool Visible { get; set; }

        public TmxObjectGroup()
        {
            Opacity = 1.0f;
            Visible = true;
            Properties = new List<TmxProperty>();
            Objects = new List<TmxObject>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}