using System;

namespace MonoGame.Extended.Tweening.Animation.Tracks
{
    public class DelegateTrack<TTransformable, TValue> : Track<TTransformable, TValue>
        where TTransformable : class
    {
        private readonly Action<TTransformable, TValue> _setter;
        private readonly Func<TTransformable, TValue> _getter;

        public DelegateTrack(Action<TTransformable, TValue> setter, Func<TTransformable, TValue> getter) {
            _setter = setter;
            _getter = getter;
        }

        protected override void SetValue(TValue value) => 
            _setter(Transformable, value);
        protected override TValue GetValue() => 
            _getter(Transformable);
    }
}