using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    [DataContract]
    public struct EllipseF : IEquatable<EllipseF>, IEquatableByRef<EllipseF>, IShapeF
    {
        [DataMember] public Vector2 Center { get; set; }
        [DataMember] public float RadiusX { get; set; }
        [DataMember] public float RadiusY { get; set; }

        public Point2 Position
        {
            get => Center;
            set => Center = value;
        }

        public EllipseF(Vector2 center, float radiusX, float radiusY)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }
    
        public float Left => Center.X - RadiusX;
        public float Top => Center.Y - RadiusY;
        public float Right => Center.X + RadiusX;
        public float Bottom => Center.Y + RadiusY;

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

        [Pure]
        public bool Contains(float x, float y)
        {
            float xCalc = (float) (Math.Pow(x - Center.X, 2) / Math.Pow(RadiusX, 2));
            float yCalc = (float) (Math.Pow(y - Center.Y, 2) / Math.Pow(RadiusY, 2));

            return xCalc + yCalc <= 1;
        }

        [Pure]
        public bool Contains(Point2 point)
        {
            return Contains(point.X, point.Y);
        }

        [Pure]
        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }

        [Pure]
        public Point2 ClosestPointTo(Point2 point)
        {
            Vector2 offset = point - Center;
            float angle = (float)Math.Atan2(offset.Y, offset.X);
            float x = Center.X + RadiusX * (float)Math.Cos(angle);
            float y = Center.Y + RadiusY * (float)Math.Sin(angle);
            return new Point2(x, y);
        }

        [Pure]
        public bool Intersects(EllipseF ellipse)
        {
            var closestPoint = ClosestPointTo(ellipse.Center);
            return ellipse.Contains(closestPoint);
        }

        [Pure]
        public EllipseF Rotate(float angle)
        {
            float x = Math.Abs(RadiusX * (float)Math.Cos(angle)) + Math.Abs(RadiusY * (float)Math.Sin(angle));
            float y = Math.Abs(RadiusX * (float)Math.Sin(angle)) + Math.Abs(RadiusY * (float)Math.Cos(angle));
            return new EllipseF(Center, x, y);
        }

        [Pure]
        public EllipseF Reflect(Vector2 angle) => Reflect(angle.ToAngle());

        [Pure]
        public EllipseF Reflect(float angle)
        {
            Point2 center = Center;
            EllipseF rotation = Rotate(-angle);
            EllipseF conj = rotation.Conjugate;
            EllipseF result = conj.Rotate(angle);
            result.Position = center;
            return result;
        }

        [Pure]
        private EllipseF Conjugate => new(Center, -RadiusX, RadiusY);

        public bool Equals(EllipseF ellispse)
        {
            return Equals(ref ellispse);
        }

        public bool Equals(ref EllipseF ellispse)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return ellispse.Center == Center 
                   && ellispse.RadiusX == RadiusX 
                   && ellispse.RadiusY == RadiusY;
        }

        public override bool Equals(object obj)
        {
            return obj is EllipseF && Equals((EllipseF)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Center.GetHashCode();
                hashCode = (hashCode * 397) ^ RadiusX.GetHashCode();
                hashCode = (hashCode * 397) ^ RadiusY.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Centre: {Center}, RadiusX: {RadiusX}, RadiusY: {RadiusY}";
        }

        public static bool operator ==(EllipseF first, EllipseF second)
        {
            return first.Equals(ref second);
        }

        public static bool operator !=(EllipseF first, EllipseF second)
        {
            return !(first == second);
        }
    }
}
