using System;
using System.Linq.Expressions;
using System.Reflection;
using MonoGame.Extended.Animations.Easing;

namespace MonoGame.Extended.Animations.Transformations
{
    public abstract class TweenTransformBase<TTransformable, TValue> : ITweenTransform<TTransformable>
        where TTransformable : class
    {
        protected TweenTransformBase(double time, TValue value, EasingFunction easing) {
            Time = time;
            Value = value;
            ValueType = typeof(TValue);
            Easing = easing ?? EasingFunction.Linear;
        }

        public object ValueObject => Value;
        public Type ValueType { get; }
        public double Time { get; set; }
        /// <summary>
        /// The duration of the the interpolation (if smaller than the time between previous and this).
        /// </summary>
        public double MaxTweenDuration { get; set; } = -1;
        public TValue Value { get; set; }
        public EasingFunction Easing { get; set; }
        protected abstract void SetValue(double t, TTransformable transformable, TValue previous);
        public void Update(double time, TTransformable transformable, ITweenTransform<TTransformable> previous) {
            var start = MaxTweenDuration >= 0 
                ? Math.Max(previous.Time, Time - MaxTweenDuration) 
                : previous.Time;
            var t = Easing.Ease(time, start, Time);
            SetValue(t, transformable, ((TweenTransformBase<TTransformable, TValue>)previous).Value);
        }

        public override string ToString() => $"{ValueType.Name}-Transform: {Time}ms, {Value}";
    }
}