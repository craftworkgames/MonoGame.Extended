using System;

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

        public int Width { get; private set; }
        public int Height { get; private set; }

        public static Size Empty
        {
            get { return new Size(0, 0); }
        }

        public bool IsEmpty
        {
            get { return Width == 0 && Height == 0; }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Width.GetHashCode() + Height.GetHashCode();
            }
        }

        public static bool operator ==(Size a, Size b)
        {
            return a.Width == b.Width && a.Height == b.Height;
        }

        public static bool operator !=(Size a, Size b)
        {
            return !(a == b);
        }

        public bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Size && Equals((Size)obj);
        }

        public override string ToString()
        {
            return string.Format("Width: {0}, Height: {1}", Width, Height);
        }
    }
}