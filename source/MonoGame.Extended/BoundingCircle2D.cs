// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    /// Represents a circular bounding volume in 2D space, defined by a center point and radius.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct BoundingCircle2D : IEquatable<BoundingCircle2D>
    {
        #region Public Fields

        /// <summary>
        /// The center position of this circle in 2D space.
        /// </summary>
        [DataMember]
        public Vector2 Center;

        /// <summary>
        /// The radius of this circle, representing the distance from the center to any point on the edge.
        /// </summary>
        [DataMember]
        public float Radius;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the squared radius of this circle, useful for distance comparisons without square root calculations.
        /// </summary>
        public readonly float RadiusSquared => Radius * Radius;

        /// <summary>
        /// Gets the diameter of this circle, the distance across through the center.
        /// </summary>
        public readonly float Diameter => Radius * 2.0f;

        /// <summary>
        /// Gets the area enclosed by this circle.
        /// </summary>
        public readonly float Area => MathF.PI * Radius * Radius;

        #endregion

        #region Internal Properties

        internal string DebugDisplayString
        {
            get
            {
                return string.Concat(
                    "Center( ", Center.ToString(), " )  \r\n",
                    "Radius( ", Radius.ToString(), " )"
                );
            }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Creates a new <see cref="BoundingCircle2D"/> with the specified center and radius.
        /// </summary>
        /// <param name="center">The center position of the circle in 2D space.</param>
        /// <param name="radius">The radius of the circle in. Should be non-negative.</param>
        public BoundingCircle2D(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a <see cref="BoundingCircle2D"/> that encloses all specified points.
        /// </summary>
        /// <param name="points">The array of points to enclose within the circle.</param>
        /// <returns>
        /// A <see cref="BoundingCircle2D"/> that contains all the specified points.
        /// The circle is computed using an approximation algorithm and may not be the absolute minimal bounding circle.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="points"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="points"/> is empty.
        /// </exception>
        /// <remarks>
        /// Uses Ritter's algorithm for efficient approximate bounding circle computation.
        /// For a single point, returns a circle with radius <c>0</c> centered at that point.
        /// </remarks>
        public static BoundingCircle2D CreateFromPoints(Vector2[] points)
        {
            // C. Ericson, Real-Time Collision Detection, Morgan Kaufmann, 2005
            // Section 4.3.2 "Computing a Bounding Sphere"
            // Implements Ritter's algorithm for approximate bounding sphere computation

            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            if (points.Length == 0)
            {
                throw new ArgumentException("Cannot create bounding circle from empty point array", nameof(points));
            }

            if (points.Length == 1)
            {
                return new BoundingCircle2D(points[0], 0.0f);
            }

            // Find the most separate point pair along the principle axis
            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;

            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].X < points[minX].X) minX = i;
                if (points[i].X > points[maxX].X) maxX = i;
                if (points[i].Y < points[minY].Y) minY = i;
                if (points[i].Y > points[maxY].Y) maxY = i;
            }

            // Compute squared distances for the two pairs of points
            Vector2 dx = points[maxX] - points[minX];
            Vector2 dy = points[maxY] - points[minY];
            float distSqX = dx.X * dx.X + dx.Y * dx.Y;
            float distSqY = dy.X * dy.X + dy.Y * dy.Y;

            // Pick the pair of points most distant
            int min = minX;
            int max = maxX;
            if (distSqY > distSqX)
            {
                max = maxY;
                min = minY;
            }

            Vector2 center = (points[min] + points[max]) * 0.5f;
            Vector2 diff = points[max] - center;
            float radius = MathF.Sqrt(diff.X * diff.X + diff.Y * diff.Y);

            // Grow sphere to include all points
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 point = points[i];
                Vector2 d = point - center;
                float distSq = d.X * d.X + d.Y * d.Y;

                // Only update sphere if point is outside it
                if (distSq > radius * radius)
                {
                    float dist = MathF.Sqrt(distSq);
                    float newRadius = (radius + dist) * 0.5f;
                    float k = (newRadius - radius) / dist;

                    radius = newRadius;
                    center += d * k;
                }
            }

            return new BoundingCircle2D(center, radius);
        }

        /// <summary>
        /// Creates a <see cref="BoundingCircle2D"/> that completely encloses the specified bounding box.
        /// </summary>
        /// <param name="box">The bounding box to enclose within a circle.</param>
        /// <returns>
        /// A <see cref="BoundingCircle2D"/> centered at the bounding box's center with radius equal to half the diagonal of the box.
        /// </returns>
        public static BoundingCircle2D CreateFromBoundingBox2D(BoundingBox2D box)
        {
            Vector2 center = box.Center;
            Vector2 halfExtents = box.HalfExtents;
            float radius = halfExtents.Length();

            return new BoundingCircle2D(center, radius);
        }

        /// <summary>
        /// Creates a <see cref="BoundingCircle2D"/> that completely encloses the specified capsule.
        /// </summary>
        /// <param name="capsule">The capsule to enclose within a circle.</param>
        /// <returns>
        /// A <see cref="BoundingCircle2D"/> centered at the capsule's center with radius sufficient to contain both endpoint caps.
        /// </returns>
        /// <remarks>
        /// The radius is computed as half the capsule's length plus the capsule's radius, ensuring that
        /// the circle reaches the farthest points on both circular caps.
        /// </remarks>
        public static BoundingCircle2D CreateFromBoundingCapsule2D(BoundingCapsule2D capsule)
        {
            // The bounding circle center is at the capsule's midpoint
            Vector2 center = capsule.Center;

            // The radius needs to reach from the center to the farthest point on either cap
            // This is half the capsule length plus the cap radius
            float radius = (capsule.Length * 0.5f) + capsule.Radius;

            return new BoundingCircle2D(center, radius);
        }

        /// <summary>
        /// Creates a <see cref="BoundingCircle2D"/> that encloses two bounding circles.
        /// </summary>
        /// <param name="original">The first bounding circle to enclose.</param>
        /// <param name="additional">The second bounding circle to enclose.</param>
        /// <returns>
        /// A <see cref="BoundingCircle2D"/> with the smallest radius that completely contains both input circles.
        /// </returns>
        /// <remarks>
        /// If one circle completely contains the other, the larger circle is returned.
        /// Otherwise, computes the optimal merged circle that tightly encloses both.
        /// </remarks>
        public static BoundingCircle2D CreateMerged(BoundingCircle2D original, BoundingCircle2D additional)
        {
            // C. Ericson, Real-Time Collision Detection, Morgan Kaufmann, 2005
            // Section 6.5.2 "Merging Two Spheres" (2D circle adaptation)

            // Calculate distance between centers
            Vector2 centerDiff = additional.Center - original.Center;
            float distSq = centerDiff.X * centerDiff.X + centerDiff.Y * centerDiff.Y;
            float dist = MathF.Sqrt(distSq);

            // Calculate radius difference
            float radiusDiff = MathF.Abs(additional.Radius - original.Radius);

            // Check if one circle contain the other
            if (radiusDiff >= dist)
            {
                // One circle fully enclosed by the other
                // return the larger circle
                if (additional.Radius >= original.Radius)
                    return additional;
                else
                    return original;
            }

            // Circles are partially overlapping or disjoint
            // Calculate the radius of the new circle as half the maximum distance between surfaces
            float radius = (dist + original.Radius + additional.Radius) * 0.5f;

            // Calculate the center by adjusting original center toward additional center
            Vector2 center = original.Center;
            if (dist > Collision2D.Epsilon)
            {
                center += ((radius - original.Radius) / dist) * centerDiff;
            }

            return new BoundingCircle2D(center, radius);
        }

        /// <summary>
        /// Tests whether a point lies inside this circle or on its boundary.
        /// </summary>
        /// <param name="point">The point to test in 2D space.</param>
        /// <returns>
        /// <see cref="ContainmentType.Contains"/> if the point is inside or on the boundary;
        /// otherwise, <see cref="ContainmentType.Disjoint"/> if the point is outside.
        /// </returns>
        public readonly ContainmentType Contains(Vector2 point)
        {
            return Collision2D.ContainsCirclePoint(point, Center, Radius);
        }

        /// <summary>
        /// Tests whether this circle contains, intersects, or is separate from a bounding box.
        /// </summary>
        /// <param name="box">The bounding box to test against.</param>
        /// <returns>
        /// <see cref="ContainmentType.Contains"/> if the bounding box is completely inside this circle;
        /// <see cref="ContainmentType.Intersects"/> if they partially overlap;
        /// or <see cref="ContainmentType.Disjoint"/> if they do not touch.
        /// </returns>
        public readonly ContainmentType Contains(BoundingBox2D box)
        {
            return Collision2D.ContainsCircleAabb(Center, Radius, box.Min, box.Max);
        }

        /// <summary>
        /// Tests whether this circle contains, intersects, or is separate from another circle.
        /// </summary>
        /// <param name="other">The other circle to test against.</param>
        /// <returns>
        /// <see cref="ContainmentType.Contains"/> if the other circle is completely inside this one;
        /// <see cref="ContainmentType.Intersects"/> if they partially overlap;
        /// or <see cref="ContainmentType.Disjoint"/> if they do not touch.
        /// </returns>
        public readonly ContainmentType Contains(BoundingCircle2D other)
        {
            return Collision2D.ContainsCircleCircle(Center, Radius, other.Center, other.Radius);
        }

        /// <summary>
        /// Tests whether this circle contains, intersects, or is separate from an oriented bounding box
        /// </summary>
        /// <param name="obb">The oriented bounding box to test against.</param>
        /// <returns>
        /// <see cref="ContainmentType.Contains"/> if the oriented bounding box is completely inside this circle;
        /// <see cref="ContainmentType.Intersects"/> if they partially overlap;
        /// or <see cref="ContainmentType.Disjoint"/> if they do not touch.
        /// </returns>
        public readonly ContainmentType Contains(OrientedBoundingBox2D obb)
        {
            return Collision2D.ContainsCircleObb(Center, Radius, obb.Center, obb.AxisX, obb.AxisY, obb.HalfExtents);
        }

        /// <summary>
        /// Tests whether this circle contains, intersects, or is separate from a capsule.
        /// </summary>
        /// <param name="capsule">The capsule to test against.</param>
        /// <returns>
        /// <see cref="ContainmentType.Contains"/> if the capsule is completely inside this circle;
        /// <see cref="ContainmentType.Intersects"/> if they partially overlap;
        /// or <see cref="ContainmentType.Disjoint"/> if they do not touch.
        /// </returns>
        public readonly ContainmentType Contains(BoundingCapsule2D capsule)
        {
            return Collision2D.ContainsCircleCapsule(Center, Radius, capsule.PointA, capsule.PointB, capsule.Radius);
        }

        /// <summary>
        /// Tests whether this circle contains, intersects, or is separate from a polygon.
        /// </summary>
        /// <param name="polygon">The capsule to test against.</param>
        /// <returns>
        /// <see cref="ContainmentType.Contains"/> if the polygon is completely inside this circle;
        /// <see cref="ContainmentType.Intersects"/> if they partially overlap;
        /// or <see cref="ContainmentType.Disjoint"/> if they do not touch.
        /// </returns>
        public readonly ContainmentType Contains(BoundingPolygon2D polygon)
        {
            return Collision2D.ContainsCircleConvexPolygon(Center, Radius, polygon.Vertices, polygon.Normals);
        }

        /// <summary>
        /// Tests whether this circle intersects with another circle.
        /// </summary>
        /// <param name="other">The other circle to test against.</param>
        /// <returns>
        /// <see langword="true"/> if the circles overlap or touch; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool Intersects(BoundingCircle2D other)
        {
            return Collision2D.IntersectsCircleCircle(Center, Radius, other.Center, other.Radius);
        }

        /// <summary>
        /// Tests whether this circle intersects with an axis-aligned bounding box.
        /// </summary>
        /// <param name="box">The bounding box to test against.</param>
        /// <returns>
        /// <see langword="true"/> if the circle and bounding box overlap or touch; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool Intersects(BoundingBox2D box)
        {
            return Collision2D.IntersectsCircleAabb(Center, Radius, box.Min, box.Max);
        }

        /// <summary>
        /// Tests whether this circle intersects with a capsule.
        /// </summary>
        /// <param name="capsule">The capsule to test against.</param>
        /// <returns>
        /// <see langword="true"/> if the circle and capsule overlap or touch; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool Intersects(BoundingCapsule2D capsule)
        {
            return Collision2D.IntersectsCircleCapsule(Center, Radius, capsule.PointA, capsule.PointB, capsule.Radius);
        }

        /// <summary>
        /// Tests whether this circle intersects with an oriented bounding box.
        /// </summary>
        /// <param name="obb">The oriented bounding box to test against.</param>
        /// <returns>
        /// <see langword="true"/> if the circle and box overlap or touch; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool Intersects(OrientedBoundingBox2D obb)
        {
            return Collision2D.IntersectsCircleObb(Center, Radius, obb.Center, obb.AxisX, obb.AxisY, obb.HalfExtents);
        }

        /// <summary>
        /// Tests whether this circle intersects with a polygon.
        /// </summary>
        /// <param name="polygon">The polygon to test against.</param>
        /// <returns>
        /// <see langword="true"/> if the circle and polygon overlap or touch; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool Intersects(BoundingPolygon2D polygon)
        {
            return Collision2D.IntersectsCircleConvexPolygon(Center, Radius, polygon.Vertices, polygon.Normals);
        }

        /// <summary>
        /// Tests whether this circle intersects with another circle, and returns collision resolution data when they intersect.
        /// </summary>
        /// <param name="other">The other circle to test against.</param>
        /// <param name="result">
        /// When this method returns <see langword="true"/>, contains the collision result whose minimum translation vector
        /// moves this circle out of <paramref name="other"/>. When this method returns <see langword="false"/>, contains
        /// <see cref="CollisionResult2D.None"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the circles overlap or touch; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool TryGetCollision(BoundingCircle2D other, out CollisionResult2D result)
        {
            return Collision2D.TryGetCollisionCircleCircle(Center, Radius, other.Center, other.Radius, out result);
        }

        /// <summary>
        /// Tests whether this circle intersects with an axis-aligned bounding box, and returns collision resolution data
        /// when they intersect.
        /// </summary>
        /// <param name="box">The bounding box to test against.</param>
        /// <param name="result">
        /// When this method returns <see langword="true"/>, contains the collision result whose minimum translation vector
        /// moves this circle out of <paramref name="box"/>. When this method returns <see langword="false"/>, contains
        /// <see cref="CollisionResult2D.None"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the circle and bounding box overlap or touch; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool TryGetCollision(BoundingBox2D box, out CollisionResult2D result)
        {
            return Collision2D.TryGetCollisionCircleAabb(Center, Radius, box.Min, box.Max, out result);
        }

        /// <summary>
        /// Tests whether this circle intersects with a capsule, and returns collision resolution data when they intersect.
        /// </summary>
        /// <param name="capsule">The capsule to test against.</param>
        /// <param name="result">
        /// When this method returns <see langword="true"/>, contains the collision result whose minimum translation vector
        /// moves this circle out of <paramref name="capsule"/>. When this method returns <see langword="false"/>, contains
        /// <see cref="CollisionResult2D.None"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the circle and capsule overlap or touch; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool TryGetCollision(BoundingCapsule2D capsule, out CollisionResult2D result)
        {
            return Collision2D.TryGetCollisionCircleCapsule(Center, Radius, capsule.PointA, capsule.PointB, capsule.Radius, out result);
        }

        /// <summary>
        /// Tests whether this circle intersects with an oriented bounding box, and returns collision resolution data
        /// when they intersect.
        /// </summary>
        /// <param name="obb">The oriented bounding box to test against.</param>
        /// <param name="result">
        /// When this method returns <see langword="true"/>, contains the collision result whose minimum translation vector
        /// moves this circle out of <paramref name="obb"/>. When this method returns <see langword="false"/>, contains
        /// <see cref="CollisionResult2D.None"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the circle and box overlap or touch; otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool TryGetCollision(OrientedBoundingBox2D obb, out CollisionResult2D result)
        {
            return Collision2D.TryGetCollisionCircleObb(Center, Radius, obb.Center, obb.AxisX, obb.AxisY, obb.HalfExtents, out result);
        }

        /// <summary>
        /// Applies a matrix transformation to this circle and creates a new transformed circle.
        /// </summary>
        /// <param name="matrix">The transformation matrix to apply.</param>
        /// <returns>
        /// A new <see cref="BoundingCircle2D"/> with the center transformed by the matrix and the radius
        /// scaled by the maximum scale factor to ensure the transformed shape remains enclosed.
        /// </returns>
        /// <remarks>
        /// The radius is scaled by the maximum of the X and Y scale components of the transformation matrix.
        /// This ensures that the resulting circle fully encloses the transformed original circle, even if
        /// the transformation includes non-uniform scaling or rotation.
        /// </remarks>
        public readonly BoundingCircle2D Transform(Matrix matrix)
        {
            BoundingCircle2D circle = new BoundingCircle2D();
            circle.Center = Vector2.Transform(Center, matrix);

            // Scale radius by maximum scale component
            float scaleX = MathF.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);
            float scaleY = MathF.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22);
            circle.Radius = Radius * MathF.Max(scaleX, scaleY);

            return circle;
        }

        /// <summary>
        /// Creates a new <see cref="BoundingCircle2D"/> by translating this circle by the specified offset.
        /// </summary>
        /// <param name="translation">The offset to translate the circle by in 2D space.</param>
        /// <returns>
        /// A new <see cref="BoundingCircle2D"/> at the translated position with the same radius.
        /// </returns>
        public readonly BoundingCircle2D Translate(Vector2 translation)
        {
            return new BoundingCircle2D(Center + translation, Radius);
        }

        /// <summary>
        /// Deconstructs this circle into its component values.
        /// </summary>
        /// <param name="center">
        /// When this method returns, contains the center position of this circle in 2D space.
        /// </param>
        /// <param name="radius">
        /// When this method returns, contains the radius of this circle in.
        /// </param>
        public readonly void Deconstruct(out Vector2 center, out float radius)
        {
            center = Center;
            radius = Radius;
        }

        /// <inheritdoc/>
        public readonly bool Equals(BoundingCircle2D other)
        {
            return Center.Equals(other.Center) && Radius.Equals(other.Radius);
        }

        /// <inheritdoc/>
        public override readonly bool Equals(object obj)
        {
            return obj is BoundingCircle2D other && Equals(other);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return Center.GetHashCode() ^ Radius.GetHashCode();
        }

        /// <inheritdoc/>
        public override readonly string ToString()
        {
            return $"{{Center:{Center} Radius:{Radius}}}";
        }

        /// <summary/>
        public static bool operator ==(BoundingCircle2D left, BoundingCircle2D right)
        {
            return left.Equals(right);
        }

        /// <summary/>
        public static bool operator !=(BoundingCircle2D left, BoundingCircle2D right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}
