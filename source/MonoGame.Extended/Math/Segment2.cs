using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 3.5; A Math and Geometry Primer - Lines, Rays, and Segments. pg 53-54
    /// <summary>
    ///     A two dimensional line segment defined by two <see cref="Vector2" /> structures, a starting point and an ending
    ///     point.
    /// </summary>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IEquatableByRef{Segment2}" />
    public struct Segment2 : IEquatable<Segment2>, IEquatableByRef<Segment2>
    {
        /// <summary>
        ///     The starting <see cref="Vector2" /> of this <see cref="Segment2" />.
        /// </summary>
        public Vector2 Start;

        /// <summary>
        ///     The ending <see cref="Vector2" /> of this <see cref="Segment2" />.
        /// </summary>
        public Vector2 End;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Segment2" /> structure from the specified starting and ending
        ///     <see cref="Vector2" /> structures.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        public Segment2(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Segment2" /> structure.
        /// </summary>
        /// <param name="x1">The starting x-coordinate.</param>
        /// <param name="y1">The starting y-coordinate.</param>
        /// <param name="x2">The ending x-coordinate.</param>
        /// <param name="y2">The ending y-coordinate.</param>
        public Segment2(float x1, float y1, float x2, float y2)
            : this(new Vector2(x1, y1), new Vector2(x2, y2))
        {
        }

        // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.1.2; Basic Primitive Tests - Closest Point on Line Segment to Point. pg 127-130
        /// <summary>
        ///     Computes the closest <see cref="Vector2" /> on this <see cref="Segment2" /> to a specified <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The closest <see cref="Vector2" /> on this <see cref="Segment2" /> to the <paramref name="point" />.</returns>
        public Vector2 ClosestPointTo(Vector2 point)
        {
            // Computes the parameterized position: d(t) = Start + t * (End – Start)

            var startToEnd = End - Start;
            var startToPoint = point - Start;
            // Project arbitrary point onto the line segment, deferring the division
            var t = startToEnd.Dot(startToPoint);
            // If outside segment, clamp t (and therefore d) to the closest endpoint
            if (t <= 0)
                return Start;

            // Always nonnegative since denom = (||vector||)^2
            var denominator = startToEnd.Dot(startToEnd);
            if (t >= denominator)
                return End;

            // The point projects inside the [Start, End] interval, must do deferred division now
            t /= denominator;
            startToEnd *= t;
            return new Vector2(Start.X + startToEnd.X, Start.Y + startToEnd.Y);
        }

        // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.1.2.1; Basic Primitive Tests - Distance of Point to Segment. pg 127-130
        /// <summary>
        ///     Computes the squared distance from this <see cref="Segment2" /> to a specified <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The squared distance from this <see cref="Segment2" /> to a specified <see cref="Vector2" />.</returns>
        public float SquaredDistanceTo(Vector2 point)
        {
            var startToEnd = End - Start;
            var startToPoint = point - Start;
            var endToPoint = point - End;
            // Handle cases where the point projects outside the line segment
            var dot = startToPoint.Dot(startToEnd);
            var startToPointDistanceSquared = startToPoint.Dot(startToPoint);
            if (dot <= 0.0f)
                return startToPointDistanceSquared;
            var startToEndDistanceSquared = startToEnd.Dot(startToEnd);
            if (dot >= startToEndDistanceSquared)
                endToPoint.Dot(endToPoint);
            // Handle the case where the point projects onto the line segment
            return startToPointDistanceSquared - dot*dot/startToEndDistanceSquared;
        }

        /// <summary>
        ///     Computes the distance from this <see cref="Segment2" /> to a specified <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance from this <see cref="Segment2" /> to a specified <see cref="Vector2" />.</returns>
        public float DistanceTo(Vector2 point)
        {
            return (float) Math.Sqrt(SquaredDistanceTo(point));
        }

        /// <summary>
        ///     Determines whether this <see cref="Segment2" /> intersects with the specified <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="rectangle">The bounding box.</param>
        /// <param name="intersectionPoint">
        ///     When this method returns, contains the <see cref="Vector2" /> of intersection, if an
        ///     intersection was found; otherwise, the <see cref="Vector2.NaN" />. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Segment2" /> intersects with <paramref name="rectangle" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(RectangleF rectangle, out Vector2 intersectionPoint)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.3; Basic Primitive Tests - Intersecting Lines, Rays, and (Directed Segments). pg 179-181

            var minimumPoint = rectangle.TopLeft;
            var maximumPoint = rectangle.BottomRight;
            var minimumDistance = float.MinValue;
            var maximumDistance = float.MaxValue;

            var direction = End - Start;
            if (
                !PrimitivesHelper.IntersectsSlab(Start.X, direction.X, minimumPoint.X, maximumPoint.X, ref minimumDistance,
                    ref maximumDistance))
            {
                intersectionPoint = new Vector2(float.NaN);
                return false;
            }

            if (
                !PrimitivesHelper.IntersectsSlab(Start.Y, direction.Y, minimumPoint.Y, maximumPoint.Y, ref minimumDistance,
                    ref maximumDistance))
            {
                intersectionPoint = new Vector2(float.NaN);
                return false;
            }

            // Segment intersects the 2 slabs.

            if (minimumDistance <= 0)
                intersectionPoint = Start;
            else
            {
                intersectionPoint = minimumDistance * direction;
                intersectionPoint.X += Start.X;
                intersectionPoint.Y += Start.Y;
            }

            return true;
        }

        /// <summary>
        ///     Determines whether this <see cref="Segment2" /> intersects with the specified <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding box.</param>
        /// <param name="intersectionPoint">
        ///     When this method returns, contains the <see cref="Vector2" /> of intersection, if an
        ///     intersection was found; otherwise, the <see cref="Vector2.NaN" />. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Segment2" /> intersects with <paramref name="boundingRectangle" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(BoundingRectangle boundingRectangle, out Vector2 intersectionPoint)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.3; Basic Primitive Tests - Intersecting Lines, Rays, and (Directed Segments). pg 179-181

            var minimumPoint = boundingRectangle.Center - boundingRectangle.HalfExtents;
            var maximumPoint = boundingRectangle.Center + boundingRectangle.HalfExtents;
            var minimumDistance = float.MinValue;
            var maximumDistance = float.MaxValue;

            var direction = End - Start;
            if (
                !PrimitivesHelper.IntersectsSlab(Start.X, direction.X, minimumPoint.X, maximumPoint.X, ref minimumDistance,
                    ref maximumDistance))
            {
                intersectionPoint = new Vector2(float.NaN);
                return false;
            }

            if (
                !PrimitivesHelper.IntersectsSlab(Start.Y, direction.Y, minimumPoint.Y, maximumPoint.Y, ref minimumDistance,
                    ref maximumDistance))
            {
                intersectionPoint = new Vector2(float.NaN);
                return false;
            }

            // Segment intersects the 2 slabs.

            if (minimumDistance <= 0)
                intersectionPoint = Start;
            else
            {
                intersectionPoint = minimumDistance*direction;
                intersectionPoint.X += Start.X;
                intersectionPoint.Y += Start.Y;
            }

            return true;
        }

        /// <summary>
        ///     Compares two <see cref="Segment2" /> structures. The result specifies
        ///     whether the values of the <see cref="Start" /> and <see cref="End" />
        ///     fields of the two <see cref='Segment2' />
        ///     structures are equal.
        /// </summary>
        /// <param name="first">The first segment.</param>
        /// <param name="second">The second segment.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Start" /> and <see cref="End" />
        ///     fields of the two <see cref="Segment2" />
        ///     structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Segment2 first, Segment2 second)
        {
            return first.Equals(ref second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="Segment2" /> is equal to another <see cref="Segment2" />.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Segment2" /> is equal to the <paramref name="segment" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Segment2 segment)
        {
            return Equals(ref segment);
        }

        /// <summary>
        ///     Indicates whether this <see cref="Segment2" /> is equal to another <see cref="Segment2" />.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Segment2" /> is equal to the <paramref name="segment" /> parameter; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(ref Segment2 segment)
        {
            return (Start == segment.Start) && (End == segment.End);
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="Segment2" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="Segment2" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Segment2)
                return Equals((Segment2) obj);
            return false;
        }

        /// <summary>
        ///     Compares two <see cref="Segment2" /> structures. The result specifies
        ///     whether the values of the <see cref="Start" /> and <see cref="End" />
        ///     fields of the two <see cref="Segment2" />
        ///     structures are unequal.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Start" /> and <see cref="End" />
        ///     fields of the two <see cref="Segment2" />
        ///     structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Segment2 first, Segment2 second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="Segment2" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="Segment2" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode()*397) ^ End.GetHashCode();
            }
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="Segment2" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="Segment2" />.
        /// </returns>
        public override string ToString()
        {
            return $"{Start} -> {End}";
        }

        internal string DebugDisplayString => ToString();
    }
}
