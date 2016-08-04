using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ArcF : IEquatable<ArcF>
    {
        public Vector2 Centre;
        public SizeF Radius;
        public float StartAngle;
        public float EndAngle;

        public ArcF(Vector2 centre, SizeF radius, float startAngle, float endAngle)
        {
            Centre = centre;
            Radius = radius;
            StartAngle = startAngle;
            EndAngle = endAngle;
        }

        public static bool operator ==(ArcF firstArc, ArcF secondArc)
        {
            return firstArc.Equals(secondArc);
        }

        public static bool operator !=(ArcF firstArc, ArcF secondArc)
        {
            return !(firstArc == secondArc);
        }

        public bool Equals(ArcF other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return Centre == other.Centre && Radius == other.Radius && StartAngle == other.StartAngle && EndAngle == other.EndAngle;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ArcF))
                return false;
            var other = (ArcF)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return $"{{Centre={Centre}, Radius={Radius}, Angle=[{StartAngle}, {EndAngle}]}}";
        }
    }
}
