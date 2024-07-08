using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.3; Bounding Volumes - Spheres. pg 88

    /// <summary>
    ///     A two dimensional circle defined by a centre <see cref="Vector2" /> and a radius <see cref="float" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         An <see cref="CircleF" /> is categorized by the set of all points in a plane that are at equal distance from
    ///         the
    ///         centre.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IEquatableByRef{T}" />
    [DataContract]
    public struct CircleF : IEquatable<CircleF>, IEquatableByRef<CircleF>, IShapeF
    {
        /// <summary>
        ///     The centre position of this <see cref="CircleF" />.
        /// </summary>
        [DataMember] public Vector2 Center;

        /// <summary>
        ///     The distance from the <see cref="Center" /> point to any point on the boundary of this <see cref="CircleF" />.
        /// </summary>
        [DataMember] public float Radius;

        /// <summary>
        /// Gets or sets the position of the circle.
        /// </summary>
        public Vector2 Position
        {
            get => Center;
            set => Center = value;
        }

        public RectangleF BoundingRectangle
        {
            get
            {
                var minX = Center.X - Radius;
                var minY = Center.Y - Radius;
                return new RectangleF(minX, minY, Diameter, Diameter);
            }
        }

        /// <summary>
        ///     Gets the distance from a point to the opposite point, both on the boundary of this <see cref="CircleF" />.
        /// </summary>
        public float Diameter => 2 * Radius;

        /// <summary>
        ///     Gets the distance around the boundary of this <see cref="CircleF" />.
        /// </summary>
        public float Circumference => 2 * MathHelper.Pi * Radius;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircleF" /> structure from the specified centre
        ///     <see cref="Vector2" /> and the radius <see cref="float" />.
        /// </summary>
        /// <param name="center">The centre point.</param>
        /// <param name="radius">The radius.</param>
        public CircleF(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        ///     Computes the bounding <see cref="CircleF" /> from a minimum <see cref="Vector2" /> and maximum
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="minimum">The minimum point.</param>
        /// <param name="maximum">The maximum point.</param>
        /// <param name="result">The resulting circle.</param>
        public static void CreateFrom(Vector2 minimum, Vector2 maximum, out CircleF result)
        {
            result.Center = new Vector2((maximum.X + minimum.X) * 0.5f, (maximum.Y + minimum.Y) * 0.5f);
            var distanceVector = maximum - minimum;
            result.Radius = distanceVector.X > distanceVector.Y ? distanceVector.X * 0.5f : distanceVector.Y * 0.5f;
        }

        /// <summary>
        ///     Computes the bounding <see cref="CircleF" /> from a minimum <see cref="Vector2" /> and maximum
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="minimum">The minimum point.</param>
        /// <param name="maximum">The maximum point.</param>
        /// <returns>An <see cref="CircleF" />.</returns>
        public static CircleF CreateFrom(Vector2 minimum, Vector2 maximum)
        {
            CircleF result;
            CreateFrom(minimum, maximum, out result);
            return result;
        }

        /// <summary>
        ///     Computes the bounding <see cref="CircleF" /> from a list of <see cref="Vector2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="result">The resulting circle.</param>
        public static void CreateFrom(IReadOnlyList<Vector2> points, out CircleF result)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.3; Bounding Volumes - Spheres. pg 89-90

            if (points == null || points.Count == 0)
            {
                result = default(CircleF);
                return;
            }

            var minimum = new Vector2(float.MaxValue, float.MaxValue);
            var maximum = new Vector2(float.MinValue, float.MinValue);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = points.Count - 1; index >= 0; --index)
            {
                var point = points[index];
                minimum = MathExtended.CalculateMinimumVector2(minimum, point);
                maximum = MathExtended.CalculateMaximumVector2(maximum, point);
            }

            CreateFrom(minimum, maximum, out result);
        }

        /// <summary>
        ///     Computes the bounding <see cref="CircleF" /> from a list of <see cref="Vector2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>An <see cref="CircleF" />.</returns>
        public static CircleF CreateFrom(IReadOnlyList<Vector2> points)
        {
            CircleF result;
            CreateFrom(points, out result);
            return result;
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="CircleF" /> structures intersect.
        /// </summary>
        /// <param name="first">The first circle.</param>
        /// <param name="second">The second circle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(ref CircleF first, ref CircleF second)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.3; Bounding Volumes - Spheres. pg 88

            // Calculate squared distance between centers
            var distanceVector = first.Center - second.Center;
            var distanceSquared = distanceVector.Dot(distanceVector);
            var radiusSum = first.Radius + second.Radius;
            return distanceSquared <= radiusSum * radiusSum;
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="CircleF" /> structures intersect.
        /// </summary>
        /// <param name="first">The first circle.</param>
        /// <param name="second">The second circle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(CircleF first, CircleF second)
        {
            return Intersects(ref first, ref second);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="CircleF" /> intersects with this <see cref="CircleF" />.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="circle" /> intersects with this <see cref="CircleF" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(ref CircleF circle)
        {
            return Intersects(ref this, ref circle);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="CircleF" /> intersects with this <see cref="CircleF" />.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="circle" /> intersects with this <see cref="CircleF" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(CircleF circle)
        {
            return Intersects(ref this, ref circle);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="CircleF" /> and <see cref="BoundingRectangle" /> structures intersect.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="circle" /> intersects with the <see cref="rectangle" />; otherwise, <c>false</c>
        ///     .
        /// </returns>
        public static bool Intersects(ref CircleF circle, ref BoundingRectangle rectangle)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.25; Basic Primitives Test - Testing Sphere Against AABB. pg 165-166

            // Compute squared distance between sphere center and AABB boundary
            var distanceSquared = rectangle.SquaredDistanceTo(circle.Center);
            // Circle and AABB intersect if the (squared) distance between the AABB's boundary and the circle is less than the (squared) circle's radius
            return distanceSquared <= circle.Radius * circle.Radius;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="CircleF" /> and <see cref="BoundingRectangle" /> structures intersect.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="circle" /> intersects with the <see cref="rectangle" />; otherwise, <c>false</c>
        ///     .
        /// </returns>
        public static bool Intersects(CircleF circle, BoundingRectangle rectangle)
        {
            return Intersects(ref circle, ref rectangle);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="CircleF" /> intersects with this <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="rectangle" /> intersects with this <see cref="CircleF" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(ref BoundingRectangle rectangle)
        {
            return Intersects(ref this, ref rectangle);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="CircleF" /> intersects with this <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="rectangle" /> intersects with this <see cref="CircleF" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(BoundingRectangle rectangle)
        {
            return Intersects(ref this, ref rectangle);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="CircleF" /> contains the specified
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="circle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(ref CircleF circle, Vector2 point)
        {
            var dx = circle.Center.X - point.X;
            var dy = circle.Center.Y - point.Y;
            var d2 = dx * dx + dy * dy;
            var r2 = circle.Radius * circle.Radius;
            return d2 <= r2;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="CircleF" /> contains the specified
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="circle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(CircleF circle, Vector2 point)
        {
            return Contains(ref circle, point);
        }

        /// <summary>
        ///     Determines whether this <see cref="CircleF" /> contains the specified <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="BoundingRectangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Contains(Vector2 point)
        {
            return Contains(ref this, point);
        }

        /// <summary>
        ///     Computes the closest <see cref="Vector2" /> on this <see cref="CircleF" /> to a specified
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The closest <see cref="Vector2" /> on this <see cref="CircleF" /> to the <paramref name="point" />.</returns>
        public Vector2 ClosestPointTo(Vector2 point)
        {
            var distanceVector = point - Center;
            var lengthSquared = distanceVector.Dot(distanceVector);
            if (lengthSquared <= Radius * Radius)
                return point;
            distanceVector.Normalize();
            return Center + Radius * distanceVector;
        }

        /// <summary>
        ///     Computes the <see cref="Vector2" /> on the boundary of of this <see cref="CircleF" /> using the specified angle.
        /// </summary>
        /// <param name="angle">The angle in radians.</param>
        /// <returns>The <see cref="Vector2" /> on the boundary of this <see cref="CircleF" /> using <paramref name="angle" />.</returns>
        public Vector2 BoundaryPointAt(float angle)
        {
            var direction = new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));
            return Center + Radius * direction;
        }

        [Obsolete("Circle.GetPointAlongEdge() may be removed in the future. Use BoundaryPointAt() instead.")]
        public Vector2 GetPointAlongEdge(float angle)
        {
            return Center + new Vector2(Radius * (float) Math.Cos(angle), Radius * (float) Math.Sin(angle));
        }

        /// <summary>
        ///     Compares two <see cref="CircleF" /> structures. The result specifies whether the values of the
        ///     <see cref="Center" /> and <see cref="Radius" /> fields of the two <see cref="CircleF" /> structures
        ///     are equal.
        /// </summary>
        /// <param name="first">The first circle.</param>
        /// <param name="second">The second circle.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Center" /> and <see cref="Radius" /> fields of the two
        ///     <see cref="BoundingRectangle" /> structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(CircleF first, CircleF second)
        {
            return first.Equals(ref second);
        }

        /// <summary>
        ///     Compares two <see cref="CircleF" /> structures. The result specifies whether the values of the
        ///     <see cref="Center" /> and <see cref="Radius" /> fields of the two <see cref="CircleF" /> structures
        ///     are unequal.
        /// </summary>
        /// <param name="first">The first circle.</param>
        /// <param name="second">The second circle.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Center" /> and <see cref="Radius" /> fields of the two
        ///     <see cref="CircleF" /> structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(CircleF first, CircleF second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="CircleF" /> is equal to another <see cref="CircleF" />.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="CircleF" /> is equal to the <paramref name="circle" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(CircleF circle)
        {
            return Equals(ref circle);
        }

        /// <summary>
        ///     Indicates whether this <see cref="CircleF" /> is equal to another <see cref="CircleF" />.
        /// </summary>
        /// <param name="circle">The bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="CircleF" /> is equal to the <paramref name="circle" />;
        ///     otherwise,<c>false</c>.
        /// </returns>
        public bool Equals(ref CircleF circle)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return circle.Center == Center && circle.Radius == Radius;
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="CircleF" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="CircleF" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is CircleF && Equals((CircleF) obj);
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="CircleF" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="CircleF" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Center.GetHashCode() * 397) ^ Radius.GetHashCode();
            }
        }

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="CircleF" /> to a <see cref="Rectangle" />.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <returns>
        ///     The resulting <see cref="Rectangle" />.
        /// </returns>
        public static explicit operator Rectangle(CircleF circle)
        {
            var diameter = (int) circle.Diameter;
            return new Rectangle((int) (circle.Center.X - circle.Radius), (int) (circle.Center.Y - circle.Radius),
                diameter, diameter);
        }

        /// <summary>
        ///     Performs a conversion from a specified <see cref="CircleF" /> to a <see cref="Rectangle" />.
        /// </summary>
        /// <returns>
        ///     The resulting <see cref="Rectangle" />.
        /// </returns>
        public Rectangle ToRectangle()
        {
            return (Rectangle)this;
        }

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="Rectangle" /> to a <see cref="CircleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="CircleF" />.
        /// </returns>
        public static explicit operator CircleF(Rectangle rectangle)
        {
            var halfWidth = rectangle.Width / 2;
            var halfHeight = rectangle.Height / 2;
            return new CircleF(new Vector2(rectangle.X + halfWidth, rectangle.Y + halfHeight),
                halfWidth > halfHeight ? halfWidth : halfHeight);
        }

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="CircleF" /> to a <see cref="RectangleF" />.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <returns>
        ///     The resulting <see cref="RectangleF" />.
        /// </returns>
        public static explicit operator RectangleF(CircleF circle)
        {
            var diameter = circle.Diameter;
            return new RectangleF(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius, diameter, diameter);
        }

        /// <summary>
        ///     Performs a conversion from a specified <see cref="CircleF" /> to a <see cref="RectangleF" />.
        /// </summary>
        /// <returns>
        ///     The resulting <see cref="RectangleF" />.
        /// </returns>
        public RectangleF ToRectangleF()
        {
            return (RectangleF)this;
        }

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="RectangleF" /> to a <see cref="CircleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="CircleF" />.
        /// </returns>
        public static explicit operator CircleF(RectangleF rectangle)
        {
            var halfWidth = rectangle.Width * 0.5f;
            var halfHeight = rectangle.Height * 0.5f;
            return new CircleF(new Vector2(rectangle.X + halfWidth, rectangle.Y + halfHeight),
                halfWidth > halfHeight ? halfWidth : halfHeight);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="CircleF" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="CircleF" />.
        /// </returns>
        public override string ToString()
        {
            return $"Centre: {Center}, Radius: {Radius}";
        }

        internal string DebugDisplayString => ToString();
    }
}
