using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents a custom property from a Tiled TMX file.
/// </summary>
public class TiledPropertyXml
{
    /// <summary>
    /// Gets or sets the property name.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the property type (string, int, float, bool, color, file, object, class).
    /// </summary>
    [XmlAttribute("type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the property value as a string.
    /// </summary>
    [XmlAttribute("value")]
    public string Value { get; set; }
}
