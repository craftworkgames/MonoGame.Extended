using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    /// <summary>
    ///     Describes a 2D-circle.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct CircleF : IShapeF, IEquatable<CircleF>
    {
        /// <summary>
        ///     The point representing the center of this <see cref="CircleF" />.
        /// </summary>
        [DataMember]
        public Vector2 Center { get; set; }

        /// <summary>
        ///     The radius from the center of this <see cref="CircleF" />.
        /// </summary>
        [DataMember]
        public float Radius { get; set; }

        /// <summary>
        ///     Returns a <see cref="CircleF" /> with Point = Vector2.Zero and Radius= 0.
        /// </summary>
        public static CircleF Empty { get; } = new CircleF();

        /// <summary>
        ///     Returns the x coordinate of the far left point of this <see cref="CircleF" />.
        /// </summary>
        public float Left => Center.X - Radius;

        /// <summary>
        ///     Returns the x coordinate of the far right point of this <see cref="CircleF" />.
        /// </summary>
        public float Right => Center.X + Radius;

        /// <summary>
        ///     Returns the y coordinate of the far top point of this <see cref="CircleF" />.
        /// </summary>
        public float Top => Center.Y - Radius;

        /// <summary>
        ///     Returns the y coordinate of the far bottom point of this <see cref="CircleF" />.
        /// </summary>
        public float Bottom => Center.Y + Radius;

        /// <summary>
        ///     The center coordinates of this <see cref="CircleF" />.
        /// </summary>
        public Point Location
        {
            get { return Center.ToPoint(); }
            set { Center = value.ToVector2(); }
        }

        /// <summary>
        ///     Returns the diameter of this <see cref="CircleF" />
        /// </summary>
        public float Diameter => Radius*2.0f;

        /// <summary>
        ///     Returns the Circumference of this <see cref="CircleF" />
        /// </summary>
        public float Circumference => 2.0f*MathHelper.Pi*Radius;

        /// <summary>
        ///     Whether or not this <see cref="CircleF" /> has a <see cref="Center" /> and
        ///     <see cref="Radius" /> of 0.
        /// </summary>
        public bool IsEmpty => Radius.Equals(0) && (Center == Vector2.Zero);

        internal string DebugDisplayString => $"{Center} {Radius}";

        /// <summary>
        ///     Creates a new instance of <see cref="CircleF" /> struct, with the specified
        ///     position, and radius
        /// </summary>
        /// <param name="center">The position of the center of the created <see cref="CircleF" />.</param>
        /// <param name="radius">The radius of the created <see cref="CircleF" />.</param>
        public CircleF(Vector2 center, float radius)
            : this()
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        ///     Compares whether two <see cref="CircleF" /> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="CircleF" /> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="CircleF" /> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(CircleF a, CircleF b)
        {
            return (a.Center == b.Center) && a.Radius.Equals(b.Radius);
        }

        /// <summary>
        ///     Compares whether two <see cref="CircleF" /> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="CircleF" /> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="CircleF" /> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(CircleF a, CircleF b)
        {
            return !(a == b);
        }

        /// <summary>
        ///     Gets the point at the edge of this <see cref="CircleF" /> from the provided angle
        /// </summary>
        /// <param name="angle">an angle in radians</param>
        /// <returns><see cref="Vector2" /> representing the point on this <see cref="CircleF" />'s surface at the specified angle</returns>
        public Vector2 GetPointAlongEdge(float angle)
        {
            return new Vector2(Center.X + Radius*(float) Math.Cos(angle),
                Center.Y + Radius*(float) Math.Sin(angle));
        }


        public RectangleF BoundingRectangle
        {
            get
            {
                var minX = Left;
                var minY = Top;
                var maxX = Right;
                var maxY = Bottom;
                return new RectangleF(minX, minY, maxX - minX, maxY - minY);
            }
        }

        /// <summary>
        ///     Gets whether or not the provided coordinates lie within the bounds of this <see cref="CircleF" />.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><c>true</c> if the provided coordinates lie inside this <see cref="CircleF" />; <c>false</c> otherwise.</returns>
        public bool Contains(float x, float y)
        {
            return (new Vector2(x, y) - Center).LengthSquared() <= Radius*Radius;
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="Point" /> lies within the bounds of this <see cref="CircleF" />.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="CircleF" />.</param>
        /// <returns>
        ///     <c>true</c> if the provided <see cref="Point" /> lies inside this <see cref="CircleF" />; <c>false</c>
        ///     otherwise.
        /// </returns>
        public bool Contains(Point value)
        {
            return (value.ToVector2() - Center).LengthSquared() <= Radius*Radius;
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="Point" /> lies within the bounds of this <see cref="CircleF" />.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="CircleF" />.</param>
        /// <param name="result">
        ///     <c>true</c> if the provided <see cref="Point" /> lies inside this <see cref="CircleF" />;
        ///     <c>false</c> otherwise. As an output parameter.
        /// </param>
        public void Contains(ref Point value, out bool result)
        {
            result = (value.ToVector2() - Center).LengthSquared() <= Radius*Radius;
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="Vector2" /> lies within the bounds of this <see cref="CircleF" />.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="CircleF" />.</param>
        /// <returns>
        ///     <c>true</c> if the provided <see cref="Vector2" /> lies inside this <see cref="CircleF" />; <c>false</c>
        ///     otherwise.
        /// </returns>
        public bool Contains(Vector2 value)
        {
            return (value - Center).LengthSquared() <= Radius*Radius;
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="Vector2" /> lies within the bounds of this <see cref="CircleF" />.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="CircleF" />.</param>
        /// <param name="result">
        ///     <c>true</c> if the provided <see cref="Vector2" /> lies inside this <see cref="CircleF" />;
        ///     <c>false</c> otherwise. As an output parameter.
        /// </param>
        public void Contains(ref Vector2 value, out bool result)
        {
            result = (value - Center).LengthSquared() <= Radius*Radius;
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="CircleF" /> lies within the bounds of this <see cref="CircleF" />.
        /// </summary>
        /// <param name="value">The <see cref="CircleF" /> to check for inclusion in this <see cref="CircleF" />.</param>
        /// <returns>
        ///     <c>true</c> if the provided <see cref="CircleF" />'s center lie entirely inside this <see cref="CircleF" />;
        ///     <c>false</c> otherwise.
        /// </returns>
        public bool Contains(CircleF value)
        {
            var distanceOfCenter = value.Center - Center;
            var radii = Radius - value.Radius;

            return distanceOfCenter.X*distanceOfCenter.X + distanceOfCenter.Y*distanceOfCenter.Y <=
                   Math.Abs(radii*radii);
        }

        /// <summary>
        ///     Gets whether or not the provided <see cref="CircleF" /> lies within the bounds of this <see cref="CircleF" />.
        /// </summary>
        /// <param name="value">The <see cref="CircleF" /> to check for inclusion in this <see cref="CircleF" />.</param>
        /// <param name="result">
        ///     <c>true</c> if the provided <see cref="CircleF" />'s center lie entirely inside this
        ///     <see cref="CircleF" />; <c>false</c> otherwise. As an output parameter.
        /// </param>
        public void Contains(ref CircleF value, out bool result)
        {
            var distanceOfCenter = value.Center - Center;
            var radii = Radius - value.Radius;

            result = distanceOfCenter.X*distanceOfCenter.X + distanceOfCenter.Y*distanceOfCenter.Y <=
                     Math.Abs(radii*radii);
        }

        /// <summary>
        ///     Compares whether current instance is equal to specified <see cref="Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is CircleF && (this == (CircleF) obj);
        }

        /// <summary>
        ///     Compares whether current instance is equal to specified <see cref="CircleF" />.
        /// </summary>
        /// <param name="other">The <see cref="CircleF" /> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(CircleF other)
        {
            return this == other;
        }

        /// <summary>
        ///     Gets the hash code of this <see cref="CircleF" />.
        /// </summary>
        /// <returns>Hash code of this <see cref="CircleF" />.</returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return Center.GetHashCode() ^ Radius.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        /// <summary>
        ///     Adjusts the size of this <see cref="CircleF" /> by specified radius amount.
        /// </summary>
        /// <param name="radiusAmount">Value to adjust the radius by.</param>
        public void Inflate(float radiusAmount)
        {
            Center -= new Vector2(radiusAmount);
            Radius += radiusAmount*2;
        }

        /// <summary>
        ///     Gets whether or not a specified <see cref="CircleF" /> intersects with this <see cref="CircleF" />.
        /// </summary>
        /// <param name="value">Other <see cref="CircleF" />.</param>
        /// <returns>
        ///     <c>true</c> if other <see cref="CircleF" /> intersects with this <see cref="CircleF" />; <c>false</c>
        ///     otherwise.
        /// </returns>
        public bool Intersects(CircleF value)
        {
            var distanceOfCenter = value.Center - Center;
            var radii = Radius + value.Radius;

            return distanceOfCenter.X*distanceOfCenter.X + distanceOfCenter.Y*distanceOfCenter.Y < radii*radii;
        }

        /// <summary>
        ///     Gets whether or not a specified <see cref="CircleF" /> intersects with this <see cref="CircleF" />.
        /// </summary>
        /// <param name="value">Other <see cref="CircleF" />.</param>
        /// <param name="result">
        ///     <c>true</c> if other <see cref="CircleF" /> intersects with this <see cref="CircleF" />;
        ///     <c>false</c> otherwise. As an output parameter.
        /// </param>
        public void Intersects(ref CircleF value, out bool result)
        {
            var distanceOfCenter = value.Center - Center;
            var radii = Radius + value.Radius;

            result = distanceOfCenter.X*distanceOfCenter.X + distanceOfCenter.Y*distanceOfCenter.Y < radii*radii;
        }

        /// <summary>
        ///     Gets whether or not a specified <see cref="Rectangle" /> intersects with this <see cref="CircleF" />.
        /// </summary>
        /// <param name="value">Other <see cref="Rectangle" />.</param>
        /// <returns>
        ///     <c>true</c> if other <see cref="Rectangle" /> intersects with this <see cref="CircleF" />; <c>false</c>
        ///     otherwise.
        /// </returns>
        public bool Intersects(Rectangle value)
        {
            var distance = new Vector2(Math.Abs(Center.X - value.Center.X), Math.Abs(Center.Y - value.Center.Y));

            if (distance.X > value.Width/2.0f + Radius)
                return false;

            if (distance.Y > value.Height/2.0f + Radius)
                return false;

            if (distance.X <= value.Width/2.0f)
                return true;

            if (distance.Y <= value.Height/2.0f)
                return true;

            var distanceOfCorners =
                (distance.X - value.Width/2.0f)*
                (distance.X - value.Width/2.0f) +
                (distance.Y - value.Height/2.0f)*
                (distance.Y - value.Height/2.0f);

            return distanceOfCorners <= Radius*Radius;
        }

        /// <summary>
        ///     Gets whether or not a specified <see cref="Rectangle" /> intersects with this <see cref="CircleF" />.
        /// </summary>
        /// <param name="value">Other <see cref="Rectangle" />.</param>
        /// <param name="result">
        ///     <c>true</c> if other <see cref="Rectangle" /> intersects with this <see cref="CircleF" />;
        ///     <c>false</c> otherwise. As an output parameter.
        /// </param>
        public void Intersects(ref Rectangle value, out bool result)
        {
            result = Intersects(value);
        }

        /// <summary>
        ///     Changes the <see cref="Location" /> of this <see cref="CircleF" />.
        /// </summary>
        /// <param name="offsetX">The x coordinate to add to this <see cref="CircleF" />.</param>
        /// <param name="offsetY">The y coordinate to add to this <see cref="CircleF" />.</param>
        public void Offset(float offsetX, float offsetY)
        {
            Offset(new Vector2(offsetX, offsetY));
        }

        /// <summary>
        ///     Changes the <see cref="Location" /> of this <see cref="CircleF" />.
        /// </summary>
        /// <param name="amount">The x and y components to add to this <see cref="CircleF" />.</param>
        public void Offset(Point amount)
        {
            Offset(amount.ToVector2());
        }

        /// <summary>
        ///     Changes the <see cref="Location" /> of this <see cref="CircleF" />.
        /// </summary>
        /// <param name="amount">The x and y components to add to this <see cref="CircleF" />.</param>
        public void Offset(Vector2 amount)
        {
            Center += new Vector2(amount.X, amount.Y);
        }

        /// <summary>
        ///     Returns a <see cref="String" /> representation of this <see cref="CircleF" /> in the format:
        ///     {Center:[<see cref="Center" />] Radius:[<see cref="Radius" />]}
        /// </summary>
        /// <returns><see cref="String" /> representation of this <see cref="CircleF" />.</returns>
        public override string ToString()
        {
            return $"{{Center:{Center} Radius:{Radius}}}";
        }

        /// <summary>
        ///     Creates a <see cref="Rectangle" /> large enough to fit this <see cref="CircleF" />
        /// </summary>
        /// <returns><see cref="Rectangle" /> which contains this <see cref="CircleF" /></returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle((int) (Center.X - Radius), (int) (Center.Y - Radius), (int) Radius*2, (int) Radius*2);
        }
    }
}