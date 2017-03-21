using System.Collections.Generic;
using System.Xml.Serialization;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [XmlInclude(typeof(TiledMapTileLayerContent))]
    [XmlInclude(typeof(TiledMapImageLayerContent))]
    [XmlInclude(typeof(TiledMapObjectLayerContent))]
    public abstract class TiledMapLayerContent
    {
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

        [XmlIgnore]
        public TiledMapLayerModelContent[] Models { get; set; }

        protected TiledMapLayerContent(TiledMapLayerType type)
        {
            Type = type;
            Opacity = 1.0f;
            Visible = true;
            Properties = new List<TiledMapPropertyContent>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}