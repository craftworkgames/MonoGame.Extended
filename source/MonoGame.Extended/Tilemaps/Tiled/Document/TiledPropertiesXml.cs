using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents a properties collection from a Tiled TMX file.
/// </summary>
public class TiledPropertiesXml
{
    /// <summary>
    /// Gets or sets the collection of custom properties.
    /// </summary>
    [XmlElement("property")]
    public List<TiledPropertyXml> Properties { get; set; } = new List<TiledPropertyXml>();
}
