using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents a single tile element in XML-encoded tile data.
/// </summary>
public class TiledDataTileXml
{
    /// <summary>
    /// Gets or sets the global tile ID (includes flip flags in high bits).
    /// </summary>
    [XmlAttribute("gid")]
    public uint Gid { get; set; }
}
