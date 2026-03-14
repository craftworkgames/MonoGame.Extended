using System;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Specifies flip transformations applied to a tile.
/// </summary>
[Flags]
public enum TilemapTileFlipFlags
{
    /// <summary>
    /// No flip transformations.
    /// </summary>
    None = 0,

    /// <summary>
    /// Tile is flipped horizontally.
    /// </summary>
    FlipHorizontally = 1,

    /// <summary>
    /// Tile is flipped vertically.
    /// </summary>
    FlipVertically = 2,

    /// <summary>
    /// Tile is flipped diagonally (rotated 90 degrees and flipped).
    /// </summary>
    FlipDiagonally = 4
}
