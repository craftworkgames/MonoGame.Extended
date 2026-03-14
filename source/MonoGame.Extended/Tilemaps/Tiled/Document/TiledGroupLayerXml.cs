using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents a group layer from a Tiled TMX file.
/// </summary>
public class TiledGroupLayerXml : TiledLayerXml
{
    /// <summary>
    /// Gets or sets the child layers in this group.
    /// </summary>
    [XmlElement("layer", typeof(TiledTileLayerXml))]
    [XmlElement("objectgroup", typeof(TiledObjectLayerXml))]
    [XmlElement("imagelayer", typeof(TiledImageLayerXml))]
    [XmlElement("group", typeof(TiledGroupLayerXml))]
    public List<TiledLayerXml> Layers { get; set; } = new List<TiledLayerXml>();
}
