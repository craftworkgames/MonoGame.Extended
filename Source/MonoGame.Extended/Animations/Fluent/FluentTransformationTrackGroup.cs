using System.Collections.Generic;
using MonoGame.Extended.Animations.Tracks;
using MonoGame.Extended.Animations.Transformations;

namespace MonoGame.Extended.Animations.Fluent
{
    public class FluentTransformationTrackGroup<T> : IFluentAnimation<T> where T : class
    {
        public T Transformable;
        private Animation _animation;
        public Animation Animation
        {
            get
            {
                if (_animation == null) {
                    _animation = new Animation("");
                    _animation.AddAnimationTrackGroup(TrackGroup);
                }
                return _animation;
            }
        }
        private AnimationTrackGroup<T> _trackGroup;
        public AnimationTrackGroup<T> TrackGroup
        {
            get {
                if (_trackGroup == null) _trackGroup = new AnimationTrackGroup<T>(Transformable);
                _trackGroup.Add(_tweens.ToArray());
                _trackGroup.Add(_sets.ToArray());
                _tweens.Clear();
                _sets.Clear();
                return _trackGroup;
            }
        }
        public void AddTween(ITweenTransform<T> transform) { _tweens.Add(transform); }
        public void AddSet(ISetTransform<T> transform) { _sets.Add(transform); }
        private readonly List<ITweenTransform<T>> _tweens = new List<ITweenTransform<T>>();
        private readonly List<ISetTransform<T>> _sets = new List<ISetTransform<T>>();
    }
}