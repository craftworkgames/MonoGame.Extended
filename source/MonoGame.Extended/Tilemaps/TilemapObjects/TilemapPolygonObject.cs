using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a closed polygonal object in a tilemap.
/// </summary>
public class TilemapPolygonObject : TilemapObject
{
    /// <summary>
    /// Gets or sets the vertices of the polygon relative to the object's position.
    /// </summary>
    public Vector2[] Points { get; set; }

    /// <summary>
    /// Gets the vertices of the polygon in world coordinates.
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
    /// Gets the convex polygon bounding shape for this object in world coordinates.
    /// </summary>
    /// <remarks>
    /// <see cref="BoundingPolygon2D"/> only supports convex polygons. If this object's polygon
    /// is concave, collision results will be incorrect. Use <see cref="WorldPoints"/> to access
    /// the exact geometry for concave polygons.
    /// </remarks>
    public BoundingPolygon2D Shape
    {
        get
        {
            Vector2[] verts = WorldPoints;

            // BoundingPolygon2D requires counter-clockwise winding.
            float signedArea = 0f;
            for (int i = 0; i < verts.Length; i++)
            {
                signedArea += Vector2Extensions.PerpDot(verts[i], verts[(i + 1) % verts.Length]);
            }

            if (signedArea < 0f)
            {
                Array.Reverse(verts);
            }

            return new BoundingPolygon2D(verts);
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
    /// Initializes a new instance of the <see cref="TilemapPolygonObject"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the object.</param>
    /// <param name="position">The position of the object (origin point).</param>
    /// <param name="points">The vertices of the polygon relative to the position.</param>
    public TilemapPolygonObject(int id, Vector2 position, Vector2[] points) : base(id, position)
    {
        Points = points;
    }
}
