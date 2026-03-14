using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents a chunk of tiles in an infinite map.
/// </summary>
public class TiledChunkXml
{
    /// <summary>
    /// Gets or sets the X coordinate of the chunk in tiles.
    /// </summary>
    [XmlAttribute("x")]
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of the chunk in tiles.
    /// </summary>
    [XmlAttribute("y")]
    public int Y { get; set; }

    /// <summary>
    /// Gets or sets the chunk width in tiles.
    /// </summary>
    [XmlAttribute("width")]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the chunk height in tiles.
    /// </summary>
    [XmlAttribute("height")]
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the tile data for this chunk.
    /// </summary>
    [XmlText]
    public string Value { get; set; }
}
