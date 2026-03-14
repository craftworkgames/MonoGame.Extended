using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents an object group element (used for tile collision objects).
/// </summary>
public class TiledObjectGroupXml
{
    /// <summary>
    /// Gets or sets the draw order (topdown or index).
    /// </summary>
    [XmlAttribute("draworder")]
    public string DrawOrder { get; set; }

    /// <summary>
    /// Gets or sets the collection of objects in this group.
    /// </summary>
    [XmlElement("object")]
    public List<TiledObjectXml> Objects { get; set; } = new List<TiledObjectXml>();
}
