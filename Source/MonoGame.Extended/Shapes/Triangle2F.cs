using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Triangle2F : IEquatable<Triangle2F>
    {
        public Point2F FirstPoint;
        public Point2F SecondPoint;
        public Point2F ThirdPoint;

        public float Area
        {
            get { return GetArea(); }
        }

        public Triangle2F(Point2F firstPoint, Point2F secondPoint, Point2F thirdPoint)
        {
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
            ThirdPoint = thirdPoint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetDoubleSignedArea()
        {
            return FirstPoint.X * (SecondPoint.Y - ThirdPoint.Y) + SecondPoint.X * (ThirdPoint.Y - FirstPoint.Y) + ThirdPoint.X * (FirstPoint.Y - SecondPoint.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetSignedArea()
        {
            return GetDoubleSignedArea() * 0.5f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArea()
        {
            return Math.Abs(GetSignedArea());
        }

        public Vector2 GetBarycentricCoordinates(Point2F point)
        {
            var v0 = ThirdPoint - FirstPoint;
            var v1 = SecondPoint - FirstPoint;
            var v2 = point - FirstPoint;

            var dot00 = v0.Dot(v0);
            var dot01 = v0.Dot(v1);
            var dot02 = v0.Dot(v2);
            var dot11 = v1.Dot(v1);
            var dot12 = v1.Dot(v2);

            var inverseDenominator = 1 / (dot00 * dot11 - dot01 * dot01);
            var u = (dot11 * dot02 - dot01 * dot12) * inverseDenominator;
            var v = (dot00 * dot12 - dot01 * dot02) * inverseDenominator;
            return new Vector2(u, v);
        }

        public bool Contains(Point2F point)
        {
            var b = GetBarycentricCoordinates(point);
            return b.X >= 0 && b.Y >= 0 && b.X + b.Y < 1;
        }

        public static bool operator ==(Triangle2F firstTriangle, Triangle2F secondTriangle)
        {
            return firstTriangle.Equals(secondTriangle);
        }

        public static bool operator !=(Triangle2F firstTriangle, Triangle2F secondTriangle)
        {
            return !(firstTriangle == secondTriangle);
        }

        public bool Equals(Triangle2F other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return FirstPoint == other.FirstPoint && SecondPoint == other.SecondPoint && ThirdPoint == other.ThirdPoint;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Triangle2F))
                return false;
            var other = (Triangle2F)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return $"{{A={FirstPoint}, B={SecondPoint}, C={ThirdPoint}}}";
        }
    }
}
