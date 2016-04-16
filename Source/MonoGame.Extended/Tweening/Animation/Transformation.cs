using MonoGame.Extended.Tweening.Easing;

namespace MonoGame.Extended.Tweening.Animation
{
    public class Transformation<T>
    {
        public T Value { get; set; }
        public double Time { get; set; }
        public EasingFunction Easing { get; set; }
        public bool Tween { get; set; }
    }
}