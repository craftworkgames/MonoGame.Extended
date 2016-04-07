using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Animations.Transformation
{
    public sealed class QuadraticBezierEasing : Easing
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public override string ToString() => $"Beziér easing[X1:{X1},Y1:{Y1},X2:{X2},Y2:{Y2}]";

        /// <summary>
        /// Creates a quadratic bezier object used for easing.
        /// First point = 0,0 and Fourth point = 1,1.
        /// </summary>
        /// <param name="pos2">Second bezier control point</param>
        /// <param name="pos3">Third bezier control point</param>
        public QuadraticBezierEasing(Vector2 pos2, Vector2 pos3) {
            X1 = pos2.X;
            X2 = pos3.X;
            Y1 = pos2.Y;
            Y2 = pos3.Y;
        }
        public QuadraticBezierEasing(double x1, double y1, double x2, double y2) {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
        }
        public override double Ease(double t) {
            if (X1 == Y1 && X2 == Y2) return t; //linear
            return BezierInterpolation(GetTForX(t), Y1, Y2);
        }
        private double GetTForX(double x) {
            // Newton raphson iteration
            var t = x;
            for (var i = 0; i < 4; ++i) {
                var slope = 3.0 * A(X1, X2) * t * t + 2.0 * B(X1, X2) * t + C(X1);
                if (slope == 0.0) return t;
                var currentX = BezierInterpolation(t, X1, X2) - x;
                t -= currentX / slope;
            }
            return t;
        }
        public static double BezierInterpolation(double t, double p1, double p2) {
            return ((A(p1, p2) * t + B(p1, p2)) * t + C(p1)) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double A(double a, double b) => 1.0 - 3.0 * a + 3.0 * b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double B(double a, double b) => 3.0 * a - 6.0 * b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double C(double a) => 3.0 * a;
    }
}