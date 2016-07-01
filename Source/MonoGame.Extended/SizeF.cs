using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public struct SizeF : IEquatable<SizeF>
    {
        public float Width;
        public float Height;

        public static SizeF Empty => new SizeF(0, 0);

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public bool IsEmpty => Width == 0 && Height == 0;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public SizeF(float value)
            : this()
        {
            Width = value;
            Height = value;
        }

        public SizeF(float width, float height)
            : this()
        {
            Width = width;
            Height = height;
        }

        public static SizeF operator +(SizeF size1, SizeF size2)
        {
            return Add(size1, size2);
        }

        public static SizeF operator -(SizeF size1, SizeF size2)
        {
            return Subtract(size1, size2);
        }

        public static SizeF operator -(SizeF value)
        {
            value.Width = -value.Width;
            value.Height = -value.Height;
            return value;
        }

        public static SizeF operator *(SizeF size1, SizeF size2)
        {
            return Multiply(size1, size2);
        }

        public static SizeF operator *(SizeF size, float scaleFactor)
        {
            return Multiply(size, scaleFactor);
        }

        public static SizeF operator *(float scaleFactor, SizeF size)
        {
            return Multiply(size, scaleFactor);
        }

        public static SizeF operator /(SizeF size1, SizeF size2)
        {
            return Divide(size1, size2);
        }

        public static SizeF operator /(SizeF size1, float divider)
        {
            return Divide(size1, divider);
        }

        public static bool operator ==(SizeF size1, SizeF size2)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return size1.Width == size2.Width && size1.Height == size2.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public static bool operator !=(SizeF size1, SizeF size2)
        {
            return !(size1 == size2);
        }

        public static implicit operator SizeF(float value)
        {
            return new SizeF(value);
        }

        public static implicit operator Point(SizeF size)
        {
            return new Point((int)size.Width, (int)size.Height);
        }

        public static implicit operator SizeF(Size size)
        {
            return new SizeF(size.Width, size.Height);
        }

        public static implicit operator SizeF(Rectangle rectangle)
        {
            return new Size(rectangle.Width, rectangle.Height);
        }

        public static implicit operator Vector2(SizeF size)
        {
            return new Vector2(size.Width, size.Height);
        }

        public static implicit operator SizeF(Vector2 size)
        {
            return new SizeF(size.X, size.Y);
        }

        public static implicit operator SizeF(Point point)
        {
            return new SizeF(point.X, point.Y);
        }

        public static SizeF Add(SizeF size1, SizeF size2)
        {
            return new SizeF(size1.Width + size2.Width, size1.Height + size2.Height);
        }

        public static SizeF Subtract(SizeF size1, SizeF size2)
        {
            return new SizeF(size1.Width - size2.Width, size1.Height - size2.Height);
        }

        public static SizeF Multiply(SizeF size1, SizeF size2)
        {
            return new SizeF(size1.Width * size2.Width, size1.Height * size2.Height);
        }

        public static SizeF Multiply(SizeF size1, float scaleFactor)
        {
            return new SizeF((int)(size1.Width * scaleFactor), (int)(size1.Height * scaleFactor));
        }

        public static SizeF Divide(SizeF size1, SizeF size2)
        {
            return new SizeF(size1.Width / size2.Width, size1.Height / size2.Height);
        }

        public static SizeF Divide(SizeF size1, float scaleFactor)
        {
            return new SizeF((int)(size1.Width / scaleFactor), (int)(size1.Height / scaleFactor));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SizeF))
            {
                return false;
            }

            return Equals((SizeF)obj);
        }

        public bool Equals(SizeF other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return Width == other.Width && Height == other.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return HashCodeHelper.GetHashCode(Width, Height);
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        public override string ToString()
        {
            return $"Width={Width}, Height={Height}";
        }
    }
}