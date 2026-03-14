using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents the grid configuration for isometric or staggered tilesets.
/// </summary>
public class TiledGridXml
{
    /// <summary>
    /// Gets or sets the grid orientation (orthogonal or isometric).
    /// </summary>
    [XmlAttribute("orientation")]
    public string Orientation { get; set; }

    /// <summary>
    /// Gets or sets the grid width in pixels.
    /// </summary>
    [XmlAttribute("width")]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the grid height in pixels.
    /// </summary>
    [XmlAttribute("height")]
    public int Height { get; set; }
}
