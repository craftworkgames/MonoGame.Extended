using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public class TiledMapGroupLayerContent : TiledMapLayerContent
    {
        protected TiledMapGroupLayerContent() 
            : base(TiledMapLayerType.GroupLayer)
        {
        }

        [XmlElement(ElementName = "layer", Type = typeof(TiledMapTileLayerContent))]
        [XmlElement(ElementName = "imagelayer", Type = typeof(TiledMapImageLayerContent))]
        [XmlElement(ElementName = "objectgroup", Type = typeof(TiledMapObjectLayerContent))]
        [XmlElement(ElementName = "group", Type = typeof(TiledMapGroupLayerContent))]
        public List<TiledMapLayerContent> Layers { get; set; }

    }
}
