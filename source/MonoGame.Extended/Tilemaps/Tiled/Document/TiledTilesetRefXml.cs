using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents a tileset reference in a Tiled TMX file (can be inline or external).
/// </summary>
public class TiledTilesetRefXml : TiledTilesetXml
{
    /// <summary>
    /// Gets or sets the path to an external TSX file.
    /// </summary>
    /// <remarks>
    /// If this property is set, the tileset data is loaded from the external file.
    /// Otherwise, the tileset is defined inline in the TMX file.
    /// </remarks>
    [XmlAttribute("source")]
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the tileset data loaded from an external TSX file.
    /// </summary>
    /// <remarks>
    /// This property is populated during parsing when <see cref="Source"/> is specified.
    /// </remarks>
    [XmlIgnore]
    public TiledTilesetXml TilesetData { get; set; }
}
