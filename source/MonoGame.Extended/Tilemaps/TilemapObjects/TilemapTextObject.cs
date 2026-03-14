using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a text object in a tilemap.
/// </summary>
/// <remarks>
/// Text objects store text display information. Rendering is the user's responsibility.
/// </remarks>
public class TilemapTextObject : TilemapObject
{
    /// <summary>
    /// Gets or sets the bounding size for the text.
    /// </summary>
    public Vector2 Size { get; set; }

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the font family name.
    /// </summary>
    public string FontFamily { get; set; }

    /// <summary>
    /// Gets or sets the font size in pixels.
    /// </summary>
    public int PixelSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the text is bold.
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the text is italic.
    /// </summary>
    public bool Italic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the text is underlined.
    /// </summary>
    public bool Underline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the text is struck through.
    /// </summary>
    public bool Strikethrough { get; set; }

    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether text wraps within the bounding box.
    /// </summary>
    public bool WordWrap { get; set; }

    /// <summary>
    /// Gets or sets the horizontal alignment of the text.
    /// </summary>
    public TilemapTextObjectHorizontalAlignment HorizontalAlign { get; set; }

    /// <summary>
    /// Gets or sets the vertical alignment of the text.
    /// </summary>
    public TilemapTextObjectVerticalAlignment VerticalAlign { get; set; }

    /// <inheritdoc/>
    public override BoundingBox2D Bounds => BoundingBox2D.CreateFromPositionAndSize(Position, Size);

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapTextObject"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the object.</param>
    /// <param name="position">The position of the text.</param>
    /// <param name="size">The bounding size for the text.</param>
    /// <param name="text">The text content.</param>
    public TilemapTextObject(int id, Vector2 position, Vector2 size, string text) : base(id, position)
    {
        Size = size;
        Text = text;
        FontFamily = "sans-serif";
        PixelSize = 16;
        Color = Color.Black;
        HorizontalAlign = TilemapTextObjectHorizontalAlignment.Left;
        VerticalAlign = TilemapTextObjectVerticalAlignment.Top;
    }
}

/// <summary>
/// Specifies horizontal text alignment.
/// </summary>
public enum TilemapTextObjectHorizontalAlignment
{
    /// <summary>
    /// Align text to the left.
    /// </summary>
    Left,

    /// <summary>
    /// Center text horizontally.
    /// </summary>
    Center,

    /// <summary>
    /// Align text to the right.
    /// </summary>
    Right,

    /// <summary>
    /// Justify text to fill the width.
    /// </summary>
    Justify
}

/// <summary>
/// Specifies vertical text alignment.
/// </summary>
public enum TilemapTextObjectVerticalAlignment
{
    /// <summary>
    /// Align text to the top.
    /// </summary>
    Top,

    /// <summary>
    /// Center text vertically.
    /// </summary>
    Center,

    /// <summary>
    /// Align text to the bottom.
    /// </summary>
    Bottom
}
