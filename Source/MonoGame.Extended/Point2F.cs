using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Point2F : IEquatable<Point2F>
    {
        public static readonly Point2F Zero;

        public float X;
        public float Y;

        public Point2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool IsZero
        {
            get
            {
                // ReSharper disable CompareOfFloatsByEqualityOperator
                return X == 0f && Y == 0f;
                // ReSharper restore CompareOfFloatsByEqualityOperator
            }
        }

        public static Vector2 operator -(Point2F firstPoint, Point2F secondPoint)
        {
            return new Vector2(firstPoint.X - secondPoint.X, firstPoint.Y - secondPoint.Y);
        }

        public static bool operator ==(Point2F firstPoint, Point2F secondPoint)
        {
            return firstPoint.Equals(secondPoint);
        }

        public static bool operator !=(Point2F firstPoint, Point2F secondPoint)
        {
            return !(firstPoint == secondPoint);
        }

        public bool Equals(Point2F other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return X == other.X && Y == other.Y;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point3F))
                return false;
            var other = (Point3F)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return HashCodeHelper.GetHashCode(X, Y);
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}