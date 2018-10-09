using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    [XmlInclude(typeof(TiledMapTileLayerContent))]
    [XmlInclude(typeof(TiledMapImageLayerContent))]
    [XmlInclude(typeof(TiledMapObjectLayerContent))]
    public abstract class TiledMapLayerContent
    {
        protected TiledMapLayerContent(TiledMapLayerType type)
        {
            Type = type;
            Opacity = 1.0f;
            Visible = true;
            Properties = new List<TiledMapPropertyContent>();
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
        public List<TiledMapPropertyContent> Properties { get; set; }

        [XmlIgnore]
        public TiledMapLayerType Type { get; }
        
        public override string ToString()
        {
            return Name;
        }
    }
}