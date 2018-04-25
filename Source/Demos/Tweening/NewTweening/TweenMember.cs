using System;
using System.Linq.Expressions;

namespace Tweening
{
    public abstract class TweenMember
    {
        public abstract void Initialize();
        public abstract void Update(float n);
    }

    public abstract class TweenMember<T> : TweenMember
        where T : struct 
    {
        protected static Func<T, T, T> Add;
        protected static Func<T, T, T> Subtract;
        protected static Func<T, float, T> Multiply;

        static TweenMember()
        {
            var a = Expression.Parameter(typeof(T));
            var b = Expression.Parameter(typeof(T));
            var c = Expression.Parameter(typeof(float));
            Add = Expression.Lambda<Func<T, T, T>>(Expression.Add(a, b), a, b).Compile();
            Subtract = Expression.Lambda<Func<T, T, T>>(Expression.Subtract(a, b), a, b).Compile();
            Multiply = Expression.Lambda<Func<T, float, T>>(Expression.Multiply(a, c), a, c).Compile();
        }

        protected TweenMember(object target, Func<object, object> getMethod, Action<object, object> setMethod, T endValue)
        {
            Target = target;

            _endValue = endValue;
            _getMethod = getMethod;
            _setMethod = setMethod; 
        }
        
        public abstract Type Type { get; }
        public abstract string Name { get; }
        public object Target { get; }

        private readonly Func<object, object> _getMethod;
        private readonly Action<object, object> _setMethod;
        private T _startValue;
        private readonly T _endValue;
        private T _range;
        
        public override void Initialize()
        {
            _startValue = (T)_getMethod(Target);
            _range = Subtract(_endValue, _startValue);
        }

        public override void Update(float n)
        {
            var value = Add(_startValue, Multiply(_range, n));
            _setMethod(Target, value);
        }

    }
}