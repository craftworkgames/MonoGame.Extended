using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents an open polyline (unclosed polygon) object in a tilemap.
/// </summary>
/// <remarks>
/// Polyline objects define a series of connected line segments and are
/// typically used for paths or boundaries.
/// </remarks>
public class TilemapPolylineObject : TilemapObject
{
    /// <summary>
    /// Gets or sets the vertices of the polyline relative to the object's position.
    /// </summary>
    public Vector2[] Points { get; set; }

    /// <summary>
    /// Gets the vertices of the polyline in world coordinates.
    /// </summary>
    public Vector2[] WorldPoints
    {
        get
        {
            Vector2[] result = new Vector2[Points.Length];
            for (int i = 0; i < Points.Length; i++)
            {
                result[i] = Position + Points[i];
            }
            return result;
        }
    }

    /// <summary>
    /// Gets the line segments of the polyline in world coordinates.
    /// </summary>
    public LineSegment2D[] Segments
    {
        get
        {
            if (Points == null || Points.Length < 2)
            {
                return Array.Empty<LineSegment2D>();
            }

            LineSegment2D[] segments = new LineSegment2D[Points.Length - 1];
            for (int i = 0; i < Points.Length - 1; i++)
            {
                segments[i] = new LineSegment2D(Position + Points[i], Position + Points[i + 1]);
            }
            return segments;
        }
    }

    /// <inheritdoc/>
    public override BoundingBox2D Bounds
    {
        get
        {
            if (Points == null || Points.Length == 0)
            {
                return new BoundingBox2D(Position, Position);
            }

            return BoundingBox2D.CreateFromPoints(WorldPoints);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapPolylineObject"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the object.</param>
    /// <param name="position">The position of the object (origin point).</param>
    /// <param name="points">The vertices of the polyline relative to the position.</param>
    public TilemapPolylineObject(int id, Vector2 position, Vector2[] points) : base(id, position)
    {
        Points = points;
    }
}
