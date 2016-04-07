using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Transformation
{
    public class PositionTransform<T> : Transform<T, Vector2> where T : class, IMovable
    {
        public PositionTransform(double time, Vector2 value, Easing easing = null)
            : base(time, value, easing) { }

        protected override void SetValue(double t, T transformable, Vector2 previous) {
            transformable.Position = (float)t * (Value - previous) + previous;
        }
    }
    public class RotationTransform<T> : Transform<T, float> where T : class, IRotatable
    {
        public RotationTransform(double time, float value, Easing easing = null)
            : base(time, value, easing) { }
        protected override void SetValue(double t, T transformable, float previous) {
            transformable.Rotation = (float)t * (Value - previous) + previous;
        }
    }
    public class ScaleTransform<T> : Transform<T, Vector2> where T : class, IScalable
    {
        public ScaleTransform(double time, Vector2 value, Easing easing = null)
            : base(time, value, easing) { }
        protected override void SetValue(double t, T transformable, Vector2 previous) {
            transformable.Scale = (float)t * (Value - previous) + previous;
        }
    }
}