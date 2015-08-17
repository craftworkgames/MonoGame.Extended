using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxProperty
    {
        public TmxProperty()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, Value);
        }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}