using System;
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

        public Vector2 Position
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

        public bool Contains(float x, float y)
        {
            float xCalc = (float) (Math.Pow(x - Center.X, 2) / Math.Pow(RadiusX, 2));
            float yCalc = (float) (Math.Pow(y - Center.Y, 2) / Math.Pow(RadiusY, 2));

            return xCalc + yCalc <= 1;
        }

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }

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
