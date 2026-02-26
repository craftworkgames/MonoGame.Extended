using System;

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening
{
    /// <summary>
    /// Provides a collection of standard easing functions for use with animation and tweening.
    /// Each function accepts a normalized progress value in the range [0, 1] and returns a
    /// transformed value representing a non-linear rate of change.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Invert"/> and <see cref="Follow"/> to compose custom easing functions
    /// from existing ones without writing additional math.
    /// </remarks>
    public static class EasingFunctions
    {
        /// <summary>
        /// Returns a new easing function that is the inverse of the specified function,
        /// mirroring the curve so that an "In" variant becomes an "Out" variant and vice versa.
        /// </summary>
        /// <param name="easer">The easing function to invert.</param>
        /// <returns>
        /// A new easing function that applies the inverse transformation of <paramref name="easer"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="easer"/> is <c>null</c>.
        /// </exception>
        /// <example>
        /// Create a custom out variant from an existing in function:
        /// <code>
        /// Func&lt;float, float&gt; myOut = EasingFunctions.Invert(EasingFunctions.CubicIn);
        /// </code>
        /// </example>
        public static Func<float, float> Invert(Func<float, float> easer)
        {
            ArgumentNullException.ThrowIfNull(easer);
            return t => 1 - easer(1 - t);
        }

        /// <summary>
        /// Returns a new easing function that applies <paramref name="first"/> for the first half
        /// of the interpolation range and <paramref name="second"/> for the second half.
        /// This is the standard technique for composing "InOut" variants from paired "In" and "Out" functions.
        /// </summary>
        /// <param name="first">The easing function applied when the progress is in [0, 0.5].</param>
        /// <param name="second">The easing function applied when the progress is in (0.5, 1].</param>
        /// <returns>
        /// A new easing function that transitions from <paramref name="first"/> to <paramref name="second"/>
        /// at the midpoint.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="first"/> or <paramref name="second"/> is <c>null</c>.
        /// </exception>
        /// <example>
        /// Create a custom InOut variant from existing In and Out functions:
        /// <code>
        /// Func&lt;float, float&gt; myInOut = EasingFunctions.Follow(EasingFunctions.CubicIn, EasingFunctions.CubicOut);
        /// </code>
        /// </example>
        public static Func<float, float> Follow(Func<float, float> first, Func<float, float> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);
            return t => (t <= 0.5f) ? first(t * 2) / 2 : second(t * 2 - 1) / 2 + 0.5f;
        }

        /// <summary>
        /// A linear easing function with a constant rate of change and no acceleration or deceleration.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The input <paramref name="value"/> unchanged.</returns>
        public static float Linear(float value) => value;

        /// <summary>
        /// A cubic easing function that starts slow and accelerates towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float CubicIn(float value) => Power.In(value, 3);

        /// <summary>
        /// A cubic easing function that starts fast and decelerates towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float CubicOut(float value) => Power.Out(value, 3);

        /// <summary>
        /// A cubic easing function that starts slow, accelerates through the middle, and decelerates at the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float CubicInOut(float value) => Power.InOut(value, 3);

        /// <summary>
        /// A quadratic easing function that starts slow and accelerates towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float QuadraticIn(float value) => Power.In(value, 2);

        /// <summary>
        /// A quadratic easing function that starts fast and decelerates towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float QuadraticOut(float value) => Power.Out(value, 2);

        /// <summary>
        /// A quadratic easing function that starts slow, accelerates through the middle, and decelerates at the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float QuadraticInOut(float value) => Power.InOut(value, 2);

        /// <summary>
        /// A quartic easing function that starts slow and accelerates towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float QuarticIn(float value) => Power.In(value, 4);

        /// <summary>
        /// A quartic easing function that starts fast and decelerates towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float QuarticOut(float value) => Power.Out(value, 4);

        /// <summary>
        /// A quartic easing function that starts slow, accelerates through the middle, and decelerates at the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float QuarticInOut(float value) => Power.InOut(value, 4);

        /// <summary>
        /// A quintic easing function that starts slow and accelerates towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float QuinticIn(float value) => Power.In(value, 5);

        /// <summary>
        /// A quintic easing function that starts fast and decelerates towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float QuinticOut(float value) => Power.Out(value, 5);

        /// <summary>
        /// A quintic easing function that starts slow, accelerates through the middle, and decelerates at the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float QuinticInOut(float value) => Power.InOut(value, 5);

        /// <summary>
        /// A sinusoidal easing function that starts slow and accelerates towards the end,
        /// following the curve of a sine wave.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float SineIn(float value) => (float) Math.Sin(value*MathHelper.PiOver2 - MathHelper.PiOver2) + 1;

        /// <summary>
        /// A sinusoidal easing function that starts fast and decelerates towards the end,
        /// following the curve of a sine wave.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float SineOut(float value) => (float) Math.Sin(value*MathHelper.PiOver2);

        /// <summary>
        /// A sinusoidal easing function that starts slow, accelerates through the middle, and
        /// decelerates at the end, following the curve of a sine wave.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float SineInOut(float value) => (float) (Math.Sin(value*MathHelper.Pi - MathHelper.PiOver2) + 1)/2;

        /// <summary>
        /// An exponential easing function that starts very slow and accelerates sharply towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float ExponentialIn(float value) => (float) Math.Pow(2, 10*(value - 1));

        /// <summary>
        /// An exponential easing function that starts fast and decelerates sharply towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float ExponentialOut(float value) => Out(value, ExponentialIn);

        /// <summary>
        /// An exponential easing function that starts very slow, accelerates sharply through the middle,
        /// and decelerates sharply at the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float ExponentialInOut(float value) => InOut(value, ExponentialIn);

        /// <summary>
        /// A circular easing function that starts slow and accelerates towards the end,
        /// following the curve of a circle arc.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float CircleIn(float value) => (float) -(Math.Sqrt(1 - value * value) - 1);

        /// <summary>
        /// A circular easing function that starts fast and decelerates towards the end,
        /// following the curve of a circle arc.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float CircleOut(float value) => (float) Math.Sqrt(1 - (value - 1) * (value - 1));

        /// <summary>
        /// A circular easing function that starts slow, accelerates through the middle, and decelerates
        /// at the end, following the curve of a circle arc.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float CircleInOut(float value) => (float) (value <= .5 ? (Math.Sqrt(1 - value * value * 4) - 1) / -2 : (Math.Sqrt(1 - (value * 2 - 2) * (value * 2 - 2)) + 1) / 2);

        /// <summary>
        /// An elastic easing function that oscillates with a spring-like effect at the start
        /// before settling into the forward motion.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float ElasticIn(float value)
        {
            const int oscillations = 1;
            const float springiness = 3f;
            var e = (Math.Exp(springiness*value) - 1)/(Math.Exp(springiness) - 1);
            return (float) (e*Math.Sin((MathHelper.PiOver2 + MathHelper.TwoPi*oscillations)*value));
        }

        /// <summary>
        /// An elastic easing function that overshoots and oscillates with a spring-like effect
        /// at the end before settling into the final value.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float ElasticOut(float value) => Out(value, ElasticIn);

        /// <summary>
        /// An elastic easing function that oscillates with a spring-like effect at both
        /// the start and end of the motion.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float ElasticInOut(float value) => InOut(value, ElasticIn);

        /// <summary>
        /// A back easing function that slightly overshoots backward at the start
        /// before accelerating forward towards the end.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float BackIn(float value)
        {
            const float amplitude = 1f;
            return (float) (Math.Pow(value, 3) - value*amplitude*Math.Sin(value*MathHelper.Pi));
        }

        /// <summary>
        /// A back easing function that overshoots the target at the end
        /// before pulling back to settle on the final value.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float BackOut(float value) => Out(value, BackIn);

        /// <summary>
        /// A back easing function that slightly overshoots at both the start and end of the motion.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float BackInOut(float value) => InOut(value, BackIn);

        /// <summary>
        /// A bounce easing function that simulates a bouncing effect at the end,
        /// as though the value strikes a surface and bounces to rest.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float BounceOut(float value) => Out(value, BounceIn);

        /// <summary>
        /// A bounce easing function that simulates a bouncing effect at both the start and end of the motion.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
        public static float BounceInOut(float value) => InOut(value, BounceIn);

        /// <summary>
        /// A bounce easing function that simulates a bouncing effect at the start of the motion.
        /// </summary>
        /// <param name="value">The interpolation progress in the range [0, 1].</param>
        /// <returns>The eased value.</returns>
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

        private static float Out(float value, Func<float, float> function)
        {
            return 1 - function(1 - value);
        }

        private static float InOut(float value, Func<float, float> function)
        {
            if (value < 0.5f)
            {
                return 0.5f*function(value*2);
            }

            return 1f - 0.5f*function(2 - value*2);
        }

        private static class Power
        {
            public static float In(double value, int power)
            {
                return (float) Math.Pow(value, power);
            }

            public static float Out(double value, int power)
            {
                var sign = power%2 == 0 ? -1 : 1;
                return (float) (sign*(Math.Pow(value - 1, power) + sign));
            }

            public static float InOut(double s, int power)
            {
                s *= 2;

                if (s < 1)
                {
                    return In(s, power)/2;
                }

                var sign = power%2 == 0 ? -1 : 1;
                return (float) (sign/2.0*(Math.Pow(s - 2, power) + sign*2));
            }
        }
    }
}
