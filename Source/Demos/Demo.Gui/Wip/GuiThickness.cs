using System;

namespace Demo.Gui.Wip
{
    public struct GuiThickness : IEquatable<GuiThickness>
    {
        public GuiThickness(int all)
            : this(all, all, all, all)
        {
        }

        public GuiThickness(int leftRight, int topBottom)
            : this(leftRight, topBottom, leftRight, topBottom)
        {
        }

        public GuiThickness(int left, int top, int right, int bottom)
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

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        /// <param name="obj">The object to compare with the current instance. </param>
        public override bool Equals(object obj)
        {
            if (obj is GuiThickness)
            {
                var other = (GuiThickness) obj;
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Left;
                hashCode = (hashCode*397) ^ Top;
                hashCode = (hashCode*397) ^ Right;
                hashCode = (hashCode*397) ^ Bottom;
                return hashCode;
            }
        }

        public bool Equals(GuiThickness other)
        {
            return Left == other.Left && Right == other.Right && Top == other.Top && Bottom == other.Bottom;
        }

        public override string ToString()
        {
            return $"{Left}, {Right}, {Top}, {Bottom}";
        }
    }
}