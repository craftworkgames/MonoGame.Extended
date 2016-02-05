using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    // See http://www.dotnetperls.com/struct
    // See Struct Design in Framework Design Guidelines: https://msdn.microsoft.com/en-us/library/ms229031(v=vs.110).aspx
    public struct SizeF : IEquatable<SizeF>
    {
        public readonly float Width;
        public readonly float Height;

        public SizeF(float width, float height)
            : this()
        {
            Width = width;
            Height = height;
        }

        public static SizeF Empty
        {
            get { return new SizeF(0, 0); }
        }

        public static SizeF MaxValue
        {
            get { return new SizeF(float.MaxValue, float.MaxValue); }
        }

        public bool IsEmpty
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            get { return Width == 0 && Height == 0; }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override int GetHashCode()
        {
            unchecked
            {
               
                return Width.GetHashCode() + Height.GetHashCode();
            }
        }

        public static bool operator ==(SizeF a, SizeF b)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return a.Width == b.Width && a.Height == b.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public static bool operator !=(SizeF a, SizeF b)
        {
            return !(a == b);
        }

        public static implicit operator Point(SizeF size)
        {
            return new Point((int)size.Width, (int)size.Height);
        }

        public static implicit operator Vector2(SizeF size)
        {
            return new Vector2(size.Width, size.Height);
        }

        public override bool Equals(object obj)
        {
            if (obj is SizeF)
            {
                return Equals((SizeF)obj);
            }

            return false;
        }

        public bool Equals(SizeF other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return Width == other.Width && Height == other.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override string ToString()
        {
            return $"Width: {Width}, Height: {Height}";
        }
    }
}