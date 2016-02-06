using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.BoundingVolumes
{
    // IEquatable<T>: see Struct Design in Framework Design Guidelines: https://msdn.microsoft.com/en-us/library/ms229031(v=vs.110).aspx
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct AxisAlignedBoundingBox2D : IEquatable<AxisAlignedBoundingBox2D>
    {
        // these fields are not read-only because this struct is NOT suppose to be hashed and we want to work with these fields directly for performance
        [DataMember]
        public Vector2 Center;
        [DataMember]
        public Vector2 HalfSize;

        public AxisAlignedBoundingBox2D(Vector2 center, Vector2 halfSize)
        {
            Center = center;
            HalfSize = halfSize;
        }

        public bool Intersects(AxisAlignedBoundingBox2D value)
        {
            // exit with no intersection if seperated along an axis
            return !(Math.Abs(Center.X - value.Center.X) > HalfSize.X + HalfSize.Y) && !(Math.Abs(Center.Y - value.Center.Y) > HalfSize.Y + HalfSize.Y);
        }

        public void Intersects(AxisAlignedBoundingBox2D value, out bool result)
        {
            // exit with no intersection if seperated along an axis
            result = !(Math.Abs(Center.X - value.Center.X) > HalfSize.X + HalfSize.Y) && !(Math.Abs(Center.Y - value.Center.Y) > HalfSize.Y + HalfSize.Y);
        }

        public void Transform(ref Matrix transformMatrix)
        {
            Center.X = Center.X * transformMatrix.M11 + Center.Y * transformMatrix.M21 + transformMatrix.M41;
            Center.Y = Center.X * transformMatrix.M12 + Center.Y * transformMatrix.M22 + transformMatrix.M42;
            HalfSize.X = HalfSize.X * Math.Abs(transformMatrix.M11) + HalfSize.Y * Math.Abs(transformMatrix.M12);
            HalfSize.Y = HalfSize.X * Math.Abs(transformMatrix.M21) + HalfSize.Y * Math.Abs(transformMatrix.M22);
        }

        public static bool operator ==(AxisAlignedBoundingBox2D a, AxisAlignedBoundingBox2D b)
        {
            return a.Center == b.Center && a.HalfSize == b.HalfSize;
        }

        public static bool operator !=(AxisAlignedBoundingBox2D a, AxisAlignedBoundingBox2D b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is AxisAlignedBoundingBox2D)
            {
                return Equals((AxisAlignedBoundingBox2D)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // 397 is a prime suggested by resharper for overflowing the resulting integer for better distribution
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return (Center.GetHashCode() * 397) ^ HalfSize.GetHashCode();
                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }

        public bool Equals(AxisAlignedBoundingBox2D other)
        {
            return Center == other.Center && HalfSize == other.HalfSize;
        }

        internal string DebugDisplayString
        {
            get { return $"Center = {Center}, HalfSize= {HalfSize}"; }
        }

        public override string ToString()
        {
            return $"{{Center = {Center}, HalfSize= {HalfSize}}}";
        }
    }
}
