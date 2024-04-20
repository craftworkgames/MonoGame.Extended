using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <remarks>Based on the RectangleF class, modified for triangles.</remarks>
    [DataContract]
    public struct TriangleF : IEquatable<TriangleF>, IEquatableByRef<TriangleF>, IShapeF
    {
        /// <summary>
        ///     The <see cref="TriangleF" /> with <see cref="A" />, <see cref="B" />, and
        ///     <see cref="C" /> all set to <code>Point2.Zero</code>.
        /// </summary>
        public static readonly TriangleF Empty = new TriangleF();

        /// <summary>
        ///     The coordinates of the first point in this <see cref="TriangleF" />.
        /// </summary>
        [DataMember] public Point2 A;

        /// <summary>
        ///     The coordinates of the second point in this <see cref="TriangleF" />.
        /// </summary>
        [DataMember] public Point2 B;

        /// <summary>
        ///     The coordinates of the third point in this <see cref="TriangleF" />.
        /// </summary>
        [DataMember] public Point2 C;

        /// <summary>
        ///     Gets a value indicating whether this <see cref="TriangleF" /> has a <see cref="A" />, <see cref="B" />,
        ///     and <see cref="C" /> all equal to <code>Point2.Zero</code>.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => (A == Point2.Zero) && (B == Point2.Zero) && (C == Point2.Zero);

        /// <summary>
        ///     Gets the <see cref="Point2" /> representing the top-left of this <see cref="TriangleF" />.
        /// </summary>
        public Point2 Position
        {
            get => Center;
            set
            {
                Point2 center = Center;
                A = value + (A - center);
                B = value + (B - center);
                C = value + (C - center);
            }
        }

        [Pure]
        public TriangleF Rotate(float angle)
        {
            Point2 center = Center;
            Vector2 offsetA = A - center;
            Vector2 offsetB = B - center;
            Vector2 offsetC = C - center;

            return new TriangleF
            {
                A = offsetA.Rotate(angle) + center,
                B = offsetB.Rotate(angle) + center,
                C = offsetC.Rotate(angle) + center,
            };
        }

        [Pure]
        public TriangleF Reflect(Vector2 angle) => Reflect(angle.ToAngle());

        [Pure]
        public TriangleF Reflect(float angle)
        {
            Point2 center = Center;
            TriangleF rotation = Rotate(-angle);
            TriangleF conj = rotation.Conjugate;
            TriangleF result = conj.Rotate(angle);
            result.Position = center;
            return result;
        }

        [Pure]
        private TriangleF Conjugate => new(new(-A.X, A.Y), new(-B.X, B.Y), new(-C.X, C.Y));

        [Pure]
        public Vector2[] GetPoints() => new Vector2[]{A,B,C};

        [Pure]
        public Vector2[] GetRelativePoints()
        {
            Point2 center = Center;
            return new[] { A - center, B - center, C - center };
        }

        public RectangleF BoundingRectangle
        {
            get
            {
                float x = MathF.Min(MathF.Min(A.X, B.X), C.X);
                float y = MathF.Min(MathF.Min(A.Y, B.Y), C.Y);
                float width = MathF.Max(MathF.Max(A.X, B.X), C.X) - x;
                float height = MathF.Max(MathF.Max(A.Y, B.Y), C.Y) - y;
                return new RectangleF(x, y, width, height);
            }
        }

        /// <summary>
        ///     Gets the <see cref="float" /> representing the area of this <see cref="TriangleF" />, expressed in units².
        /// </summary>
        public float Area => GetArea(A, B, C);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetArea(Point2 a, Point2 b, Point2 c)
            => MathF.Abs((a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y)) / 2f);

        /// <summary>
        ///     Gets the <see cref="Point2" /> representing the center of this <see cref="TriangleF" />.
        /// </summary>
        public Point2 Center => new((A.X + B.X + C.X) / 3f, (A.Y + B.Y + C.Y) / 3f);


        /// <summary>
        ///     Initializes a new instance of the <see cref="TriangleF" /> structure from the specified top-left xy-coordinate
        ///     <see cref="float" />s, width <see cref="float" /> and height <see cref="float" />.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        public TriangleF(Point2 a, Point2 b, Point2 c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TriangleF" /> structure from the specified source
        ///     <see cref="TriangleF" />, scaled by the scale <see cref="float" />.
        /// </summary>
        /// <param name="source">The original triangle.</param>
        /// <param name="scale">The scale factor.</param>
        public TriangleF(TriangleF source, float scale) : this(source.A, source.B, source.C)
        {
            var center = Center;
            A = center + ((A - center) * scale);
            B = center + ((B - center) * scale);
            C = center + ((C - center) * scale);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TriangleF" /> structure from the specified source
        ///     <see cref="TriangleF" />, scaled by the scale <see cref="float" />.
        /// </summary>
        /// <param name="source">The original triangle.</param>
        /// <param name="scale">The scale factor.</param>
        public TriangleF(TriangleF source, Vector2 scale) : this(source.A, source.B, source.C)
        {
            var center = Center;
            A = center + ((A - center) * scale);
            B = center + ((B - center) * scale);
            C = center + ((C - center) * scale);
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="TriangleF" /> structures intersect.
        /// </summary>
        /// <param name="first">The first triangle.</param>
        /// <param name="second">The second triangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(ref TriangleF first, ref TriangleF second)
        {
            // Check if any of the edges of the first triangle intersect with any of the edges of the second triangle
            return EdgeIntersects(first.A, first.B, second) ||
                   EdgeIntersects(first.B, first.C, second) ||
                   EdgeIntersects(first.C, first.A, second) ||
                   EdgeIntersects(second.A, second.B, first) ||
                   EdgeIntersects(second.B, second.C, first) ||
                   EdgeIntersects(second.C, second.A, first);
        }

        private static bool EdgeIntersects(Point2 p1, Point2 p2, TriangleF triangle)
        {
            float minX = Math.Min(triangle.A.X, Math.Min(triangle.B.X, triangle.C.X));
            float maxX = Math.Max(triangle.A.X, Math.Max(triangle.B.X, triangle.C.X));
            float minY = Math.Min(triangle.A.Y, Math.Min(triangle.B.Y, triangle.C.Y));
            float maxY = Math.Max(triangle.A.Y, Math.Max(triangle.B.Y, triangle.C.Y));

            float slope = (p2.Y - p1.Y) / (p2.X - p1.X);
            float yIntercept = p1.Y - slope * p1.X;

            float yMin = slope * minX + yIntercept;
            float yMax = slope * maxX + yIntercept;

            // check if edge intersects y-range of triangle
            if ((yMin >= minY && yMin <= maxY) || (yMax >= minY && yMax <= maxY))
            {
                // check if intersection.x is in x-range of triangle
                float xMin = Math.Min(triangle.A.X, Math.Min(triangle.B.X, triangle.C.X));
                float xMax = Math.Max(triangle.A.X, Math.Max(triangle.B.X, triangle.C.X));

                float xIntersection = (yMin - yIntercept) / slope;
                if (xIntersection >= xMin && xIntersection <= xMax)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="TriangleF" /> structures intersect.
        /// </summary>
        /// <param name="first">The first triangle.</param>
        /// <param name="second">The second triangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(TriangleF first, TriangleF second)
        {
            return Intersects(ref first, ref second);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="TriangleF" /> intersects with this
        ///     <see cref="TriangleF" />.
        /// </summary>
        /// <param name="triangle">The bounding triangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="triangle" /> intersects with this
        ///     <see cref="TriangleF" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(TriangleF triangle)
        {
            return Intersects(ref this, ref triangle);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="TriangleF" /> contains the specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="triangle">The triangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="triangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(ref TriangleF triangle, ref Point2 point)
        {
            const float EPSILON = 0.001f;

            float areaABC = GetArea(triangle.A, triangle.B, triangle.C);
            float areaPBC = GetArea(point, triangle.B, triangle.C);
            float areaAPC = GetArea(triangle.A, point, triangle.C);
            float areaABP = GetArea(triangle.A, triangle.B, point);

            // If the sum of the areas of the sub-triangles equals the area of the main triangle,
            // then the point is inside the triangle.
            return MathF.Abs(areaABC - (areaPBC + areaAPC + areaABP)) < EPSILON;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="TriangleF" /> contains the specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="triangle">The triangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="triangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(TriangleF triangle, Point2 point)
        {
            return Contains(ref triangle, ref point);
        }

        /// <summary>
        ///     Determines whether this <see cref="TriangleF" /> contains the specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="TriangleF"/> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Contains(Point2 point)
        {
            return Contains(ref this, ref point);
        }

        /// <summary>
        ///     Computes the closest <see cref="Point2" /> on this <see cref="TriangleF" /> to a specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The closest <see cref="Point2" /> on this <see cref="TriangleF" /> to the <paramref name="point" />.</returns>
        public Point2 ClosestPointTo(Point2 point)
        {
            float denominator = ((B.Y - C.Y) * (A.X - C.X) + (C.X - B.X) * (A.Y - C.Y));
            if (Math.Abs(denominator) < float.Epsilon) return A;

            float w1 = ((B.Y - C.Y) * (point.X - C.X) + (C.X - B.X) * (point.Y - C.Y)) / denominator;
            float w2 = ((C.Y - A.Y) * (point.X - C.X) + (A.X - C.X) * (point.Y - C.Y)) / denominator;
            float w3 = 1 - w1 - w2;

            w1 = Math.Clamp(w1, 0, 1);
            w2 = Math.Clamp(w2, 0, 1);
            w3 = Math.Clamp(w3, 0, 1);

            float closestX = w1 * A.X + w2 * B.X + w3 * C.X;
            float closestY = w1 * A.Y + w2 * B.Y + w3 * C.Y;

            return new Point2(closestX, closestY);
        }

        //TODO: Document this.
        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            Vector2 scale = new(horizontalAmount, verticalAmount);
            var center = Center;
            A = center + ((A - center) * scale);
            B = center + ((B - center) * scale);
            C = center + ((C - center) * scale);
        }

        //TODO: Document this.
        public void Offset(float offsetX, float offsetY) => Position += new Vector2(offsetX, offsetY);

        //TODO: Document this.
        public void Offset(Vector2 amount) => Position += amount;

        /// <summary>
        ///     Compares two <see cref="TriangleF" /> structures. The result specifies whether the values of the
        ///     <see cref="A" />, <see cref="B"/>, and <see cref="C" /> fields of the two <see cref="TriangleF" /> structures
        ///     are equal.
        /// </summary>
        /// <param name="first">The first triangle.</param>
        /// <param name="second">The second triangle.</param>
        /// <returns>
        ///     <c>true</c> if the values of the
        ///     <see cref="A" />, <see cref="B"/>, and <see cref="C" /> fields of the two <see cref="TriangleF" /> structures
        ///     are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(TriangleF first, TriangleF second) => first.Equals(ref second);

        /// <summary>
        ///     Compares two <see cref="TriangleF" /> structures. The result specifies whether the values of the
        ///     <see cref="A" />, <see cref="B"/>, and <see cref="C" /> fields of the two <see cref="TriangleF" /> structures
        ///     are unequal.
        /// </summary>
        /// <param name="first">The first triangle.</param>
        /// <param name="second">The second triangle.</param>
        /// <returns>
        ///     <c>true</c> if the values of the
        ///     <see cref="A" />, <see cref="B"/>, and <see cref="C" /> fields of the two <see cref="TriangleF" /> structures
        ///     are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(TriangleF first, TriangleF second) => !(first == second);

        /// <summary>
        ///     Indicates whether this <see cref="TriangleF" /> is equal to another <see cref="TriangleF" />.
        /// </summary>
        /// <param name="triangle">The triangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="TriangleF" /> is equal to the <paramref name="triangle" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(TriangleF triangle) => Equals(ref triangle);

        /// <summary>
        ///     Indicates whether this <see cref="TriangleF" /> is equal to another <see cref="TriangleF" />.
        /// </summary>
        /// <param name="triangle">The triangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="TriangleF" /> is equal to the <paramref name="triangle" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref TriangleF triangle)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (A == triangle.A) || (B == triangle.B) || (C == triangle.C);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="TriangleF" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="TriangleF" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is TriangleF tf && Equals(tf);

        /// <summary>
        ///     Returns a hash code of this <see cref="TriangleF" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="TriangleF" />.
        /// </returns>
        public override int GetHashCode() => HashCode.Combine(A, B, C);

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="TriangleF" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="TriangleF" />.
        /// </returns>
        public override string ToString()
        {
            return $"{{A: ({A.X}, {A.Y}), B: ({B.X}, {B.Y}), C: ({C.X}, {C.Y})";
        }
    }
}
