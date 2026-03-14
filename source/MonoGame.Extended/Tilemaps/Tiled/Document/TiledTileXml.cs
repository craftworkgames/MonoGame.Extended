using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents an individual tile definition within a tileset.
/// </summary>
public class TiledTileXml
{
    /// <summary>
    /// Gets or sets the local tile ID within the tileset.
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the tile type (deprecated, use <see cref="Class"/> instead).
    /// </summary>
    [XmlAttribute("type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the tile class (Tiled 1.9+).
    /// </summary>
    [XmlAttribute("class")]
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets the probability that this tile is chosen over others when painting with random mode.
    /// </summary>
    [XmlAttribute("probability")]
    public float Probability { get; set; }

    /// <summary>
    /// Gets or sets the custom properties for this tile.
    /// </summary>
    [XmlElement("properties")]
    public TiledPropertiesXml Properties { get; set; }

    /// <summary>
    /// Gets or sets the image for this tile (for image collection tilesets).
    /// </summary>
    [XmlElement("image")]
    public TiledImageXml Image { get; set; }

    /// <summary>
    /// Gets or sets the collision object group for this tile.
    /// </summary>
    [XmlElement("objectgroup")]
    public TiledObjectGroupXml ObjectGroup { get; set; }

    /// <summary>
    /// Gets or sets the animation data for this tile.
    /// </summary>
    [XmlElement("animation")]
    public TiledAnimationXml Animation { get; set; }
}
