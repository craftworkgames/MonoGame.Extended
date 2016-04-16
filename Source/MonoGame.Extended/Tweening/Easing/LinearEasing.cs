namespace MonoGame.Extended.Tweening.Easing
{
    public sealed class LinearEasing : EasingFunction
    {
        protected override double Function(double t) => t;
        public override string ToString() => "No easing";
    }
}