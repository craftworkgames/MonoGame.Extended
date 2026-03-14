using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents polygon point data.
/// </summary>
public class TiledPolygonXml
{
    /// <summary>
    /// Gets or sets the polygon points as a space-separated list of "x,y" pairs.
    /// </summary>
    [XmlAttribute("points")]
    public string Points { get; set; }
}
