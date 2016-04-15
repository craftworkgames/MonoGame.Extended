using System;

namespace MonoGame.Extended.Interpolation.Easing
{
    public class PowerEasing : EasingFunction
    {
        public PowerEasing(int power) {
            Power = power;
        }
        public int Power { get; set; }
        protected override double Function(double t) {
            return Math.Pow(t, Power);
        }

        public static EasingFunction QuadraticEasing => new PowerEasing(2);
        public static EasingFunction CubicEasing => new PowerEasing(3);
        public static EasingFunction QuarticEasing => new PowerEasing(4);
        public static EasingFunction QuinticEasing => new PowerEasing(5);
    }
}