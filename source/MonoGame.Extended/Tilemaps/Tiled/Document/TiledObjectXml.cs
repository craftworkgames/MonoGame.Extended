using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents an object from a Tiled TMX file.
/// </summary>
public class TiledObjectXml
{
    /// <summary>
    /// Gets or sets the unique object ID.
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the object name.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the object type (deprecated, use <see cref="Class"/> instead).
    /// </summary>
    [XmlAttribute("type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the object class (Tiled 1.9+).
    /// </summary>
    [XmlAttribute("class")]
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets the X coordinate in pixels.
    /// </summary>
    [XmlAttribute("x")]
    public float X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate in pixels.
    /// </summary>
    [XmlAttribute("y")]
    public float Y { get; set; }

    /// <summary>
    /// Gets or sets the width in pixels.
    /// </summary>
    [XmlAttribute("width")]
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the height in pixels.
    /// </summary>
    [XmlAttribute("height")]
    public float Height { get; set; }

    /// <summary>
    /// Gets or sets the rotation in degrees (clockwise).
    /// </summary>
    [XmlAttribute("rotation")]
    public float Rotation { get; set; }

    /// <summary>
    /// Gets or sets the global tile ID (for tile objects).
    /// </summary>
    [XmlAttribute("gid")]
    public uint Gid { get; set; }

    /// <summary>
    /// Gets or sets whether the object is visible (1 = visible, 0 = hidden).
    /// </summary>
    [XmlAttribute("visible")]
    public int Visible { get; set; } = 1;

    /// <summary>
    /// Gets or sets the custom properties for this object.
    /// </summary>
    [XmlElement("properties")]
    public TiledPropertiesXml Properties { get; set; }

    /// <summary>
    /// Gets or sets the ellipse marker (presence indicates ellipse object).
    /// </summary>
    [XmlElement("ellipse")]
    public TiledEllipseXml Ellipse { get; set; }

    /// <summary>
    /// Gets or sets the point marker (presence indicates point object).
    /// </summary>
    [XmlElement("point")]
    public TiledPointXml Point { get; set; }

    /// <summary>
    /// Gets or sets the polygon data (for polygon objects).
    /// </summary>
    [XmlElement("polygon")]
    public TiledPolygonXml Polygon { get; set; }

    /// <summary>
    /// Gets or sets the polyline data (for polyline objects).
    /// </summary>
    [XmlElement("polyline")]
    public TiledPolylineXml Polyline { get; set; }

    /// <summary>
    /// Gets or sets the text data (for text objects).
    /// </summary>
    [XmlElement("text")]
    public TiledTextXml Text { get; set; }
}
