using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Easing
{
    public sealed class CubicBezierEasing : EasingFunction
    {
        private readonly Vector2 _controlPoint1;
        private readonly Vector2 _controlPoint2;
        private Vector2[] _points;


        /// <summary>
        /// Creates a cubic bezier object used for easing.
        /// First point = 0,0 and Fourth point = 1,1.
        /// </summary>
        /// <param name="controlPoint1">Second bezier control point</param>
        /// <param name="controlPoint2">Third bezier control point</param>
        public CubicBezierEasing(Vector2 controlPoint1, Vector2 controlPoint2, int resolution) {
            _controlPoint1 = controlPoint1;
            _controlPoint2 = controlPoint2;
            _points = new Vector2[resolution];
            for (int i = 1; i < resolution; i++) {
                _points[i - 1] = PosAt(1f * i / resolution);
            }
            _points[resolution - 1] = Vector2.One;
        }

        public CubicBezierEasing(float x1, float y1, float x2, float y2, int resolution = 19) : this(new Vector2(x1, y1), new Vector2(x2, y2), resolution) { }
        protected override double Function(double t) {
            for (int i = 0; i < _points.Length; i++) {
                if (t > _points[i].X) continue;
                var prev = i > 0 ? _points[i - 1] : Vector2.Zero;
                var current = _points[i];
                var d = (t - prev.X) / (current.X - prev.X);
                return (current.Y - prev.Y) * d + prev.Y;
            }
            return 1;
        }

        private Vector2 PosAt(float t) {
            var i = 1 - t;
            return
                3 * i * i * t * _controlPoint1 +
                3 * i * t * t * _controlPoint2 +
                t * t * t * Vector2.One;
        }

        /// <summary>
        /// Smoothly starts the transition.
        /// </summary>
        public static EasingFunction EaseIn => new CubicBezierEasing(0.42f, 0, 1, 1);

        /// <summary>
        /// Smoothly ends the transition.
        /// </summary>
        public static EasingFunction EaseOut => new CubicBezierEasing(0, 0, 0.58f, 1);

        //Smoothly starts and ends the transition.
        public static EasingFunction EaseInOut => new CubicBezierEasing(0.42f, 0, 0.58f, 1);
    }
}