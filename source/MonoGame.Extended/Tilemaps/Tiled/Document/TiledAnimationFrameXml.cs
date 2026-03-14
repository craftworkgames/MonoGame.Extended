using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents a single frame in a tile animation.
/// </summary>
public class TiledAnimationFrameXml
{
    /// <summary>
    /// Gets or sets the local tile ID for this frame.
    /// </summary>
    [XmlAttribute("tileid")]
    public int TileId { get; set; }

    /// <summary>
    /// Gets or sets the duration of this frame in milliseconds.
    /// </summary>
    [XmlAttribute("duration")]
    public int Duration { get; set; }
}
