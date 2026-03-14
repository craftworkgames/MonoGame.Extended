using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents a tileset element from a Tiled TMX or TSX file.
/// </summary>
[XmlRoot("tileset")]
public class TiledTilesetXml
{
    /// <summary>
    /// Gets or sets the first global tile ID of this tileset.
    /// </summary>
    [XmlAttribute("firstgid")]
    public int FirstGlobalId { get; set; }

    /// <summary>
    /// Gets or sets the name of the tileset.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the width of tiles in this tileset in pixels.
    /// </summary>
    [XmlAttribute("tilewidth")]
    public int TileWidth { get; set; }

    /// <summary>
    /// Gets or sets the height of tiles in this tileset in pixels.
    /// </summary>
    [XmlAttribute("tileheight")]
    public int TileHeight { get; set; }

    /// <summary>
    /// Gets or sets the total number of tiles in this tileset.
    /// </summary>
    [XmlAttribute("tilecount")]
    public int TileCount { get; set; }

    /// <summary>
    /// Gets or sets the number of tile columns in the tileset image.
    /// </summary>
    [XmlAttribute("columns")]
    public int Columns { get; set; }

    /// <summary>
    /// Gets or sets the spacing in pixels between tiles in the tileset image.
    /// </summary>
    [XmlAttribute("spacing")]
    public int Spacing { get; set; }

    /// <summary>
    /// Gets or sets the margin in pixels around tiles in the tileset image.
    /// </summary>
    [XmlAttribute("margin")]
    public int Margin { get; set; }

    /// <summary>
    /// Gets or sets the alignment for tile objects (unspecified, topleft, top, topright, left, center, right, bottomleft, bottom, bottomright).
    /// </summary>
    [XmlAttribute("objectalignment")]
    public string ObjectAlignment { get; set; }

    /// <summary>
    /// Gets or sets the tile offset configuration.
    /// </summary>
    [XmlElement("tileoffset")]
    public TiledTileOffsetXml TileOffset { get; set; }

    /// <summary>
    /// Gets or sets the grid configuration for isometric or staggered tilesets.
    /// </summary>
    [XmlElement("grid")]
    public TiledGridXml Grid { get; set; }

    /// <summary>
    /// Gets or sets the tileset image.
    /// </summary>
    [XmlElement("image")]
    public TiledImageXml Image { get; set; }

    /// <summary>
    /// Gets or sets the collection of individual tile data (animations, collisions, properties).
    /// </summary>
    [XmlElement("tile")]
    public List<TiledTileXml> Tiles { get; set; } = new List<TiledTileXml>();

    /// <summary>
    /// Gets or sets the custom properties for this tileset.
    /// </summary>
    [XmlElement("properties")]
    public TiledPropertiesXml Properties { get; set; }

    /// <summary>
    /// Gets or sets the loaded texture for this tileset.
    /// </summary>
    [XmlIgnore]
    public Texture2D Texture { get; set; }
}
