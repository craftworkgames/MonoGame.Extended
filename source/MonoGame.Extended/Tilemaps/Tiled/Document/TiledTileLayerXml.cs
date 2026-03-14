using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents a tile layer from a Tiled TMX file.
/// </summary>
public class TiledTileLayerXml : TiledLayerXml
{
    /// <summary>
    /// Gets or sets the layer width in tiles.
    /// </summary>
    [XmlAttribute("width")]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the layer height in tiles.
    /// </summary>
    [XmlAttribute("height")]
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the tile data for this layer.
    /// </summary>
    [XmlElement("data")]
    public TiledTileLayerDataXml Data { get; set; }
}
