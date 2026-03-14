using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents an object layer from a Tiled TMX file.
/// </summary>
public class TiledObjectLayerXml : TiledLayerXml
{
    /// <summary>
    /// Gets or sets the layer color in #AARRGGBB or #RRGGBB format.
    /// </summary>
    [XmlAttribute("color")]
    public string Color { get; set; }

    /// <summary>
    /// Gets or sets the draw order (topdown or index).
    /// </summary>
    [XmlAttribute("draworder")]
    public string DrawOrder { get; set; }

    /// <summary>
    /// Gets or sets the collection of objects in this layer.
    /// </summary>
    [XmlElement("object")]
    public List<TiledObjectXml> Objects { get; set; } = new List<TiledObjectXml>();
}
