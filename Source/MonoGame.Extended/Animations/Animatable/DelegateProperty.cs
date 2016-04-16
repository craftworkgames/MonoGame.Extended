using System;

namespace MonoGame.Extended.Animations.Animatable
{
    public class DelegateProperty<TTransformable,TValue>
    {
        public Func<TTransformable, TValue> Getter { get; set; }
        public Action<TTransformable, TValue> Setter { get; set; }

        public DelegateProperty(Func<TTransformable, TValue> getter, Action<TTransformable, TValue> setter) {
            Getter = getter;
            Setter = setter;
        }
    }
}