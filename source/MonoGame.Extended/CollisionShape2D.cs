using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended;

/// <summary>
/// Represents a collision shape for two-dimensional intersection and collision queries.
/// </summary>
/// <remarks>
/// The default value represents a <c>None</c> shape and does not intersect any other shape.
/// Convex polygon support is limited to polygons that satisfy the requirements of
/// <see cref="BoundingPolygon2D"/>, including convexity and consistent winding for vertices and normals.
/// </remarks>
public readonly struct CollisionShape2D
{
    private readonly CollisionShapeKind2D _kind;
    private readonly BoundingBox2D _boundingBox;
    private readonly Vector2 _primary;
    private readonly Vector2 _secondary;
    private readonly Vector2 _tertiary;
    private readonly float _scalar;
    private readonly Vector2[] _polygonVertices;
    private readonly Vector2[] _polygonNormals;

    /// <summary>
    /// Gets the axis-aligned broadphase bounds for this collision shape.
    /// </summary>
    /// <value>
    /// The enclosing axis-aligned bounding box for the represented shape. For the default <c>None</c> shape,
    /// this value is the default <see cref="BoundingBox2D"/>.
    /// </value>
    public BoundingBox2D BoundingBox
    {
        get
        {
            return _boundingBox;
        }
    }

    private BoundingCircle2D Circle
    {
        get
        {
            return new BoundingCircle2D(_primary, _scalar);
        }
    }

    private OrientedBoundingBox2D OrientedBox
    {
        get
        {
            Vector2 axisY = new Vector2(-_secondary.Y, _secondary.X);
            return new OrientedBoundingBox2D(_primary, _secondary, axisY, _tertiary);
        }
    }

    private BoundingCapsule2D Capsule
    {
        get
        {
            return new BoundingCapsule2D(_primary, _secondary, _scalar);
        }
    }

    private BoundingPolygon2D Polygon
    {
        get
        {
            return new BoundingPolygon2D(_polygonVertices, _polygonNormals);
        }
    }

    /// <summary>
    /// Creates a collision shape from an axis-aligned bounding box.
    /// </summary>
    /// <param name="box">The axis-aligned bounding box to represent.</param>
    /// <remarks>
    /// The supplied box is stored directly and also serves as the broadphase bounds for this shape.
    /// </remarks>
    public CollisionShape2D(BoundingBox2D box)
    {
        _kind = CollisionShapeKind2D.Box;
        _boundingBox = box;
        _primary = Vector2.Zero;
        _secondary = Vector2.Zero;
        _tertiary = Vector2.Zero;
        _scalar = 0.0f;
        _polygonVertices = null;
        _polygonNormals = null;
    }

    /// <summary>
    /// Creates a collision shape from a bounding circle.
    /// </summary>
    /// <param name="circle">The bounding circle to represent.</param>
    /// <remarks>
    /// The broadphase bounds are cached from the circle center and radius at construction time.
    /// </remarks>
    public CollisionShape2D(BoundingCircle2D circle)
    {
        Vector2 radiusVector = new Vector2(circle.Radius, circle.Radius);

        _kind = CollisionShapeKind2D.Circle;
        _boundingBox = BoundingBox2D.CreateFromMinMax(circle.Center - radiusVector, circle.Center + radiusVector);
        _primary = circle.Center;
        _secondary = Vector2.Zero;
        _tertiary = Vector2.Zero;
        _scalar = circle.Radius;
        _polygonVertices = null;
        _polygonNormals = null;
    }

    /// <summary>
    /// Creates a collision shape from an oriented bounding box.
    /// </summary>
    /// <param name="box">The oriented bounding box to represent.</param>
    /// <remarks>
    /// The broadphase bounds are cached from the oriented box corners at construction time.
    /// The stored orientation assumes a valid orthonormal basis and derives the Y axis from the X axis.
    /// </remarks>
    public CollisionShape2D(OrientedBoundingBox2D box)
    {
        _kind = CollisionShapeKind2D.OrientedBox;
        _boundingBox = BoundingBox2D.CreateFromPoints(box.GetCorners());
        _primary = box.Center;
        _secondary = box.AxisX;
        _tertiary = box.HalfExtents;
        _scalar = 0.0f;
        _polygonVertices = null;
        _polygonNormals = null;
    }

    /// <summary>
    /// Creates a collision shape from a bounding capsule.
    /// </summary>
    /// <param name="capsule">The bounding capsule to represent.</param>
    /// <remarks>
    /// The broadphase bounds are cached from the segment endpoints expanded by the capsule radius.
    /// </remarks>
    public CollisionShape2D(BoundingCapsule2D capsule)
    {
        Vector2 radiusVector = new Vector2(capsule.Radius, capsule.Radius);
        Vector2 min = Vector2.Min(capsule.PointA, capsule.PointB) - radiusVector;
        Vector2 max = Vector2.Max(capsule.PointA, capsule.PointB) + radiusVector;

        _kind = CollisionShapeKind2D.Capsule;
        _boundingBox = BoundingBox2D.CreateFromMinMax(min, max);
        _primary = capsule.PointA;
        _secondary = capsule.PointB;
        _tertiary = Vector2.Zero;
        _scalar = capsule.Radius;
        _polygonVertices = null;
        _polygonNormals = null;
    }

    /// <summary>
    /// Creates a collision shape from a convex bounding polygon.
    /// </summary>
    /// <param name="polygon">The convex bounding polygon to represent.</param>
    /// <remarks>
    /// The polygon is expected to satisfy the requirements of <see cref="BoundingPolygon2D"/>,
    /// including convexity and counter-clockwise winding.
    /// Polygon data remains reference-backed by design to avoid copying arbitrary-size arrays
    /// into this value type.
    /// </remarks>
    public CollisionShape2D(BoundingPolygon2D polygon)
    {
        _kind = CollisionShapeKind2D.Polygon;
        _boundingBox = BoundingBox2D.CreateFromPoints(polygon.Vertices);
        _primary = Vector2.Zero;
        _secondary = Vector2.Zero;
        _tertiary = Vector2.Zero;
        _scalar = 0.0f;
        _polygonVertices = polygon.Vertices;
        _polygonNormals = polygon.Normals;
    }

    /// <summary>
    /// Determines whether this collision shape intersects with another collision shape.
    /// </summary>
    /// <param name="other">The other collision shape to test against.</param>
    /// <returns>
    /// <see langword="true"/> if the shapes overlap or touch; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This method delegates to the existing bounding volume intersection APIs.
    /// If either shape is the default <c>None</c> shape, this method returns <see langword="false"/>.
    /// </remarks>
    public bool Intersects(CollisionShape2D other)
    {
        if (_kind == CollisionShapeKind2D.None || other._kind == CollisionShapeKind2D.None)
        {
            return false;
        }

        switch (_kind)
        {
            case CollisionShapeKind2D.Box:
                switch (other._kind)
                {
                    case CollisionShapeKind2D.Box:
                        return _boundingBox.Intersects(other._boundingBox);
                    case CollisionShapeKind2D.Circle:
                        return _boundingBox.Intersects(other.Circle);
                    case CollisionShapeKind2D.OrientedBox:
                        return _boundingBox.Intersects(other.OrientedBox);
                    case CollisionShapeKind2D.Capsule:
                        return _boundingBox.Intersects(other.Capsule);
                    case CollisionShapeKind2D.Polygon:
                        return _boundingBox.Intersects(other.Polygon);
                    default:
                        return false;
                }

            case CollisionShapeKind2D.Circle:
                switch (other._kind)
                {
                    case CollisionShapeKind2D.Box:
                        return Circle.Intersects(other._boundingBox);
                    case CollisionShapeKind2D.Circle:
                        return Circle.Intersects(other.Circle);
                    case CollisionShapeKind2D.OrientedBox:
                        return Circle.Intersects(other.OrientedBox);
                    case CollisionShapeKind2D.Capsule:
                        return Circle.Intersects(other.Capsule);
                    case CollisionShapeKind2D.Polygon:
                        return Circle.Intersects(other.Polygon);
                    default:
                        return false;
                }

            case CollisionShapeKind2D.OrientedBox:
                switch (other._kind)
                {
                    case CollisionShapeKind2D.Box:
                        return OrientedBox.Intersects(other._boundingBox);
                    case CollisionShapeKind2D.Circle:
                        return OrientedBox.Intersects(other.Circle);
                    case CollisionShapeKind2D.OrientedBox:
                        return OrientedBox.Intersects(other.OrientedBox);
                    case CollisionShapeKind2D.Capsule:
                        return OrientedBox.Intersects(other.Capsule);
                    case CollisionShapeKind2D.Polygon:
                        return OrientedBox.Intersects(other.Polygon);
                    default:
                        return false;
                }

            case CollisionShapeKind2D.Capsule:
                switch (other._kind)
                {
                    case CollisionShapeKind2D.Box:
                        return Capsule.Intersects(other._boundingBox);
                    case CollisionShapeKind2D.Circle:
                        return Capsule.Intersects(other.Circle);
                    case CollisionShapeKind2D.OrientedBox:
                        return Capsule.Intersects(other.OrientedBox);
                    case CollisionShapeKind2D.Capsule:
                        return Capsule.Intersects(other.Capsule);
                    case CollisionShapeKind2D.Polygon:
                        return Capsule.Intersects(other.Polygon);
                    default:
                        return false;
                }

            case CollisionShapeKind2D.Polygon:
                switch (other._kind)
                {
                    case CollisionShapeKind2D.Box:
                        return Polygon.Intersects(other._boundingBox);
                    case CollisionShapeKind2D.Circle:
                        return Polygon.Intersects(other.Circle);
                    case CollisionShapeKind2D.OrientedBox:
                        return Polygon.Intersects(other.OrientedBox);
                    case CollisionShapeKind2D.Capsule:
                        return Polygon.Intersects(other.Capsule);
                    case CollisionShapeKind2D.Polygon:
                        return Polygon.Intersects(other.Polygon);
                    default:
                        return false;
                }

            default:
                return false;
        }
    }

    /// <summary>
    /// Tests whether this collision shape intersects with another collision shape, and returns collision resolution data
    /// when the represented shape pair supports it.
    /// </summary>
    /// <param name="other">The other collision shape to test against.</param>
    /// <param name="result">
    /// When this method returns <see langword="true"/>, contains the collision result whose minimum translation vector
    /// moves this shape out of <paramref name="other"/>. When this method returns <see langword="false"/>, contains
    /// <see cref="CollisionResult2D.None"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the shapes overlap or touch and the represented pair supports collision-result queries;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This method does not add new collision-resolution algorithms.
    /// It delegates only to existing shape-pair APIs that already return <see cref="CollisionResult2D"/>.
    /// If either shape is the default <c>None</c> shape, or if the represented pair does not support
    /// collision-result queries, this method returns <see langword="false"/> and sets <paramref name="result"/>
    /// to <see cref="CollisionResult2D.None"/>.
    /// </remarks>
    public bool TryGetCollision(CollisionShape2D other, out CollisionResult2D result)
    {
        if (_kind == CollisionShapeKind2D.None || other._kind == CollisionShapeKind2D.None)
        {
            result = CollisionResult2D.None;
            return false;
        }

        switch (_kind)
        {
            case CollisionShapeKind2D.Box:
                switch (other._kind)
                {
                    case CollisionShapeKind2D.Box:
                        return _boundingBox.TryGetCollision(other._boundingBox, out result);
                    case CollisionShapeKind2D.Circle:
                        return _boundingBox.TryGetCollision(other.Circle, out result);
                    case CollisionShapeKind2D.OrientedBox:
                        return _boundingBox.TryGetCollision(other.OrientedBox, out result);
                    case CollisionShapeKind2D.Polygon:
                        return _boundingBox.TryGetCollision(other.Polygon, out result);
                    default:
                        result = CollisionResult2D.None;
                        return false;
                }

            case CollisionShapeKind2D.Circle:
                switch (other._kind)
                {
                    case CollisionShapeKind2D.Box:
                        return Circle.TryGetCollision(other._boundingBox, out result);
                    case CollisionShapeKind2D.Circle:
                        return Circle.TryGetCollision(other.Circle, out result);
                    case CollisionShapeKind2D.Capsule:
                        return Circle.TryGetCollision(other.Capsule, out result);
                    case CollisionShapeKind2D.OrientedBox:
                        return Circle.TryGetCollision(other.OrientedBox, out result);
                    default:
                        result = CollisionResult2D.None;
                        return false;
                }

            case CollisionShapeKind2D.OrientedBox:
                switch (other._kind)
                {
                    case CollisionShapeKind2D.Box:
                        return OrientedBox.TryGetCollision(other._boundingBox, out result);
                    case CollisionShapeKind2D.Circle:
                        return OrientedBox.TryGetCollision(other.Circle, out result);
                    case CollisionShapeKind2D.OrientedBox:
                        return OrientedBox.TryGetCollision(other.OrientedBox, out result);
                    case CollisionShapeKind2D.Polygon:
                        return OrientedBox.TryGetCollision(other.Polygon, out result);
                    default:
                        result = CollisionResult2D.None;
                        return false;
                }

            case CollisionShapeKind2D.Capsule:
                switch (other._kind)
                {
                    case CollisionShapeKind2D.Circle:
                        return Capsule.TryGetCollision(other.Circle, out result);
                    default:
                        result = CollisionResult2D.None;
                        return false;
                }

            case CollisionShapeKind2D.Polygon:
                switch (other._kind)
                {
                    case CollisionShapeKind2D.Box:
                        return Polygon.TryGetCollision(other._boundingBox, out result);
                    case CollisionShapeKind2D.OrientedBox:
                        return Polygon.TryGetCollision(other.OrientedBox, out result);
                    case CollisionShapeKind2D.Polygon:
                        return Polygon.TryGetCollision(other.Polygon, out result);
                    default:
                        result = CollisionResult2D.None;
                        return false;
                }

            default:
                result = CollisionResult2D.None;
                return false;
        }
    }

    internal bool TryGetLegacyPenetrationVector(CollisionShape2D other, out Vector2 penetrationVector)
    {
        if (_kind == CollisionShapeKind2D.Circle && other._kind == CollisionShapeKind2D.Circle)
        {
            penetrationVector = GetLegacyCircleCirclePenetrationVector(Circle, other.Circle);
            return true;
        }

        if (_kind == CollisionShapeKind2D.Circle && other._kind == CollisionShapeKind2D.Box)
        {
            penetrationVector = GetLegacyCircleBoxPenetrationVector(Circle, other._boundingBox);
            return true;
        }

        if (_kind == CollisionShapeKind2D.Box && other._kind == CollisionShapeKind2D.Circle)
        {
            penetrationVector = -GetLegacyCircleBoxPenetrationVector(other.Circle, _boundingBox);
            return true;
        }

        if (_kind == CollisionShapeKind2D.Box && other._kind == CollisionShapeKind2D.Box)
        {
            penetrationVector = GetLegacyBoxBoxPenetrationVector(_boundingBox, other._boundingBox);
            return true;
        }

        penetrationVector = Vector2.Zero;
        return false;
    }

    private static Vector2 GetLegacyCircleCirclePenetrationVector(BoundingCircle2D first, BoundingCircle2D second)
    {
        Vector2 displacement = first.Center - second.Center;
        Vector2 desiredDisplacement;

        if (displacement != Vector2.Zero)
        {
            desiredDisplacement = displacement.NormalizedCopy() * (first.Radius + second.Radius);
        }
        else
        {
            desiredDisplacement = -Vector2.UnitY * (first.Radius + second.Radius);
        }

        return displacement - desiredDisplacement;
    }

    private static Vector2 GetLegacyCircleBoxPenetrationVector(BoundingCircle2D circle, BoundingBox2D box)
    {
        Vector2 collisionPoint = GetClosestPoint(box, circle.Center);
        Vector2 circleToCollisionPoint = collisionPoint - circle.Center;

        if (Contains(box, circle.Center) || circleToCollisionPoint == Vector2.Zero)
        {
            Vector2 displacement = circle.Center - box.Center;
            Vector2 desiredDisplacement;

            if (displacement != Vector2.Zero)
            {
                Vector2 displacementX = new Vector2(displacement.X, 0.0f);
                Vector2 displacementY = new Vector2(0.0f, displacement.Y);
                displacementX.Normalize();
                displacementY.Normalize();

                displacementX *= circle.Radius + box.Width * 0.5f;
                displacementY *= circle.Radius + box.Height * 0.5f;

                if (displacementX.LengthSquared() < displacementY.LengthSquared())
                {
                    desiredDisplacement = displacementX;
                    displacement.Y = 0.0f;
                }
                else
                {
                    desiredDisplacement = displacementY;
                    displacement.X = 0.0f;
                }
            }
            else
            {
                desiredDisplacement = -Vector2.UnitY * (circle.Radius + box.Height * 0.5f);
            }

            return displacement - desiredDisplacement;
        }

        return circle.Radius * circleToCollisionPoint.NormalizedCopy() - circleToCollisionPoint;
    }

    private static Vector2 GetLegacyBoxBoxPenetrationVector(BoundingBox2D first, BoundingBox2D second)
    {
        BoundingBox2D intersection = new BoundingBox2D(
            new Vector2(Math.Max(first.Min.X, second.Min.X), Math.Max(first.Min.Y, second.Min.Y)),
            new Vector2(Math.Min(first.Max.X, second.Max.X), Math.Min(first.Max.Y, second.Max.Y)));

        if (intersection.Width < intersection.Height)
        {
            float displacement = first.Center.X < second.Center.X
                ? intersection.Width
                : -intersection.Width;

            return new Vector2(displacement, 0.0f);
        }

        {
            float displacement = first.Center.Y < second.Center.Y
                ? intersection.Height
                : -intersection.Height;

            return new Vector2(0.0f, displacement);
        }
    }

    private static bool Contains(BoundingBox2D box, Vector2 point)
    {
        return point.X >= box.Min.X && point.X <= box.Max.X &&
               point.Y >= box.Min.Y && point.Y <= box.Max.Y;
    }

    private static Vector2 GetClosestPoint(BoundingBox2D box, Vector2 point)
    {
        float x = MathHelper.Clamp(point.X, box.Min.X, box.Max.X);
        float y = MathHelper.Clamp(point.Y, box.Min.Y, box.Max.Y);
        return new Vector2(x, y);
    }
}
