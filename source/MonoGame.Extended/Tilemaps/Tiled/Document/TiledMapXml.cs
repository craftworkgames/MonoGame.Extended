using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents the root map element from a Tiled TMX file.
/// </summary>
[XmlRoot("map")]
public class TiledMapXml
{
    /// <summary>
    /// Gets or sets the TMX format version.
    /// </summary>
    [XmlAttribute("version")]
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets the Tiled version used to save the file.
    /// </summary>
    [XmlAttribute("tiledversion")]
    public string TiledVersion { get; set; }

    /// <summary>
    /// Gets or sets the map orientation (orthogonal, isometric, staggered, hexagonal).
    /// </summary>
    [XmlAttribute("orientation")]
    public string Orientation { get; set; }

    /// <summary>
    /// Gets or sets the tile render order (right-down, right-up, left-down, left-up).
    /// </summary>
    [XmlAttribute("renderorder")]
    public string RenderOrder { get; set; }

    /// <summary>
    /// Gets or sets the map width in tiles.
    /// </summary>
    [XmlAttribute("width")]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the map height in tiles.
    /// </summary>
    [XmlAttribute("height")]
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the tile width in pixels.
    /// </summary>
    [XmlAttribute("tilewidth")]
    public int TileWidth { get; set; }

    /// <summary>
    /// Gets or sets the tile height in pixels.
    /// </summary>
    [XmlAttribute("tileheight")]
    public int TileHeight { get; set; }

    /// <summary>
    /// Gets or sets whether the map is infinite (1) or fixed size (0).
    /// </summary>
    [XmlAttribute("infinite")]
    public int Infinite { get; set; }

    /// <summary>
    /// Gets or sets the background color in #AARRGGBB or #RRGGBB format.
    /// </summary>
    [XmlAttribute("backgroundcolor")]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the parallax origin X in world pixels (Tiled 1.8+).
    /// </summary>
    [XmlAttribute("parallaxoriginx")]
    public float ParallaxOriginX { get; set; }

    /// <summary>
    /// Gets or sets the parallax origin Y in world pixels (Tiled 1.8+).
    /// </summary>
    [XmlAttribute("parallaxoriginy")]
    public float ParallaxOriginY { get; set; }

    /// <summary>
    /// Gets or sets the side length of hex tiles in pixels (hexagonal maps only).
    /// </summary>
    [XmlAttribute("hexsidelength")]
    public int HexSideLength { get; set; }

    /// <summary>
    /// Gets or sets the stagger axis for staggered and hexagonal maps ("x" or "y").
    /// </summary>
    [XmlAttribute("staggeraxis")]
    public string StaggerAxis { get; set; }

    /// <summary>
    /// Gets or sets the stagger index for staggered and hexagonal maps ("even" or "odd").
    /// </summary>
    [XmlAttribute("staggerindex")]
    public string StaggerIndex { get; set; }

    /// <summary>
    /// Gets or sets the next available layer ID.
    /// </summary>
    [XmlAttribute("nextlayerid")]
    public int NextLayerId { get; set; }

    /// <summary>
    /// Gets or sets the next available object ID.
    /// </summary>
    [XmlAttribute("nextobjectid")]
    public int NextObjectId { get; set; }

    /// <summary>
    /// Gets or sets the custom properties for this map.
    /// </summary>
    [XmlElement("properties")]
    public TiledPropertiesXml Properties { get; set; }

    /// <summary>
    /// Gets or sets the collection of tilesets used by this map.
    /// </summary>
    [XmlElement("tileset")]
    public List<TiledTilesetRefXml> Tilesets { get; set; } = new List<TiledTilesetRefXml>();

    /// <summary>
    /// Gets or sets the collection of layers in this map.
    /// </summary>
    [XmlElement("layer", typeof(TiledTileLayerXml))]
    [XmlElement("objectgroup", typeof(TiledObjectLayerXml))]
    [XmlElement("imagelayer", typeof(TiledImageLayerXml))]
    [XmlElement("group", typeof(TiledGroupLayerXml))]
    public List<TiledLayerXml> Layers { get; set; } = new List<TiledLayerXml>();
}
