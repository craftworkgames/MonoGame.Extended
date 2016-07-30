using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Line2F : IEquatable<Line2F>
    {
        public Point2F FirstPoint;
        public Point2F SecondPoint;

        public Vector2 Vector
        {
            get { return SecondPoint - FirstPoint; }
        }

        public Line2F(Point2F firstPoint, Point2F secondPoint)
        {
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
        }

        public static bool operator ==(Line2F firstLine, Line2F secondLine)
        {
            return firstLine.Equals(secondLine);
        }

        public static bool operator !=(Line2F firstLine, Line2F secondLine)
        {
            return !(firstLine == secondLine);
        }

        public bool Equals(Line2F other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return FirstPoint == other.FirstPoint && SecondPoint == other.SecondPoint;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Line2F))
                return false;
            var other = (Line2F)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return $"{{FirstPoint={FirstPoint}, SecondPoint={SecondPoint}}}";
        }
    }
}
