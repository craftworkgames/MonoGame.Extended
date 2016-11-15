using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public delegate float EasingFunction(float value);

    public static class EasingFunctions
    {
        public static float Linear(float value) => value;

        public static float CubicEaseIn(float value) => Power.EaseIn(value, 2);
        public static float CubicEaseOut(float value) => Power.EaseOut(value, 2);
        public static float CubicEaseInOut(float value) => Power.EaseInOut(value, 2);

        public static float QuadraticEaseIn(float value) => Power.EaseIn(value, 3);
        public static float QuadraticEaseOut(float value) => Power.EaseOut(value, 3);
        public static float QuadraticEaseInOut(float value) => Power.EaseInOut(value, 3);

        public static float QuarticEaseIn(float value) => Power.EaseIn(value, 4);
        public static float QuarticEaseOut(float value) => Power.EaseOut(value, 4);
        public static float QuarticEaseInOut(float value) => Power.EaseInOut(value, 4);

        public static float QuinticEaseIn(float value) => Power.EaseIn(value, 5);
        public static float QuinticEaseOut(float value) => Power.EaseOut(value, 5);
        public static float QuinticEaseInOut(float value) => Power.EaseInOut(value, 5);

        public static float SineEaseIn(float value)
            => (float) Math.Sin(value*MathHelper.PiOver2 - MathHelper.PiOver2) + 1;

        public static float SineEaseOut(float value) => (float) Math.Sin(value*MathHelper.PiOver2);

        public static float SineEaseInOut(float value)
            => (float) (Math.Sin(value*MathHelper.Pi - MathHelper.PiOver2) + 1)/2;

        public static float ExponentialIn(float value) => (float) Math.Pow(2, 10*(value - 1));
        public static float ExponentialOut(float value) => Out(value, ExponentialIn);
        public static float ExponentialInOut(float value) => InOut(value, ExponentialIn);

        public static float ElasticIn(float value)
        {
            const int oscillations = 1;
            const float springiness = 3f;
            var e = (Math.Exp(springiness*value) - 1)/(Math.Exp(springiness) - 1);
            return (float) (e*Math.Sin((MathHelper.PiOver2 + MathHelper.TwoPi*oscillations)*value));
        }

        public static float ElasticOut(float value) => Out(value, ElasticIn);
        public static float ElasticInOut(float value) => InOut(value, ElasticIn);

        public static float BackIn(float value)
        {
            const float amplitude = 1f;
            return (float) (Math.Pow(value, 3) - value*amplitude*Math.Sin(value*MathHelper.Pi));
        }

        public static float BackOut(float value) => Out(value, BackIn);
        public static float BackInOut(float value) => InOut(value, BackIn);

        public static float BounceOut(float value) => Out(value, BounceIn);
        public static float BounceInOut(float value) => InOut(value, BounceIn);

        public static float BounceIn(float value)
        {
            const float bounceConst1 = 2.75f;
            var bounceConst2 = (float) Math.Pow(bounceConst1, 2);

            value = 1 - value; //flip x-axis

            if (value < 1/bounceConst1) // big bounce
                return 1f - bounceConst2*value*value;

            if (value < 2/bounceConst1)
                return 1 - (float) (bounceConst2*Math.Pow(value - 1.5f/bounceConst1, 2) + .75);

            if (value < 2.5/bounceConst1)
                return 1 - (float) (bounceConst2*Math.Pow(value - 2.25f/bounceConst1, 2) + .9375);

            //small bounce
            return 1f - (float) (bounceConst2*Math.Pow(value - 2.625f/bounceConst1, 2) + .984375);
        }


        private static float Out(float value, EasingFunction function)
        {
            return 1 - function(1 - value);
        }

        private static float InOut(float value, EasingFunction function)
        {
            if (value < 0.5f)
                return 0.5f*function(value*2);

            return 1f - 0.5f*function(2 - value*2);
        }

        private static class Power
        {
            public static float EaseIn(double value, int power)
            {
                return (float) Math.Pow(value, power);
            }

            public static float EaseOut(double value, int power)
            {
                var sign = power%2 == 0 ? -1 : 1;
                return (float) (sign*(Math.Pow(value - 1, power) + sign));
            }

            public static float EaseInOut(double s, int power)
            {
                s *= 2;

                if (s < 1)
                    return EaseIn(s, power)/2;

                var sign = power%2 == 0 ? -1 : 1;
                return (float) (sign/2.0*(Math.Pow(s - 2, power) + sign*2));
            }
        }
    }
}