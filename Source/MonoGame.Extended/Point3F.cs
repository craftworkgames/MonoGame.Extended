using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Point3F : IEquatable<Point3F>
    {
        public static readonly Point3F Zero;

        public float X;
        public float Y;
        public float Z;

        public Point3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool IsZero
        {
            get
            {
                // ReSharper disable CompareOfFloatsByEqualityOperator
                return X == 0f && Y == 0f && Z == 0f;
                // ReSharper restore CompareOfFloatsByEqualityOperator
            }
        }

        public static Vector3 operator -(Point3F firstPoint, Point3F secondPoint)
        {
            return new Vector3(firstPoint.X - secondPoint.X, firstPoint.Y - secondPoint.Y, firstPoint.Z - secondPoint.Z);
        }

        public static bool operator ==(Point3F firstPoint, Point3F secondPoint)
        {
            return firstPoint.Equals(secondPoint);
        }

        public static bool operator !=(Point3F firstPoint, Point3F secondPoint)
        {
            return !(firstPoint == secondPoint);
        }

        public bool Equals(Point3F other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return X == other.X && Y == other.Y && Z == other.Z;
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
            return HashCodeHelper.GetHashCode(X, Y, Z);
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }
        
        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
    }
}