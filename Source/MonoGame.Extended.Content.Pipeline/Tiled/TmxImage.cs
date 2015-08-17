using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxImage
    {
        public TmxImage()
        {
        }

        public override string ToString()
        {
            return Source;
        }

        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }

        [XmlAttribute(AttributeName = "format")]
        public string Format { get; set; }

        [XmlAttribute(AttributeName = "trans")]
        public string Trans { get; set; }

        [XmlElement(ElementName = "data")]
        public TmxData Data { get; set; }
    }
}