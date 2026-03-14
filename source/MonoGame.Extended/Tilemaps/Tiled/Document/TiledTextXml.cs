using System.Xml.Serialization;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents text object data.
/// </summary>
public class TiledTextXml
{
    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    [XmlAttribute("fontfamily")]
    public string FontFamily { get; set; }

    /// <summary>
    /// Gets or sets the font size in pixels.
    /// </summary>
    [XmlAttribute("pixelsize")]
    public int PixelSize { get; set; } = 16;

    /// <summary>
    /// Gets or sets whether text wrapping is enabled.
    /// </summary>
    [XmlAttribute("wrap")]
    public int Wrap { get; set; }

    /// <summary>
    /// Gets or sets the text color in #AARRGGBB or #RRGGBB format.
    /// </summary>
    [XmlAttribute("color")]
    public string Color { get; set; } = "#000000";

    /// <summary>
    /// Gets or sets whether the text is bold.
    /// </summary>
    [XmlAttribute("bold")]
    public int Bold { get; set; }

    /// <summary>
    /// Gets or sets whether the text is italic.
    /// </summary>
    [XmlAttribute("italic")]
    public int Italic { get; set; }

    /// <summary>
    /// Gets or sets whether the text is underlined.
    /// </summary>
    [XmlAttribute("underline")]
    public int Underline { get; set; }

    /// <summary>
    /// Gets or sets whether the text has a strikethrough.
    /// </summary>
    [XmlAttribute("strikeout")]
    public int Strikeout { get; set; }

    /// <summary>
    /// Gets or sets whether kerning is enabled.
    /// </summary>
    [XmlAttribute("kerning")]
    public int Kerning { get; set; } = 1;

    /// <summary>
    /// Gets or sets the horizontal alignment (left, center, right, justify).
    /// </summary>
    [XmlAttribute("halign")]
    public string HAlign { get; set; } = "left";

    /// <summary>
    /// Gets or sets the vertical alignment (top, center, bottom).
    /// </summary>
    [XmlAttribute("valign")]
    public string VAlign { get; set; } = "top";

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    [XmlText]
    public string Value { get; set; }
}
