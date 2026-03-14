using System.Collections.Generic;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Pipeline content item produced by <see cref="TilemapWorldProcessor"/> and consumed by
/// <see cref="TilemapWorldWriter"/>. Holds all levels for world serialization.
/// </summary>
public sealed class TilemapWorldContentItem : ContentItem<List<TilemapData>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapWorldContentItem"/> class.
    /// </summary>
    /// <param name="levels">All levels to serialize into the world asset.</param>
    public TilemapWorldContentItem(List<TilemapData> levels) : base(levels) { }
}
