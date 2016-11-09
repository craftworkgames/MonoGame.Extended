using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    /// <summary>
    ///     Describes a floating point 2D-rectangle.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct RectangleF : IShapeF, IEquatable<RectangleF>
    {
        /// <summary>
        ///     The x coordinate of the top-left corner of this <see cref="RectangleF" />.
        /// </summary>
        [DataMember] public float X;

        /// <summary>
        ///     The y coordinate of the top-left corner of this <see cref="RectangleF" />.
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
        ///     Returns a <see cref="RectangleF" /> with X=0, Y=0, Width=0, Height=0.
        /// </summary>
        public static RectangleF Empty { get; } = new RectangleF();

        /// <summary>
        ///     Returns the x coordinate of the left edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Left => X;

        /// <summary>
        ///     Returns the x coordinate of the right edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Right => X + Width;

        /// <summary>
        ///     Returns the y coordinate of the top edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Top => Y;

        /// <summary>
        ///     Returns the y coordinate of the bottom edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Bottom => Y + Height;

        /// <summary>
        ///     Whether or not this <see cref="RectangleF" /> has a <see cref="Width" /> and
        ///     <see cref="Height" /> of 0, and a <see cref="Location" /> of (0, 0).
        /// </summary>
        public bool IsEmpty => Width.Equals(0) && Height.Equals(0) && X.Equals(0) && Y.Equals(0);

        /// <summary>
        ///     The top-left coordinates of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 Location
        {
            get { return new Vector2(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        ///     The width-height coordinates of this <see cref="RectangleF" />.
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
        ///     A <see cref="Vector2" /> located in the center of this <see cref="RectangleF" />.
        /// </summary>
        public Vector2 Center => new Vector2(X + Width/2f, Y + Height/2f);

        internal string DebugDisplayString => string.Concat(X, "  ", Y, "  ", Width, "  ", Height);

        /// <summary>
        ///     Creates a new instance of <see cref="RectangleF" /> struct, with the specified
        ///     position, width, and height.
        /// </summary>
        /// <param name="x">The x coordinate of the top-left corner of the created <see cref="RectangleF" />.</param>
        /// <param name="y">The y coordinate of the top-left corner of the created <see cref="RectangleF" />.</param>
        /// <param name="width">The width of the created <see cref="RectangleF" />.</param>
        /// <param name="height">The height of the created <see cref="RectangleF" />.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="RectangleF" /> struct, with the specified
        ///     location and size.
        /// </summary>
        /// <param name="location">The x and y coordinates of the top-left corner of the created <see cref="RectangleF" />.</param>
        /// <param name="size">The width and height of the created <see cref="RectangleF" />.</param>
        public RectangleF(Vector2 location, SizeF size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="RectangleF" /> struct, based on a <see cref="Rectangle" />
        /// </summary>
        /// <param name="rect">The source <see cref="Rectangle" />.</param>
        public RectangleF(Rectangle rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        /// <summary>
        ///     Allow implict cast from a <see cref="Rectangle" />
        /// </summary>
        /// <param name="rect">The <see cref="Rectangle" /> to be cast.</param>
        public static implicit operator RectangleF(Rectangle rect)
        {
            return new RectangleF(rect);
        }

        /// <summary>
        ///     Allow implict cast from a <see cref="Rectangle" />
        /// </summary>
        /// <param name="rect">The <see cref="Rectangle" /> to be cast.</param>
        public static implicit operator RectangleF?(Rectangle? rect)
        {
            if (!rect.HasValue)
                return null;

            return new RectangleF(rect.Value);
        }

        /// <summary>
        ///     Allow explict cast to a <see cref="Rectangle" />
        /// </summary>
        /// <remark>
        ///     Loss of precision due to the truncation from <see cref="float" /> to <see cref="int" />.
        /// </remark>
        /// <param name="rect">The <see cref="RectangleF" /> to be cast.</param>
        public static explicit operator Rectangle(RectangleF rect)
        {
            return new Rectangle((int) rect.X, (int) rect.Y, (int) rect.Width, (int) rect.Height);
        }

        /// <summary>
        ///     Compares whether two <see cref="RectangleF" /> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="RectangleF" /> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="RectangleF" /> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(RectangleF a, RectangleF b)
        {
            const float epsilon = 0.00001f;
            return (Math.Abs(a.X - b.X) < epsilon)
                   && (Math.Abs(a.Y - b.Y) < epsilon)
                   && (Math.Abs(a.Width - b.Width) < epsilon)
                   && (Math.Abs(a.Height - b.Height) < epsilon);
        }

        /// <summary>
        ///     Compares whether two <see cref="RectangleF" /> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="RectangleF" /> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="RectangleF" /> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return !(a == b);
        }

        /// <summary>
        ///     Gets whether or not the provided coordinates lie within the bounds of this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><c>true</c> if the provided coordinates lie inside this <see cref="RectangleF" />; <c>false</c> otherwise.</returns>
        public bool Contains(int x, int y)
        {
            return (X <= x) && (x < X + Width) && (Y <= y) && (y < Y + Height);
        }

        public RectangleF BoundingRectangle => this;

        /// <summary>
        ///     Gets whether or not the provided coordinates lie within the bounds of this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><c>true</c> if the provided coordinates lie inside this <see cref="RectangleF" />; <c>false</c> otherwise.</returns>
        public bool Contains(float x, float y)
        {
            return (X <= x) && (x < X + Width) && (Y <= y) && (y < Y + Height);
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="Point" /> lies within the bounds of this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="RectangleF" />.</param>
        /// <returns>
        ///     <c>true</c> if the provided <see cref="Point" /> lies inside this <see cref="RectangleF" />; <c>false</c>
        ///     otherwise.
        /// </returns>
        public bool Contains(Point value)
        {
            return (X <= value.X) && (value.X < X + Width) && (Y <= value.Y) && (value.Y < Y + Height);
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="Point" /> lies within the bounds of this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="RectangleF" />.</param>
        /// <param name="result">
        ///     <c>true</c> if the provided <see cref="Point" /> lies inside this <see cref="RectangleF" />;
        ///     <c>false</c> otherwise. As an output parameter.
        /// </param>
        public void Contains(ref Point value, out bool result)
        {
            result = (X <= value.X) && (value.X < X + Width) && (Y <= value.Y) && (value.Y < Y + Height);
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="Vector2" /> lies within the bounds of this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="RectangleF" />.</param>
        /// <returns>
        ///     <c>true</c> if the provided <see cref="Vector2" /> lies inside this <see cref="RectangleF" />; <c>false</c>
        ///     otherwise.
        /// </returns>
        public bool Contains(Vector2 value)
        {
            return (X <= value.X) && (value.X < X + Width) && (Y <= value.Y) && (value.Y < Y + Height);
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="Vector2" /> lies within the bounds of this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="RectangleF" />.</param>
        /// <param name="result">
        ///     <c>true</c> if the provided <see cref="Vector2" /> lies inside this <see cref="RectangleF" />;
        ///     <c>false</c> otherwise. As an output parameter.
        /// </param>
        public void Contains(ref Vector2 value, out bool result)
        {
            result = (X <= value.X) && (value.X < X + Width) && (Y <= value.Y) && (value.Y < Y + Height);
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="RectangleF" /> lies within the bounds of this <see cref="RectangleF" />
        ///     .
        /// </summary>
        /// <param name="value">The <see cref="RectangleF" /> to check for inclusion in this <see cref="RectangleF" />.</param>
        /// <returns>
        ///     <c>true</c> if the provided <see cref="RectangleF" />'s bounds lie entirely inside this
        ///     <see cref="RectangleF" />; <c>false</c> otherwise.
        /// </returns>
        public bool Contains(RectangleF value)
        {
            return (X <= value.X) && (value.X + value.Width <= X + Width) && (Y <= value.Y) &&
                   (value.Y + value.Height <= Y + Height);
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="RectangleF" /> lies within the bounds of this <see cref="RectangleF" />
        ///     .
        /// </summary>
        /// <param name="value">The <see cref="RectangleF" /> to check for inclusion in this <see cref="RectangleF" />.</param>
        /// <param name="result">
        ///     <c>true</c> if the provided <see cref="RectangleF" />'s bounds lie entirely inside this
        ///     <see cref="RectangleF" />; <c>false</c> otherwise. As an output parameter.
        /// </param>
        public void Contains(ref RectangleF value, out bool result)
        {
            result = (X <= value.X) && (value.X + value.Width <= X + Width) && (Y <= value.Y) &&
                     (value.Y + value.Height <= Y + Height);
        }

        /// <summary>
        ///     Compares whether current instance is equal to specified <see cref="Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is RectangleF && (this == (RectangleF) obj);
        }

        /// <summary>
        ///     Compares whether current instance is equal to specified <see cref="RectangleF" />.
        /// </summary>
        /// <param name="other">The <see cref="RectangleF" /> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(RectangleF other)
        {
            return this == other;
        }

        /// <summary>
        ///     Gets the hash code of this <see cref="RectangleF" />.
        /// </summary>
        /// <returns>Hash code of this <see cref="RectangleF" />.</returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        /// <summary>
        ///     Adjusts the edges of this <see cref="RectangleF" /> by specified horizontal and vertical amounts.
        /// </summary>
        /// <param name="horizontalAmount">Value to adjust the left and right edges.</param>
        /// <param name="verticalAmount">Value to adjust the top and bottom edges.</param>
        public void Inflate(int horizontalAmount, int verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount*2;
            Height += verticalAmount*2;
        }

        /// <summary>
        ///     Adjusts the edges of this <see cref="RectangleF" /> by specified horizontal and vertical amounts.
        /// </summary>
        /// <param name="horizontalAmount">Value to adjust the left and right edges.</param>
        /// <param name="verticalAmount">Value to adjust the top and bottom edges.</param>
        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount*2;
            Height += verticalAmount*2;
        }

        /// <summary>
        ///     Gets whether or not the other <see cref="RectangleF" /> intersects with this RectangleF.
        /// </summary>
        /// <param name="value">The other rectangle for testing.</param>
        /// <returns><c>true</c> if other <see cref="RectangleF" /> intersects with this rectangle; <c>false</c> otherwise.</returns>
        public bool Intersects(RectangleF value)
        {
            return (value.Left < Right) && (Left < value.Right) &&
                   (value.Top < Bottom) && (Top < value.Bottom);
        }


        /// <summary>
        ///     Gets whether or not the other <see cref="RectangleF" /> intersects with this rectangle.
        /// </summary>
        /// <param name="value">The other rectangle for testing.</param>
        /// <param name="result">
        ///     <c>true</c> if other <see cref="RectangleF" /> intersects with this rectangle; <c>false</c>
        ///     otherwise. As an output parameter.
        /// </param>
        public void Intersects(ref RectangleF value, out bool result)
        {
            result = (value.Left < Right) && (Left < value.Right) &&
                     (value.Top < Bottom) && (Top < value.Bottom);
        }

        /// <summary>
        ///     Creates a new <see cref="RectangleF" /> that contains overlapping region of two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="RectangleF" />.</param>
        /// <param name="value2">The second <see cref="RectangleF" />.</param>
        /// <returns>Overlapping region of the two rectangles.</returns>
        public static RectangleF Intersect(RectangleF value1, RectangleF value2)
        {
            RectangleF rectangle;
            Intersect(ref value1, ref value2, out rectangle);
            return rectangle;
        }

        /// <summary>
        ///     Creates a new <see cref="RectangleF" /> that contains overlapping region of two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="RectangleF" />.</param>
        /// <param name="value2">The second <see cref="RectangleF" />.</param>
        /// <param name="result">Overlapping region of the two rectangles as an output parameter.</param>
        public static void Intersect(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            if (value1.Intersects(value2))
            {
                var rightSide = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
                var leftSide = Math.Max(value1.X, value2.X);
                var topSide = Math.Max(value1.Y, value2.Y);
                var bottomSide = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);
                result = new RectangleF(leftSide, topSide, rightSide - leftSide, bottomSide - topSide);
            }
            else
            {
                result = new RectangleF(0, 0, 0, 0);
            }
        }

        /// <summary>
        ///     Changes the <see cref="Location" /> of this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="offsetX">The x coordinate to add to this <see cref="RectangleF" />.</param>
        /// <param name="offsetY">The y coordinate to add to this <see cref="RectangleF" />.</param>
        public void Offset(int offsetX, int offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        /// <summary>
        ///     Changes the <see cref="Location" /> of this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="offsetX">The x coordinate to add to this <see cref="RectangleF" />.</param>
        /// <param name="offsetY">The y coordinate to add to this <see cref="RectangleF" />.</param>
        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        /// <summary>
        ///     Changes the <see cref="Location" /> of this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="amount">The x and y components to add to this <see cref="RectangleF" />.</param>
        public void Offset(Point amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        /// <summary>
        ///     Changes the <see cref="Location" /> of this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="amount">The x and y components to add to this <see cref="RectangleF" />.</param>
        public void Offset(Vector2 amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        /// <summary>
        ///     Returns a <see cref="String" /> representation of this <see cref="RectangleF" /> in the format:
        ///     {X:[<see cref="X" />] Y:[<see cref="Y" />] Width:[<see cref="Width" />] Height:[<see cref="Height" />]}
        /// </summary>
        /// <returns><see cref="String" /> representation of this <see cref="RectangleF" />.</returns>
        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Width:" + Width + " Height:" + Height + "}";
        }

        /// <summary>
        ///     Creates a new <see cref="RectangleF" /> that completely contains two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="RectangleF" />.</param>
        /// <param name="value2">The second <see cref="RectangleF" />.</param>
        /// <returns>The union of the two rectangles.</returns>
        public static RectangleF Union(RectangleF value1, RectangleF value2)
        {
            var x = Math.Min(value1.X, value2.X);
            var y = Math.Min(value1.Y, value2.Y);
            return new RectangleF(x, y,
                Math.Max(value1.Right, value2.Right) - x,
                Math.Max(value1.Bottom, value2.Bottom) - y);
        }

        /// <summary>
        ///     Creates a new <see cref="RectangleF" /> that completely contains two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="RectangleF" />.</param>
        /// <param name="value2">The second <see cref="RectangleF" />.</param>
        /// <param name="result">The union of the two rectangles as an output parameter.</param>
        public static void Union(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            result.X = Math.Min(value1.X, value2.X);
            result.Y = Math.Min(value1.Y, value2.Y);
            result.Width = Math.Max(value1.Right, value2.Right) - result.X;
            result.Height = Math.Max(value1.Bottom, value2.Bottom) - result.Y;
        }

        /// <summary>
        ///     Creates a new <see cref="RectangleF" /> from two points.
        /// </summary>
        /// <param name="point0">The top left or bottom right corner</param>
        /// <param name="point1">The bottom left or top right corner</param>
        /// <returns></returns>
        public static RectangleF FromPoints(Vector2 point0, Vector2 point1)
        {
            var x = Math.Min(point0.X, point1.X);
            var y = Math.Min(point0.Y, point1.Y);
            var width = Math.Abs(point0.X - point1.X);
            var height = Math.Abs(point0.Y - point1.Y);
            var rectangle = new RectangleF(x, y, width, height);
            return rectangle;
        }

        /// <summary>
        ///     Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        ///     The amount of overlap between two intersecting rectangles. These
        ///     depth values can be negative depending on which wides the rectangles
        ///     intersect. This allows callers to determine the correct direction
        ///     to push objects in order to resolve collisions.
        ///     If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public Vector2 IntersectionDepth(RectangleF other)
        {
            // Calculate half sizes.
            var thisHalfWidth = Width/2.0f;
            var thisHalfHeight = Height/2.0f;
            var otherHalfWidth = other.Width/2.0f;
            var otherHalfHeight = other.Height/2.0f;

            // Calculate centers.
            var centerA = new Vector2(Left + thisHalfWidth, Top + thisHalfHeight);
            var centerB = new Vector2(other.Left + otherHalfWidth, other.Top + otherHalfHeight);

            // Calculate current and minimum-non-intersecting distances between centers.
            var distanceX = centerA.X - centerB.X;
            var distanceY = centerA.Y - centerB.Y;
            var minDistanceX = thisHalfWidth + otherHalfWidth;
            var minDistanceY = thisHalfHeight + otherHalfHeight;

            // If we are not intersecting at all, return (0, 0).
            if ((Math.Abs(distanceX) >= minDistanceX) || (Math.Abs(distanceY) >= minDistanceY))
                return Vector2.Zero;

            // Calculate and return intersection depths.
            var depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            var depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
    }
}