using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxImage
    {
        [XmlElement(ElementName = "data")]
        public TmxData Data { get; set; }

        [XmlAttribute(AttributeName = "format")]
        public string Format { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }

        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "trans")]
        public string Trans { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        public override string ToString()
        {
            return Source;
        }
    }
}