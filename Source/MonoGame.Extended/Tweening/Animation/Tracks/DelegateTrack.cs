using System;

namespace MonoGame.Extended.Tweening.Animation.Tracks
{
    public class DelegateTrack<TTransformable, TValue> : Track<TTransformable, TValue>
        where TTransformable : class
    {
        private readonly Action<TTransformable, TValue> _setter;
        public DelegateTrack(Action<TTransformable, TValue> setter) {
            _setter = setter;
        }
        protected override void Set(TValue value) => _setter(Transformable, value);
    }
}