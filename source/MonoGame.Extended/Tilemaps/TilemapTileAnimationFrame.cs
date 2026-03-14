namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a single frame in a tile animation.
/// </summary>
public readonly struct TilemapTileAnimationFrame
{
    /// <summary>
    /// Gets the local tile ID for this frame.
    /// </summary>
    public int TileId { get; }

    /// <summary>
    /// Gets the duration of this frame in seconds.
    /// </summary>
    public float Duration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapTileAnimationFrame"/> struct.
    /// </summary>
    /// <param name="tileId">The local tile ID for this frame.</param>
    /// <param name="duration">The duration of this frame in seconds.</param>
    public TilemapTileAnimationFrame(int tileId, float duration)
    {
        TileId = tileId;
        Duration = duration;
    }
}
