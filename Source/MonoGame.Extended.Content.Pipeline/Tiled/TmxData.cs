using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxData
    {
        [XmlAttribute(AttributeName = "compression")]
        public string Compression { get; set; }

        [XmlAttribute(AttributeName = "encoding")]
        public string Encoding { get; set; }

        [XmlElement(ElementName = "tile")]
        public List<TmxDataTile> Tiles { get; set; }

        [XmlText]
        public string Value { get; set; }

        public TmxData()
        {
            Tiles = new List<TmxDataTile>();
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Encoding, Compression);
        }
    }
}