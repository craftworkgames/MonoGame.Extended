using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Tweening
{
    public static class InterpolatorStore
    {
        private static readonly Dictionary<Type, object> INTERPOLATORS;

        static InterpolatorStore() {
            INTERPOLATORS = new Dictionary<Type, object> {
                { typeof(double), new DoubleInterpolator() },
                { typeof(float), new FloatInterpolator() },
                { typeof(int), new IntInterpolator() },
                { typeof(Vector2), new Vector2Interpolator() },
                { typeof(Vector3), new Vector3Interpolator() },
                { typeof(Vector4), new Vector4Interpolator() },
                { typeof(Quaternion), new QuaternionInterpolator() },
                { typeof(Color), new ColorInterpolator() },
                { typeof(HslColor), new HslColorInterpolator() },
                { typeof(object), DynamicInterpolator },
            };
        }

        public static void Register<TValue>(Interpolator<TValue> interpolator) {
            INTERPOLATORS[typeof(TValue)] = interpolator;
        }

        public static Interpolator<TValue> GetRegistered<TValue>() {
            object result;
            if (INTERPOLATORS.TryGetValue(typeof(TValue), out result)) return (Interpolator<TValue>)result;
            return null;
        }

        public static Interpolator<object> DynamicInterpolator = Tweening.DynamicInterpolator.Singleton;
    }
}