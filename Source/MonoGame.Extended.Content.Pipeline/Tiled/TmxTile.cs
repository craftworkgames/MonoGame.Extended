using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxTile
    {
        public TmxTile()
        {
            Probability = 1.0f;
            Properties = new List<TmxProperty>();
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        [XmlElement(ElementName = "terrain")]
        public TmxTerrain Terrain { get; set; }

        [XmlAttribute(AttributeName = "probability")]
        public float Probability { get; set; }

        [XmlElement(ElementName = "image")]
        public TmxImage Image { get; set; }

        [XmlArray("animation")]
        [XmlArrayItem("frame")]
        public List<TmxFrame> Frames { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TmxProperty> Properties { get; set; }
    }
}