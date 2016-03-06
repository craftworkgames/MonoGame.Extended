using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [XmlRoot(ElementName = "tileoffset")]
    public class TmxTileOffset
    {
        public TmxTileOffset()
        {
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }

        [XmlAttribute(AttributeName = "x")]
        public int X { get; set; }

        [XmlAttribute(AttributeName = "y")]
        public int Y { get; set; }
    }
}