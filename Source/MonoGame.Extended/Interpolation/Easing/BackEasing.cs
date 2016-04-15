using static System.Math;

namespace MonoGame.Extended.Interpolation.Easing
{
    public class BackEasing : EasingFunction
    {
        public double Amplitude = 1;
        protected override double Function(double t) {
            return Pow(t, 3) - t * Amplitude * Sin(t * PI);
        }
    }
}