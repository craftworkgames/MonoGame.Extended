using System.Collections.Generic;
using System.Xml.Serialization;

using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapGroupLayerContent : TiledMapLayerContent
    {

        [XmlElement(ElementName = "layer", Type = typeof(TiledMapTileLayerContent))]
        [XmlElement(ElementName = "imagelayer", Type = typeof(TiledMapImageLayerContent))]
        [XmlElement(ElementName = "objectgroup", Type = typeof(TiledMapObjectLayerContent))]
        [XmlElement(ElementName = "group", Type = typeof(TiledMapGroupLayerContent))]
        public List<TiledMapLayerContent> Layers { get; set; }

        public TiledMapGroupLayerContent()
            : base(TiledMapLayerType.GroupLayer)
        {
            Layers = new List<TiledMapLayerContent>();
        }
    }
}
