using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents a tile animation sequence.
/// </summary>
public class TiledAnimationXml
{
    /// <summary>
    /// Gets or sets the collection of animation frames.
    /// </summary>
    [XmlElement("frame")]
    public List<TiledAnimationFrameXml> Frames { get; set; } = new List<TiledAnimationFrameXml>();
}
