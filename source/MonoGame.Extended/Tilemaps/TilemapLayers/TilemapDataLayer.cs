using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a layer that carries data but has no visual tiles.
/// The renderer skips data layers entirely.
/// </summary>
/// <remarks>
/// Data layers carry format-specific information that does not correspond to
/// renderable content, such as collision type grids or region tag grids.
/// The data is stored in <see cref="TilemapLayer.Properties"/> and its meaning
/// is defined by the source format and the game; MonoGame.Extended does not
/// interpret the property values.
/// </remarks>
public class TilemapDataLayer : TilemapLayer
{
    /// <summary>
    /// Gets the width of the layer in tiles.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the layer in tiles.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the width of each cell in pixels.
    /// </summary>
    public int TileWidth { get; }

    /// <summary>
    /// Gets the height of each cell in pixels.
    /// </summary>
    public int TileHeight { get; }

    /// <inheritdoc/>
    public override Rectangle Bounds =>
        new Rectangle(0, 0, Width * TileWidth, Height * TileHeight);

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapDataLayer"/> class.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    /// <param name="width">The width of the layer in tiles.</param>
    /// <param name="height">The height of the layer in tiles.</param>
    /// <param name="tileWidth">The width of each cell in pixels.</param>
    /// <param name="tileHeight">The height of each cell in pixels.</param>
    public TilemapDataLayer(string name, int width, int height, int tileWidth, int tileHeight)
        : base(name)
    {
        Width = width;
        Height = height;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
    }
}
