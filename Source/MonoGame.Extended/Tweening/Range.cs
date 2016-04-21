using MonoGame.Extended.Particles;
using MonoGame.Extended.Tweening.Interpolators;

namespace MonoGame.Extended.Tweening
{
    public class Range<T>
    {
        private static readonly Interpolator<T> INTERPOLATOR;
        static Range() {
            INTERPOLATOR = InterpolatorStore.GetRegistered<T>();
        }
        public T Min { get; set; }
        public T Max { get; set; }
        public T Random() {
            if(INTERPOLATOR == null) {
                return (T)DynamicInterpolator.Singleton.Interpolate(Min, Max, FastRand.NextSingle());
            }
            return INTERPOLATOR.Interpolate(Min, Max, FastRand.NextSingle());
        }
    }
}