using Microsoft.Xna.Framework;
using static System.Math;

namespace MonoGame.Extended.Interpolation.Easing
{
    public class ElasticEasing : EasingFunction
    {
        public int Oscillations { get; set; } = 3;
        public double Springiness { get; set; } = 3;

        protected override double Function(double t) {
            var e = (Exp(Springiness * t) - 1) / (Exp(Springiness) - 1);
            return e * Sin((MathHelper.PiOver2 + MathHelper.TwoPi * Oscillations) * t);
        }
    }
}