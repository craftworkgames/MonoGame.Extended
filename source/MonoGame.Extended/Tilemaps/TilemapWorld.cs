using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// A collection of positioned tilemaps that together form a world map.
/// </summary>
public sealed class TilemapWorld
{
    /// <summary>
    /// Gets the levels that make up this world.
    /// </summary>
    public IReadOnlyList<Tilemap> Levels { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapWorld"/> class.
    /// </summary>
    /// <param name="levels">The tilemap levels that make up the world.</param>
    public TilemapWorld(IReadOnlyList<Tilemap> levels)
    {
        ArgumentNullException.ThrowIfNull(levels);
        Levels = levels;
    }
}
