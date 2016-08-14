using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Line3F : IEquatable<Line3F>
    {
        public Vector3 FirstPoint;
        public Vector3 SecondPoint;

        public Vector3 Vector
        {
            get { return SecondPoint - FirstPoint; }
        }

        public Line3F(Vector3 firstPoint, Vector3 secondPoint)
        {
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
        }

        public static bool operator ==(Line3F firstLine, Line3F secondLine)
        {
            return firstLine.Equals(secondLine);
        }

        public static bool operator !=(Line3F firstLine, Line3F secondLine)
        {
            return !(firstLine == secondLine);
        }

        public bool Equals(Line3F other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return FirstPoint == other.FirstPoint && SecondPoint == other.SecondPoint;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Line3F))
                return false;
            var other = (Line3F)obj;
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
