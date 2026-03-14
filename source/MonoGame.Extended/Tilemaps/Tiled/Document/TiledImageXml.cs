using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents an image element from a Tiled TMX file.
/// </summary>
public class TiledImageXml
{
    /// <summary>
    /// Gets or sets the image source path (relative to the TMX/TSX file).
    /// </summary>
    [XmlAttribute("source")]
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the image width in pixels.
    /// </summary>
    [XmlAttribute("width")]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the image height in pixels.
    /// </summary>
    [XmlAttribute("height")]
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the transparent color in #RRGGBB format.
    /// </summary>
    [XmlAttribute("trans")]
    public string Trans { get; set; }

    /// <summary>
    /// Gets or sets the loaded texture.
    /// </summary>
    [XmlIgnore]
    public Texture2D Texture { get; set; }
}
