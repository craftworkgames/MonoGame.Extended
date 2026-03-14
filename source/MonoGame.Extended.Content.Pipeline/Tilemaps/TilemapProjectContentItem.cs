using System.Collections.Generic;

using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Common pipeline content item produced by all format-specific tilemap importers and consumed by
/// <see cref="TilemapProcessor"/> and <see cref="TilemapWorldProcessor"/>. Holds all levels from
/// the source project, already converted to the format-agnostic <see cref="TilemapData"/> representation.
/// </summary>
public sealed class TilemapProjectContentItem : ContentItem<List<TilemapData>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapProjectContentItem"/> class.
    /// </summary>
    /// <param name="levels">All converted levels from the source project.</param>
    public TilemapProjectContentItem(List<TilemapData> levels) : base(levels) { }
}
