using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Pipeline content item carrying format-agnostic tilemap data ready for binary serialization.
/// </summary>
/// <remarks>
/// All format processors (Tiled, LDtk, Ogmo) produce this type; <see cref="TilemapWriter"/> consumes it.
/// </remarks>
public sealed class TilemapContentItem : ContentItem<TilemapData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapContentItem"/> class.
    /// </summary>
    /// <param name="data">The format-agnostic tilemap data.</param>
    public TilemapContentItem(TilemapData data) : base(data) { }
}
