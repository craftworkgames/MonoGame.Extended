using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a point object in a tilemap with no spatial extent.
/// </summary>
public class TilemapPointObject : TilemapObject
{
    /// <inheritdoc/>
    public override BoundingBox2D Bounds => new BoundingBox2D(Position, Position);

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapPointObject"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the object.</param>
    /// <param name="position">The position of the point.</param>
    public TilemapPointObject(int id, Vector2 position) : base(id, position)
    {
    }
}
