using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MonoGame.Extended.Animations.Transformations
{
    public abstract class TweenTransformBase<TTransformable, TValue> : ITweenTransform<TTransformable>
        where TTransformable : class
    {
        protected TweenTransformBase(double time, TValue value, Easing easing) {
            Time = time;
            Value = value;
            ValueType = typeof(TValue);
            Easing = easing ?? Easing.None;
        }

        public object ValueObject => Value;
        public Type ValueType { get; }
        public double Time { get; set; }
        public TValue Value { get; set; }
        public Easing Easing { get; set; }
        protected abstract void SetValue(double t, TTransformable transformable, TValue previous);
        public bool Update(double time, TTransformable transformable, ITweenTransform<TTransformable> previous) {
            if (time > Time) return false;

            var t = Easing.Ease(time, Time, previous.Time);
            SetValue(t, transformable, ((TweenTransformBase<TTransformable, TValue>)previous).Value);
            return true;
        }

        public override string ToString() => $"{ValueType.Name}-Transform: {Time}ms, {Value}";
    }
}