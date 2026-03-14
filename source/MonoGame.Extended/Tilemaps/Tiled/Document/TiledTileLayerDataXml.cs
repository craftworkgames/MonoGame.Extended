using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents the tile data element of a tile layer.
/// </summary>
public class TiledTileLayerDataXml
{
    /// <summary>
    /// Gets or sets the encoding format (csv, base64, or null for XML).
    /// </summary>
    [XmlAttribute("encoding")]
    public string Encoding { get; set; }

    /// <summary>
    /// Gets or sets the compression format (gzip, zlib, or null for uncompressed).
    /// </summary>
    [XmlAttribute("compression")]
    public string Compression { get; set; }

    /// <summary>
    /// Gets or sets the tile data as a string (for CSV or Base64 encoding).
    /// </summary>
    [XmlText]
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the tile data as individual tile elements (for XML encoding).
    /// </summary>
    [XmlElement("tile")]
    public List<TiledDataTileXml> Tiles { get; set; } = new List<TiledDataTileXml>();

    /// <summary>
    /// Gets or sets the chunk data (for infinite maps).
    /// </summary>
    [XmlElement("chunk")]
    public List<TiledChunkXml> Chunks { get; set; } = new List<TiledChunkXml>();
}
