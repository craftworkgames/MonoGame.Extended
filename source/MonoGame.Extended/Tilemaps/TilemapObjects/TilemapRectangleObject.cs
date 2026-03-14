using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a rectangular object in a tilemap.
/// </summary>
public class TilemapRectangleObject : TilemapObject
{
    /// <summary>
    /// Gets or sets the size of the rectangle.
    /// </summary>
    public Vector2 Size { get; set; }

    /// <summary>
    /// Gets the oriented bounding box representing the rectangle, accounting for rotation.
    /// </summary>
    public OrientedBoundingBox2D Shape
    {
        get
        {
            float cos = MathF.Cos(Rotation);
            float sin = MathF.Sin(Rotation);
            Vector2 halfExtents = Size * 0.5f;

            // Rotation pivots around Position (top-left corner), so the OBB center is rotated accordingly.
            Vector2 center = Position + new Vector2(
                halfExtents.X * cos - halfExtents.Y * sin,
                halfExtents.X * sin + halfExtents.Y * cos
            );

            return OrientedBoundingBox2D.CreateFromRotation(center, Rotation, halfExtents);
        }
    }

    /// <inheritdoc/>
    public override BoundingBox2D Bounds
    {
        get
        {
            return BoundingBox2D.CreateFromPoints(Shape.GetCorners());
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapRectangleObject"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the object.</param>
    /// <param name="position">The position of the top-left corner.</param>
    /// <param name="size">The size of the rectangle.</param>
    public TilemapRectangleObject(int id, Vector2 position, Vector2 size) : base(id, position)
    {
        Size = size;
    }
}
