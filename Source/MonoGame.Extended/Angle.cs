using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public enum AngleType : byte
    {
        Radian = 0,
        Degree,
        Revolution, //or Turn / cycle
        Gradian // or Gon
    }
    [DataContract]
    [DebuggerDisplay("{ToString(),nq}")]
    public struct Angle : IComparable<Angle>, IEquatable<Angle>
    {
        private const double TAU = Math.PI * 2.0;
        private const double TAU_INV = 0.5 / Math.PI;
        private const double DEGREE_RADIAN = Math.PI / 180.0;
        private const double RADIAN_DEGREE = 180.0 / Math.PI;
        private const double GRADIAN_RADIAN = Math.PI / 200.0;
        private const double RADIAN_GRADIAN = 200.0 / Math.PI;

        [DataMember]
        public double Radians { get; set; }

        public double Degrees
        {
            get { return Radians * RADIAN_DEGREE; }
            set { Radians = value * DEGREE_RADIAN; }
        }

        public double Gradians
        {
            get { return Radians * RADIAN_GRADIAN; }
            set { Radians = value * GRADIAN_RADIAN; }
        }

        public double Revolutions
        {
            get { return Radians * TAU_INV; }
            set { Radians = value * TAU; }
        }

        public Angle(double value, AngleType angleType = AngleType.Radian)
        {
            switch (angleType)
            {
                default:
                    Radians = 0d;
                    break;
                case AngleType.Radian:
                    Radians = value;
                    break;
                case AngleType.Degree:
                    Radians = value * DEGREE_RADIAN;
                    break;
                case AngleType.Revolution:
                    Radians = value * TAU;
                    break;
                case AngleType.Gradian:
                    Radians = value * GRADIAN_RADIAN;
                    break;
            }
        }

        public double GetValue(AngleType angleType)
        {
            switch (angleType)
            {
                default:
                    return 0;
                case AngleType.Radian:
                    return Radians;
                case AngleType.Degree:
                    return Degrees;
                case AngleType.Revolution:
                    return Revolutions;
                case AngleType.Gradian:
                    return Gradians;
            }
        }

        public void Wrap()
        {
            var angle = Radians % TAU;
            if (angle <= Math.PI) angle += TAU;
            if (angle > Math.PI) angle -= TAU;
            Radians = angle;
        }

        public void WrapPositive()
        {
            Radians %= TAU;
            if (Radians < 0d) Radians += TAU;
            Radians = Radians;
        }

        public static Angle FromVector(Vector2 vector)
        {
            return new Angle(Math.Atan2(-vector.Y, vector.X));
        }

        public Vector2 ToUnitVector() => ToVector(1);

        public Vector2 ToVector(float length)
        {
            return new Vector2(length * (float)Math.Cos(Radians), -length * (float)Math.Sin(Radians));
        }

        public static bool IsBetween(Angle value, Angle min, Angle end)
        {
            return end < min ?
                value >= min || value <= end :
                value >= min && value <= end;
        }

        public int CompareTo(Angle other)
        {
            WrapPositive();
            other.WrapPositive();
            return Radians.CompareTo(other.Radians);
        }

        public bool Equals(Angle other)
        {
            WrapPositive();
            other.WrapPositive();
            return Radians == other.Radians;
        }

        #region operators
        public static implicit operator double(Angle angle)
        {
            return angle.Radians;
        }
        public static implicit operator float(Angle angle)
        {
            return (float)angle.Radians;
        }
        public static explicit operator Angle(double angle)
        {
            return new Angle(angle);
        }
        public static explicit operator Angle(float angle)
        {
            return new Angle(angle);
        }
        public static Angle operator -(Angle angle)
        {
            return new Angle(-angle.Radians);
        }
        public static bool operator ==(Angle a, Angle b)
        {
            return a.Equals(a);
        }

        public static bool operator !=(Angle a, Angle b)
        {
            return !a.Equals(a);
        }

        public static Angle operator -(Angle left, Angle right)
        {
            return new Angle(left.Radians - right.Radians);
        }
        public static Angle operator *(Angle left, double right)
        {
            return new Angle(left.Radians * right);
        }
        public static Angle operator *(double left, Angle right)
        {
            return new Angle(right.Radians * left);
        }
        public static Angle operator +(Angle left, Angle right)
        {
            return new Angle(left.Radians + right.Radians);
        }
        #endregion


        public override string ToString()
        {
            return $"{Radians} Radians";
        }
    }
}