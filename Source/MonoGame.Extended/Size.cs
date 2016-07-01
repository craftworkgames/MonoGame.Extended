using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public struct Size : IEquatable<Size>
    {
        public int Width;
        public int Height;

        public static Size Empty => new SizeF(0, 0);

        public bool IsEmpty => Width == 0 && Height == 0;

        public Size(int width, int height)
            : this()
        {
            Width = width;
            Height = height;
        }

        public static Size operator +(Size size1, Size size2)
        {
            return Add(size1, size2);
        }

        public static Size operator -(Size size1, Size size2)
        {
            return Subtract(size1, size2);
        }

        public static Size operator -(Size value)
        {
            value.Width = -value.Width;
            value.Height = -value.Height;
            return value;
        }

        public static Size operator *(Size size1, Size size2)
        {
            return Multiply(size1, size2);
        }

        public static Size operator *(Size size, int scaleFactor)
        {
            return Multiply(size, scaleFactor);
        }

        public static Size operator *(int scaleFactor, Size size)
        {
            return Multiply(size, scaleFactor);
        }

        public static Size operator *(Size size, float scaleFactor)
        {
            return Multiply(size, scaleFactor);
        }

        public static Size operator *(float scaleFactor, Size size)
        {
            return Multiply(size, scaleFactor);
        }

        public static Size operator /(Size size1, Size size2)
        {
            return Divide(size1, size2);
        }

        public static Size operator /(Size size1, float divider)
        {
            return Divide(size1, divider);
        }

        public static Size operator /(Size size1, int divider)
        {
            return Divide(size1, divider);
        }

        public static bool operator ==(Size size1, Size size2)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return size1.Width == size2.Width && size1.Height == size2.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public static bool operator !=(Size size1, Size size2)
        {
            return !(size1 == size2);
        }

        public static implicit operator Point(Size size)
        {
            return new Point(size.Width, size.Height);
        }

        public static implicit operator Size(SizeF size)
        {
            return new Size((int)size.Width, (int)size.Height);
        }

        public static implicit operator Size(Rectangle rectangle)
        {
            return new Size(rectangle.Width, rectangle.Height);
        }

        public static implicit operator Vector2(Size size)
        {
            return new Vector2(size.Width, size.Height);
        }

        public static implicit operator Size(Vector2 size)
        {
            return new Size((int)size.X, (int)size.Y);
        }

        public static Size Add(Size size1, Size size2)
        {
            return new Size(size1.Width + size2.Width, size1.Height + size2.Height);
        }

        public static Size Subtract(Size size1, Size size2)
        {
            return new Size(size1.Width - size2.Width, size1.Height - size2.Height);
        }

        public static Size Multiply(Size size1, Size size2)
        {
            return new Size(size1.Width * size2.Width, size1.Height * size2.Height);
        }

        public static Size Multiply(Size size1, float scaleFactor)
        {
            return new Size((int)(size1.Width * scaleFactor), (int)(size1.Height * scaleFactor));
        }

        public static Size Multiply(Size size1, int scaleFactor)
        {
            return new Size(size1.Width * scaleFactor, size1.Height * scaleFactor);
        }

        public static Size Divide(Size size1, Size size2)
        {
            return new Size(size1.Width / size2.Width, size1.Height / size2.Height);
        }

        public static Size Divide(Size size1, float scaleFactor)
        {
            return new Size((int)(size1.Width / scaleFactor), (int)(size1.Height / scaleFactor));
        }

        public static Size Divide(Size size1, int scaleFactor)
        {
            return new Size(size1.Width / scaleFactor, size1.Height / scaleFactor);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size))
            {
                return false;
            }

            return Equals((Size)obj);
        }

        public bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
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