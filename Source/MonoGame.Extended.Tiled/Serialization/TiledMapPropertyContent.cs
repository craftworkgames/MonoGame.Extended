using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public class TiledMapPropertyContent
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}