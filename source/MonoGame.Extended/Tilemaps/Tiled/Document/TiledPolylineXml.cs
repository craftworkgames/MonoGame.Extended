using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents polyline point data.
/// </summary>
public class TiledPolylineXml
{
    /// <summary>
    /// Gets or sets the polyline points as a space-separated list of "x,y" pairs.
    /// </summary>
    [XmlAttribute("points")]
    public string Points { get; set; }
}
