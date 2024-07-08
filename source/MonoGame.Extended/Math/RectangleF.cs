using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 77

    /// <summary>
    ///     An axis-aligned, four sided, two dimensional box defined by a top-left position (<see cref="X" /> and
    ///     <see cref="Y" />) and a size (<see cref="Width" /> and <see cref="Height" />).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         An <see cref="RectangleF" /> is categorized by having its faces oriented in such a way that its
    ///         face normals are at all times parallel with the axes of the given coordinate system.
    ///     </para>
    ///     <para>
    ///         The bounding <see cref="RectangleF" /> of a rotated <see cref="RectangleF" /> will be equivalent or larger
    ///         in size than the original depending on the angle of rotation.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IEquatableByRef{T}" />
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct RectangleF : IEquatable<RectangleF>, IEquatableByRef<RectangleF>, IShapeF
    {
        /// <summary>
        ///     The <see cref="RectangleF" /> with <see cref="X" />, <see cref="Y" />, <see cref="Width" /> and
        ///     <see cref="Height" /> all set to <code>0.0f</code>.
        /// </summary>
        public static readonly RectangleF Empty = new RectangleF();

        /// <summary>
        ///     The x-coordinate of the top-left corner position of this <see cref="RectangleF" />.
        /// </summary>
        [DataMember] public float X;

        /// <summary>
        ///     The y-coordinate of the top-left corner position of this <see cref="RectangleF" />.
        /// </summary>
        [DataMember] public float Y;

        /// <summary>
        ///     The width of this <see cref="RectangleF" />.
        /// </summary>
        [DataMember] public float Width;

        /// <summary>
        ///     The height of this <see cref="RectangleF" />.
        /// </summary>
        [DataMember] public float Height;

        /// <summary>
        ///     Gets the x-coordinate of the left edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Left => X;

        /// <summary>
        ///     Gets the x-coordinate of the right edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Right => X + Width;

        /// <summary>
        ///     Gets the y-coordinate of the top edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Top => Y;

        /// <summary>
        ///     Gets the y-coordinate of the bottom edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Bottom => Y + Height;

        /// <summary>
        ///     Gets a value indicating whether this <see cref="RectangleF" /> has a <see cref="X" />, <see cref="Y" />,
        ///     <see cref="Width" />,
        ///     <see cref="Height" /> all equal to <code>0.0f</code>.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => Width.Equals(0) && Height.Equals(0) && X.Equals(0) && Y.Equals(0);

        /// <summary>
        ///     Gets the <see cref="Vector2" /> representing the the top-left of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 Position
        {
            get { return new Vector2(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public RectangleF BoundingRectangle => this;

        /// <summary>
        ///     Gets the <see cref="SizeF" /> representing the extents of this <see cref="RectangleF" />.
        /// </summary>
        public SizeF Size
        {
            get { return new SizeF(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        ///     Gets the <see cref="Vector2" /> representing the center of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 Center => new Vector2(X + Width * 0.5f, Y + Height * 0.5f);

        /// <summary>
        ///     Gets the <see cref="Vector2" /> representing the top-left of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 TopLeft => new Vector2(X, Y);

        /// <summary>
        ///     Gets the <see cref="Vector2" /> representing the top-right of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 TopRight => new Vector2(X + Width, Y);

        /// <summary>
        ///     Gets the <see cref="Vector2" /> representing the bottom-left of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 BottomLeft => new Vector2(X, Y + Height);

        /// <summary>
        ///     Gets the <see cref="Vector2" /> representing the bottom-right of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 BottomRight => new Vector2(X + Width, Y + Height);

        /// <summary>
        ///     Initializes a new instance of the <see cref="RectangleF" /> structure from the specified top-left xy-coordinate
        ///     <see cref="float" />s, width <see cref="float" /> and height <see cref="float" />.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RectangleF" /> structure from the specified top-left
        ///     <see cref="Vector2" /> and the extents <see cref="SizeF" />.
        /// </summary>
        /// <param name="position">The top-left point.</param>
        /// <param name="size">The extents.</param>
        public RectangleF(Vector2 position, SizeF size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from a minimum <see cref="Vector2" /> and maximum
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="minimum">The minimum point.</param>
        /// <param name="maximum">The maximum point.</param>
        /// <param name="result">The resulting rectangle.</param>
        public static void CreateFrom(Vector2 minimum, Vector2 maximum, out RectangleF result)
        {
            result.X = minimum.X;
            result.Y = minimum.Y;
            result.Width = maximum.X - minimum.X;
            result.Height = maximum.Y - minimum.Y;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from a minimum <see cref="Vector2" /> and maximum
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="minimum">The minimum point.</param>
        /// <param name="maximum">The maximum point.</param>
        /// <returns>The resulting <see cref="RectangleF" />.</returns>
        public static RectangleF CreateFrom(Vector2 minimum, Vector2 maximum)
        {
            RectangleF result;
            CreateFrom(minimum, maximum, out result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from a list of <see cref="Vector2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="result">The resulting rectangle.</param>
        public static void CreateFrom(IReadOnlyList<Vector2> points, out RectangleF result)
        {
            Vector2 minimum;
            Vector2 maximum;
            PrimitivesHelper.CreateRectangleFromPoints(points, out minimum, out maximum);
            CreateFrom(minimum, maximum, out result);
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from a list of <see cref="Vector2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>The resulting <see cref="RectangleF" />.</returns>
        public static RectangleF CreateFrom(IReadOnlyList<Vector2> points)
        {
            RectangleF result;
            CreateFrom(points, out result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from the specified <see cref="RectangleF" /> transformed by
        ///     the specified <see cref="Matrix3x2" />.
        /// </summary>
        /// <param name="rectangle">The rectangle to be transformed.</param>
        /// <param name="transformMatrix">The transform matrix.</param>
        /// <param name="result">The resulting transformed rectangle.</param>
        /// <returns>
        ///     The <see cref="Extended.BoundingRectangle" /> from the <paramref name="rectangle" /> transformed by the
        ///     <paramref name="transformMatrix" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If a transformed <see cref="Extended.BoundingRectangle" /> is used for <paramref name="rectangle" /> then the
        ///         resulting <see cref="Extended.BoundingRectangle" /> will have the compounded transformation, which most likely is
        ///         not desired.
        ///     </para>
        /// </remarks>
        public static void Transform(ref RectangleF rectangle,
            ref Matrix3x2 transformMatrix, out RectangleF result)
        {
            var center = rectangle.Center;
            var halfExtents = (Vector2)rectangle.Size * 0.5f;

            PrimitivesHelper.TransformRectangle(ref center, ref halfExtents, ref transformMatrix);

            result.X = center.X - halfExtents.X;
            result.Y = center.Y - halfExtents.Y;
            result.Width = halfExtents.X * 2;
            result.Height = halfExtents.Y * 2;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from the specified <see cref="Extended.BoundingRectangle" /> transformed by
        ///     the
        ///     specified <see cref="Matrix3x2" />.
        /// </summary>
        /// <param name="rectangle">The bounding rectangle.</param>
        /// <param name="transformMatrix">The transform matrix.</param>
        /// <returns>
        ///     The <see cref="Extended.BoundingRectangle" /> from the <paramref name="rectangle" /> transformed by the
        ///     <paramref name="transformMatrix" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If a transformed <see cref="Extended.BoundingRectangle" /> is used for <paramref name="rectangle" /> then the
        ///         resulting <see cref="Extended.BoundingRectangle" /> will have the compounded transformation, which most likely is
        ///         not desired.
        ///     </para>
        /// </remarks>
        public static RectangleF Transform(RectangleF rectangle, ref Matrix3x2 transformMatrix)
        {
            RectangleF result;
            Transform(ref rectangle, ref transformMatrix, out result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that contains the two specified
        ///     <see cref="RectangleF" /> structures.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <param name="result">The resulting rectangle that contains both the <paramref name="first" /> and the
        ///     <paramref name="second" />.</param>
        public static void Union(ref RectangleF first, ref RectangleF second, out RectangleF result)
        {
            result.X = Math.Min(first.X, second.X);
            result.Y = Math.Min(first.Y, second.Y);
            result.Width = Math.Max(first.Right, second.Right) - result.X;
            result.Height = Math.Max(first.Bottom, second.Bottom) - result.Y;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that contains the two specified
        ///     <see cref="RectangleF" /> structures.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     An <see cref="RectangleF" /> that contains both the <paramref name="first" /> and the
        ///     <paramref name="second" />.
        /// </returns>
        public static RectangleF Union(RectangleF first, RectangleF second)
        {
            RectangleF result;
            Union(ref first, ref second, out result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that contains both the specified <see cref="RectangleF" /> and this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     An <see cref="RectangleF" /> that contains both the <paramref name="rectangle" /> and
        ///     this <see cref="RectangleF" />.
        /// </returns>
        public RectangleF Union(RectangleF rectangle)
        {
            RectangleF result;
            Union(ref this, ref rectangle, out result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that is in common between the two specified
        ///     <see cref="RectangleF" /> structures.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <param name="result">The resulting rectangle that is in common between both the <paramref name="first" /> and
        ///     the <paramref name="second" />, if they intersect; otherwise, <see cref="Empty"/>.</param>
        public static void Intersection(ref RectangleF first,
            ref RectangleF second, out RectangleF result)
        {
            var firstMinimum = first.TopLeft;
            var firstMaximum = first.BottomRight;
            var secondMinimum = second.TopLeft;
            var secondMaximum = second.BottomRight;

            var minimum = MathExtended.CalculateMaximumVector2(firstMinimum, secondMinimum);
            var maximum = MathExtended.CalculateMinimumVector2(firstMaximum, secondMaximum);

            if ((maximum.X < minimum.X) || (maximum.Y < minimum.Y))
                result = new RectangleF();
            else
                result = CreateFrom(minimum, maximum);
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that is in common between the two specified
        ///     <see cref="RectangleF" /> structures.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     A <see cref="RectangleF" /> that is in common between both the <paramref name="first" /> and
        ///     the <paramref name="second" />, if they intersect; otherwise, <see cref="Empty"/>.
        /// </returns>
        public static RectangleF Intersection(RectangleF first,
            RectangleF second)
        {
            RectangleF result;
            Intersection(ref first, ref second, out result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that is in common between the specified
        ///     <see cref="RectangleF" /> and this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     A <see cref="RectangleF" /> that is in common between both the <paramref name="rectangle" /> and
        ///     this <see cref="RectangleF"/>, if they intersect; otherwise, <see cref="Empty"/>.
        /// </returns>
        public RectangleF Intersection(RectangleF rectangle)
        {
            RectangleF result;
            Intersection(ref this, ref rectangle, out result);
            return result;
        }

        [Obsolete("RectangleF.Intersect() may be removed in the future. Use Intersection() instead.")]
        public static RectangleF Intersect(RectangleF value1, RectangleF value2)
        {
            RectangleF rectangle;
            Intersection(ref value1, ref value2, out rectangle);
            return rectangle;
        }

        [Obsolete("RectangleF.Intersect() may be removed in the future. Use Intersection() instead.")]
        public static void Intersect(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            Intersection(ref value1, ref value2, out result);
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="RectangleF" /> structures intersect.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(ref RectangleF first, ref RectangleF second)
        {
            return first.X < second.X + second.Width && first.X + first.Width > second.X &&
                   first.Y < second.Y + second.Height && first.Y + first.Height > second.Y;
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="RectangleF" /> structures intersect.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(RectangleF first, RectangleF second)
        {
            return Intersects(ref first, ref second);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="RectangleF" /> intersects with this
        ///     <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="rectangle" /> intersects with this
        ///     <see cref="RectangleF" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(RectangleF rectangle)
        {
            return Intersects(ref this, ref rectangle);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="RectangleF" /> contains the specified
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="rectangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(ref RectangleF rectangle, ref Vector2 point)
        {
            return rectangle.X <= point.X && point.X < rectangle.X + rectangle.Width && rectangle.Y <= point.Y && point.Y < rectangle.Y + rectangle.Height;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="RectangleF" /> contains the specified
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="rectangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(RectangleF rectangle, Vector2 point)
        {
            return Contains(ref rectangle, ref point);
        }

        /// <summary>
        ///     Determines whether this <see cref="RectangleF" /> contains the specified
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the this <see cref="RectangleF"/> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Contains(Vector2 point)
        {
            return Contains(ref this, ref point);
        }

        /// <summary>
        ///     Updates this <see cref="RectangleF" /> from a list of <see cref="Vector2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        public void UpdateFromPoints(IReadOnlyList<Vector2> points)
        {
            var rectangle = CreateFrom(points);
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        /// <summary>
        ///     Computes the squared distance from this <see cref="RectangleF"/> to a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The squared distance from this <see cref="RectangleF"/> to the <paramref name="point"/>.</returns>
        public float SquaredDistanceTo(Vector2 point)
        {
            return PrimitivesHelper.SquaredDistanceToPointFromRectangle(TopLeft, BottomRight, point);
        }

        /// <summary>
        ///     Computes the distance from this <see cref="RectangleF"/> to a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance from this <see cref="RectangleF"/> to the <paramref name="point"/>.</returns>
        public float DistanceTo(Vector2 point)
        {
            return (float)Math.Sqrt(SquaredDistanceTo(point));
        }

        /// <summary>
        ///     Computes the closest <see cref="Vector2" /> on this <see cref="RectangleF" /> to a specified
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The closest <see cref="Vector2" /> on this <see cref="RectangleF" /> to the <paramref name="point" />.</returns>
        public Vector2 ClosestPointTo(Vector2 point)
        {
            Vector2 result;
            PrimitivesHelper.ClosestPointToPointFromRectangle(TopLeft, BottomRight, point, out result);
            return result;
        }

        //TODO: Document this.
        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2;
            Height += verticalAmount * 2;
        }

        //TODO: Document this.
        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        //TODO: Document this.
        public void Offset(Vector2 amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        /// <summary>
        ///     Compares two <see cref="RectangleF" /> structures. The result specifies whether the values of the
        ///     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        ///     are equal.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the values of the
        ///     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        ///     are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(RectangleF first, RectangleF second)
        {
            return first.Equals(ref second);
        }

        /// <summary>
        ///     Compares two <see cref="RectangleF" /> structures. The result specifies whether the values of the
        ///     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        ///     are unequal.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the values of the
        ///     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        ///     are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(RectangleF first, RectangleF second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="RectangleF" /> is equal to another <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="RectangleF" /> is equal to the <paramref name="rectangle" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(RectangleF rectangle)
        {
            return Equals(ref rectangle);
        }

        /// <summary>
        ///     Indicates whether this <see cref="RectangleF" /> is equal to another <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="RectangleF" /> is equal to the <paramref name="rectangle" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref RectangleF rectangle)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return X == rectangle.X && Y == rectangle.Y && Width == rectangle.Width && Height == rectangle.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="RectangleF" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="RectangleF" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is RectangleF && Equals((RectangleF)obj);
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="RectangleF" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="RectangleF" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Rectangle" /> to a <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="RectangleF" />.
        /// </returns>
        public static implicit operator RectangleF(Rectangle rectangle)
        {
            return new RectangleF
            {
                X = rectangle.X,
                Y = rectangle.Y,
                Width = rectangle.Width,
                Height = rectangle.Height
            };
        }

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="Rectangle" /> to a <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="RectangleF" />.
        /// </returns>
        /// <remarks>
        ///     <para>A loss of precision may occur due to the truncation from <see cref="float" /> to <see cref="int" />.</para>
        /// </remarks>
        public static explicit operator Rectangle(RectangleF rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="RectangleF" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="RectangleF" />.
        /// </returns>
        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
        }

        internal string DebugDisplayString => string.Concat(X, "  ", Y, "  ", Width, "  ", Height);
    }
}
