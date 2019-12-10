using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public class TiledMapPropertyContent
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string ValueAttribute { get; set; }

        [XmlText]
        public string ValueBody { get; set; }

        public string Value => ValueAttribute ?? ValueBody;

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}