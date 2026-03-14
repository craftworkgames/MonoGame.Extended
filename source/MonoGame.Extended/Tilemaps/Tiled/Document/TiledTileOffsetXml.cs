using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents the tile offset configuration for a tileset.
/// </summary>
public class TiledTileOffsetXml
{
    /// <summary>
    /// Gets or sets the horizontal offset in pixels.
    /// </summary>
    [XmlAttribute("x")]
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the vertical offset in pixels.
    /// </summary>
    [XmlAttribute("y")]
    public int Y { get; set; }
}
