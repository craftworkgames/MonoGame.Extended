using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class RandomExtensions
    {
        public static int Next(this Random random, Interval<int> interval)
        {
            return random.Next(interval.Min, interval.Max);
        }

        public static float NextSingle(this Random random, float min, float max)
        {
            return (max - min) * NextSingle(random) + min;
        }

        public static float NextSingle(this Random random, float max)
        {
            return max * NextSingle(random);
        }

        public static float NextSingle(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static float NextSingle(this Random random, Interval<float> interval)
        {
            return NextSingle(random, interval.Min, interval.Max);
        }

        public static float NextAngle(this Random random)
        {
            return NextSingle(random, -MathHelper.Pi, MathHelper.Pi);
        }

        public static void NextUnitVector(this Random random, out Vector2 vector)
        {
            var angle = NextAngle(random);
            vector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}
