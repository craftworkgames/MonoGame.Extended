namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Specifies the rendering order for objects in an object layer.
/// </summary>
public enum TilemapObjectDrawOrder
{
    /// <summary>
    /// Objects are drawn based on their Y position (top to bottom).
    /// </summary>
    TopDown,

    /// <summary>
    /// Objects are drawn in the order they appear in the layer.
    /// </summary>
    Index
}
