using System.Collections.Generic;
using MonoGame.Extended.Animations.Transformations;
using MonoGame.Extended.Tweening.Easing;

namespace MonoGame.Extended.Tweening.Animation.Tracks
{
    public abstract class Track<TTransformable, TValue> : ITrack<TTransformable> where TTransformable : class
    {
        public bool Interpolate { get; set; } = true;
        protected abstract void Set(TValue value);
        public TTransformable Transformable { get; set; }
        protected readonly Easer<TValue> Easer = new Easer<TValue>();
        public EasingFunction DefaultEasing
        {
            get { return Easer.Easing; }
            set { Easer.Easing = value; }
        }
        public List<Transformation<TValue>> Transforms { get; } = new List<Transformation<TValue>>();

        //after running
        private Transformation<TValue> _previous;
        public void Update(double time) {
            Transformation<TValue> previous = null, current = null;
            var count = Transforms.Count;
            if (time <= Transforms[0].Time) {
                Set(Transforms[0].Value);
                return;
            }
            if (time >= Transforms[count - 1].Time) {
                Set(Transforms[count - 1].Value);
                return;
            }

            for (int i = 0; i < count; i++) {
                previous = Transforms[i];
                if (previous.Time < time) continue;
                current = Transforms[i + 1];
                if (current.Tween) break;
                Set(current.Value);
                return;
            }

            if (current != _previous) {
                _previous = current;
                Easer.Startvalue = current.Value;
                Easer.Endvalue = previous.Value;
            }
            var start = previous.Time;
            Easer.Ease((current.Time - start) / (time - start), current.Easing);
        }

    }
}