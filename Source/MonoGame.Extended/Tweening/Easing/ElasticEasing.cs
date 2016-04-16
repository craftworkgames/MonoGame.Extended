using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Easing
{
    public class ElasticEasing : EasingFunction
    {
        public int Oscillations { get; set; } = 3;
        public double Springiness { get; set; } = 3;

        protected override double Function(double t) {
            var e = (Math.Exp(Springiness * t) - 1) / (Math.Exp(Springiness) - 1);
            return e * Math.Sin((MathHelper.PiOver2 + MathHelper.TwoPi * Oscillations) * t);
        }
    }
}