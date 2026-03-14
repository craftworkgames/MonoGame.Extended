using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps.Tiled.Document;

/// <summary>
/// Represents an image layer from a Tiled TMX file.
/// </summary>
public class TiledImageLayerXml : TiledLayerXml
{
    /// <summary>
    /// Gets or sets whether the image repeats horizontally (Tiled 1.8+).
    /// </summary>
    [XmlAttribute("repeatx")]
    public int RepeatX { get; set; }

    /// <summary>
    /// Gets or sets whether the image repeats vertically (Tiled 1.8+).
    /// </summary>
    [XmlAttribute("repeaty")]
    public int RepeatY { get; set; }

    /// <summary>
    /// Gets or sets the image for this layer.
    /// </summary>
    [XmlElement("image")]
    public TiledImageXml Image { get; set; }

    /// <summary>
    /// Gets or sets the loaded texture for this layer.
    /// </summary>
    [XmlIgnore]
    public Texture2D Texture { get; set; }
}
