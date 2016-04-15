using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Interpolation
{
    public static class InterpolatorStore
    {
        private static readonly Dictionary<Type, object> INTERPOLATORS;

        static InterpolatorStore() {
            INTERPOLATORS = new Dictionary<Type, object> {
                { typeof(double), new DoubleInterpolator() },
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

        public static Interpolator<object> DynamicInterpolator = Interpolation.DynamicInterpolator.Singleton;
    }
}