using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Tweening.Easing;

namespace MonoGame.Extended.Tweening.Animation.Tracks
{

    public abstract class Track<TTransformable, TValue> : ITrackValue<TValue>, ITrack<TTransformable> where TTransformable : class
    {
        public bool Interpolate { get; set; } = true;
        protected abstract void Set(TValue value);
        public TTransformable Transformable { get; set; }
        public double LastEndtime => Transforms.Max(t => t.Time);
        protected readonly Easer<TValue> Easer = new Easer<TValue>();
        public EasingFunction DefaultEasing { get; set; }
        public List<Transformation<TValue>> Transforms { get; internal set; } = new List<Transformation<TValue>>();
        public void AddTransform(Transformation<TValue> transform) {
            Transforms.Add(transform);
        }

        //after running
        private Transformation<TValue> _previous;
        public virtual void Update(double time) {
            Transformation<TValue> next = null, current = null;
            var count = Transforms.Count;
            if (count < 1) return;
            if (time <= Transforms[0].Time) {
                Set(Transforms[0].Value);
                return;
            }
            if (time >= Transforms[count - 1].Time) {
                Set(Transforms[count - 1].Value);
                return;
            }

            for (int i = 0; i < count; i++) {
                next = Transforms[i];
                if (next.Time < time) continue;
                current = Transforms[i - 1];
                if (current.Tween && Interpolate) break;
                Set(current.Value);
                return;
            }

            if (current != _previous) {
                _previous = current;
                Easer.SetValues(current.Value, next.Value);
                Easer.Easing = current.Easing ?? DefaultEasing ?? EasingFunction.None;
            }
            var start = current.Time;
            var value = Easer.Ease((time - start) / (next.Time - start));
            Set(value);
        }

        public virtual ITrack<TTransformable> Copy() {
            var result = (Track<TTransformable, TValue>)MemberwiseClone();
            result.Transforms = Transforms.Select(t => t.Copy()).ToList();
            return result;
        }
    }
}