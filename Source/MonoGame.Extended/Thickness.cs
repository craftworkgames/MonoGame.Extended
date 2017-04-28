using System;
using System.Linq;

namespace MonoGame.Extended
{
    public struct Thickness : IEquatable<Thickness>
    {
        public Thickness(int all)
            : this(all, all, all, all)
        {
        }

        public Thickness(int leftRight, int topBottom)
            : this(leftRight, topBottom, leftRight, topBottom)
        {
        }

        public Thickness(int left, int top, int right, int bottom)
            : this()
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public Size2 Size => new Size2(Left + Right, Top + Bottom);

        public override bool Equals(object obj)
        {
            if (obj is Thickness)
            {
                var other = (Thickness)obj;
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Left;
                hashCode = (hashCode * 397) ^ Top;
                hashCode = (hashCode * 397) ^ Right;
                hashCode = (hashCode * 397) ^ Bottom;
                return hashCode;
            }
        }

        public bool Equals(Thickness other)
        {
            return Left == other.Left && Right == other.Right && Top == other.Top && Bottom == other.Bottom;
        }

        public static Thickness Parse(string value)
        {
            var ints = value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            switch (ints.Length)
            {
                case 1:
                    return new Thickness(ints[0]);
                case 2:
                    return new Thickness(ints[0], ints[1]);
                case 4:
                    return new Thickness(ints[0], ints[1], ints[2], ints[3]);
                default:
                    throw new FormatException($"Invalid thickness {value}");
            }
        }

        public override string ToString()
        {
            if (Left == Right && Top == Bottom)
                return Left == Top ? $"{Left}" : $"{Left} {Top}";

            return $"{Left}, {Right}, {Top}, {Bottom}";
        }
    }
}