using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxImageLayer : TmxLayer
    {
        public TmxImageLayer()
        {
            Opacity = 1.0f;
            Visible = true;
            Properties = new List<TmxProperty>();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, Image);
        }

        [XmlAttribute(AttributeName = "x")]
        public int X { get; set; }

        [XmlAttribute(AttributeName = "y")]
        public int Y { get; set; }

        [XmlElement(ElementName = "image")]
        public TmxImage Image { get; set; }
    }
}