using MonoGame.Extended.Tweening.Easing;

namespace MonoGame.Extended.Tweening.Animation
{
    public class Transformation<T>
    {
        public T Value { get; set; }
        public double Time { get; set; }
        public EasingFunction Easing { get; set; }
        public bool Tween { get; set; }

        public override string ToString() {
            return $"{typeof(T).Name}: {Value} - {Time} -" + (Tween ? $"Tween - {Easing}" : "Set");
        }

        public virtual Transformation<T> Copy() {
            return new Transformation<T> {
                Value = Value,
                Easing = Easing,
                Time = Time,
                Tween = Tween,
            };
        }
    }
}