using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a layer that displays a single image.
/// </summary>
public class TilemapImageLayer : TilemapLayer
{
    /// <summary>
    /// Gets or sets the image texture displayed by this layer.
    /// </summary>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// Gets or sets the position of the image.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the image repeats horizontally.
    /// </summary>
    public bool RepeatX { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the image repeats vertically.
    /// </summary>
    public bool RepeatY { get; set; }

    /// <inheritdoc/>
    public override Rectangle Bounds
    {
        get
        {
            if (Texture == null)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                Texture.Width,
                Texture.Height
            );
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapImageLayer"/> class.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    /// <param name="texture">The image texture.</param>
    /// <param name="position">The position of the image.</param>
    public TilemapImageLayer(string name, Texture2D texture, Vector2 position) : base(name)
    {
        Texture = texture;
        Position = position;
    }
}
