using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxData
    {
        public TmxData()
        {
            Tiles = new List<TmxDataTile>();
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Encoding, Compression);
        }

        [XmlAttribute(AttributeName = "encoding")]
        public string Encoding { get; set; }

        [XmlAttribute(AttributeName = "compression")]
        public string Compression { get; set; }

        [XmlElement(ElementName = "tile")]
        public List<TmxDataTile> Tiles { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}