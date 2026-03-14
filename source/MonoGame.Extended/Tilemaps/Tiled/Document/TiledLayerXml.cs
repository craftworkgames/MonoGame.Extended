using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Base class for all Tiled layer types.
/// </summary>
public class TiledLayerXml
{
    /// <summary>
    /// Gets or sets the unique layer ID.
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the layer name.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the layer class (Tiled 1.9+).
    /// </summary>
    [XmlAttribute("class")]
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets the horizontal offset in pixels.
    /// </summary>
    [XmlAttribute("offsetx")]
    public float OffsetX { get; set; }

    /// <summary>
    /// Gets or sets the vertical offset in pixels.
    /// </summary>
    [XmlAttribute("offsety")]
    public float OffsetY { get; set; }

    /// <summary>
    /// Gets or sets the horizontal parallax factor (Tiled 1.5+).
    /// </summary>
    [XmlAttribute("parallaxx")]
    public float ParallaxX { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the vertical parallax factor (Tiled 1.5+).
    /// </summary>
    [XmlAttribute("parallaxy")]
    public float ParallaxY { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the layer opacity (0.0 to 1.0).
    /// </summary>
    [XmlAttribute("opacity")]
    public float Opacity { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets whether the layer is visible (1 = visible, 0 = hidden).
    /// </summary>
    [XmlAttribute("visible")]
    public int Visible { get; set; } = 1;

    /// <summary>
    /// Gets or sets the layer tint color in #AARRGGBB or #RRGGBB format.
    /// </summary>
    [XmlAttribute("tintcolor")]
    public string TintColor { get; set; }

    /// <summary>
    /// Gets or sets the custom properties for this layer.
    /// </summary>
    [XmlElement("properties")]
    public TiledPropertiesXml Properties { get; set; }
}
