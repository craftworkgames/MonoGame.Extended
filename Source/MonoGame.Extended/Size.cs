using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public struct Size : IEquatable<Size>
    {
        public Size(int width, int height)
            : this()
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }

        public static Size Empty => new Size(0, 0);

        public static Size MaxValue => new Size(int.MaxValue, int.MaxValue);

        public bool IsEmpty => (Width == 0) && (Height == 0);

        public override int GetHashCode()
        {
            unchecked
            {
                return Width.GetHashCode() + Height.GetHashCode();
            }
        }

        public static bool operator ==(Size a, Size b)
        {
            return (a.Width == b.Width) && (a.Height == b.Height);
        }

        public static bool operator !=(Size a, Size b)
        {
            return !(a == b);
        }

        public bool Equals(Size other)
        {
            return (Width == other.Width) && (Height == other.Height);
        }

        public static implicit operator Point(Size size)
        {
            return new Point(size.Width, size.Height);
        }

        public static implicit operator Vector2(Size size)
        {
            return new Vector2(size.Width, size.Height);
        }

        public static implicit operator SizeF(Size size)
        {
            return new SizeF(size.Width, size.Height);
        }

        public static SizeF operator /(Size size, float value)
        {
            return new SizeF(size.Width / value, size.Height / value);
        }

        public static SizeF operator *(Size size, float value)
        {
            return new SizeF(size.Width * value, size.Height * value);
        }

        public static Size operator *(Size size, int value)
        {
            return new Size(size.Width * value, size.Height * value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Size && Equals((Size) obj);
        }

        public override string ToString()
        {
            return $"Width: {Width}, Height: {Height}";
        }
    }
}