using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json.Schema;

namespace MonoGame.Extended.Shapes.Curves
{
    public interface IBezier
    {
        int Order { get; }
    }

    /// <summary>
    /// N-point bezier
    /// </summary>
    public class Bezier : CurveF, IBezier
    {
        private Vector2[] _controlPoints;
        public Bezier(Vector2 start, Vector2 end, params Vector2[] controlpoints) {
            _controlPoints = controlpoints;
            StartPoint = start;
            EndPoint = end;
            if (Order < 2) throw new InvalidOperationException("Any curve needs at least 2 points");
        }

        public int Order
        {
            get { return _controlPoints.Length + 2; }
            set
            {
                if (Order == value) return;
                if (Order < 2) throw new InvalidOperationException("Any curve needs at least 2 points");
                var result = new Vector2[Order];
                Array.Copy(_controlPoints, result, Math.Min(Order, _controlPoints.Length));
                _controlPoints = result;
            }
        }

        public Vector2 GetControlPoint(int index) {
            return _controlPoints[index];
        }
        public void SetControlPoint(int index, Vector2 position) {
            _controlPoints[index] = position;
        }
        
        public override float Length { get; protected set; }
        public override Vector2 GetPointOnCurve(float t) {
            return GetBezierPoint(t);
        }

        private Vector2 GetBezierPoint(double t) {
            var length = _controlPoints.Length - 1;
            var result = StartPoint * (float)Bernstein(0, length + 2, t);
            for (var i = 0; i <= length;) {
                result += _controlPoints[i] * (float)Bernstein(i++, length + 2, t);
            }
            result += EndPoint * (float)Bernstein(length + 1, length + 2, t);
            return result;
        }

        private static double Bernstein(int i, int n, double t) {
            var powers = Math.Pow(t, i) * Math.Pow(1 - t, n - i);
            long r = 1;
            long d;
            if (i > n) return 0;
            for (d = 1; d <= i; d++) {
                r *= n--;
                r /= d;
            }
            return r * powers;
        }
    }
}