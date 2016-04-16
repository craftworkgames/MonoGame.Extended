using System;

namespace MonoGame.Extended.Animations.Animatable
{
    public class DelegateProperty<T>
    {
        public Func<T> Getter { get; set; }
        public Action<T> Setter { get; set; }

        public DelegateProperty(Func<T> getter, Action<T> setter) {
            Getter = getter;
            Setter = setter;
        }
    }
}