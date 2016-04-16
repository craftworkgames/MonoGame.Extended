using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Easing
{
    public sealed class CubicBezierEasing : EasingFunction
    {
        private object _curve;

        /// <summary>
        /// Creates a cubic bezier object used for easing.
        /// First point = 0,0 and Fourth point = 1,1.
        /// </summary>
        /// <param name="pos2">Second bezier control point</param>
        /// <param name="pos3">Third bezier control point</param>
        public CubicBezierEasing(Vector2 pos2, Vector2 pos3) {
            _curve = null; //TODO make a C. bézier curve with startpoint (0,0) and endpoint (1,1)
        }
        public CubicBezierEasing(float x1, float y1, float x2, float y2) : this(new Vector2(x1, y1), new Vector2(x2, y2)) { }
        protected override double Function(double t) {
            return 1; //TODO evaluate curve
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