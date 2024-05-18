using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public class TiledMapPropertyContent
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string ValueAttribute { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "propertytype")]
        public string PropertyType { get; set; }

        [XmlText]
        public string ValueBody { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TiledMapPropertyContent> Properties { get; set; }

        public string Value => ValueAttribute ?? ValueBody;

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}
