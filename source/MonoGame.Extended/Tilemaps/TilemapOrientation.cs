namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Specifies which axis is staggered for staggered and hexagonal tilemaps.
/// </summary>
public enum TilemapStaggerAxis
{
    /// <summary>
    /// Columns are staggered along the X axis.
    /// </summary>
    X,

    /// <summary>
    /// Rows are staggered along the Y axis.
    /// </summary>
    Y
}

/// <summary>
/// Specifies which rows or columns are staggered for staggered and hexagonal tilemaps.
/// </summary>
public enum TilemapStaggerIndex
{
    /// <summary>
    /// Even rows or columns are staggered.
    /// </summary>
    Even,

    /// <summary>
    /// Odd rows or columns are staggered.
    /// </summary>
    Odd
}

/// <summary>
/// Specifies the orientation of a tilemap.
/// </summary>
public enum TilemapOrientation
{
    /// <summary>
    /// Standard grid layout where tiles are arranged in rows and columns.
    /// </summary>
    Orthogonal,

    /// <summary>
    /// Diamond-shaped grid layout where tiles are rotated 45 degrees.
    /// </summary>
    Isometric,

    /// <summary>
    /// Hexagonal or isometric layout with staggered rows or columns.
    /// </summary>
    Staggered,

    /// <summary>
    /// Hexagonal grid layout.
    /// </summary>
    Hexagonal
}
