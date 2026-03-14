using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents an elliptical object in a tilemap.
/// </summary>
public class TilemapEllipseObject : TilemapObject
{
    /// <summary>
    /// Gets or sets the size of the ellipse's bounding rectangle.
    /// </summary>
    public Vector2 Size { get; set; }

    /// <summary>
    /// Gets the center point of the ellipse.
    /// </summary>
    public Vector2 Center => Position + Size * 0.5f;

    /// <summary>
    /// Gets the horizontal radius of the ellipse.
    /// </summary>
    public float RadiusX => Size.X * 0.5f;

    /// <summary>
    /// Gets the vertical radius of the ellipse.
    /// </summary>
    public float RadiusY => Size.Y * 0.5f;

    /// <summary>
    /// Gets a bounding circle for this ellipse when it is circular, or <see langword="null"/> when it is not.
    /// </summary>
    /// <remarks>
    /// Returns a value only when <see cref="RadiusX"/> equals <see cref="RadiusY"/> within floating-point
    /// tolerance. For non-uniform ellipses, use <see cref="Bounds"/> as an approximation.
    /// </remarks>
    public BoundingCircle2D? Circle
    {
        get
        {
            if (MathF.Abs(RadiusX - RadiusY) < 1e-6f)
            {
                return new BoundingCircle2D(Center, RadiusX);
            }

            return null;
        }
    }

    /// <inheritdoc/>
    public override BoundingBox2D Bounds => BoundingBox2D.CreateFromPositionAndSize(Position, Size);

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapEllipseObject"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the object.</param>
    /// <param name="position">The position of the top-left corner of the bounding rectangle.</param>
    /// <param name="size">The size of the bounding rectangle.</param>
    public TilemapEllipseObject(int id, Vector2 position, Vector2 size) : base(id, position)
    {
        Size = size;
    }
}
