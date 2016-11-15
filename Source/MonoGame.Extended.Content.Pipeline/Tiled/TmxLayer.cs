using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [XmlInclude(typeof(TmxTileLayer))]
    [XmlInclude(typeof(TmxImageLayer))]
    [XmlInclude(typeof(TmxObjectLayer))]
    public abstract class TmxLayer
    {
        protected TmxLayer()
        {
            Opacity = 1.0f;
            Visible = true;
            Properties = new List<TmxProperty>();
        }

        public override string ToString()
        {
            return Name;
        }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        
        [XmlAttribute(AttributeName = "opacity")]
        public float Opacity { get; set; }

        [XmlAttribute(AttributeName = "visible")]
        public bool Visible { get; set; }

        [XmlAttribute(AttributeName = "offsetx")]
        public float OffsetX { get; set; }

        [XmlAttribute(AttributeName = "offsety")]
        public float OffsetY { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TmxProperty> Properties { get; set; }
    }
}