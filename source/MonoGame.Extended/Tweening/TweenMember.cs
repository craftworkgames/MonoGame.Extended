using System;
using System.Linq.Expressions;

namespace MonoGame.Extended.Tweening
{
    public abstract class TweenMember
    {
        protected TweenMember(object target)
        {
            Target = target;
        }

        public object Target { get; }
        public abstract Type Type { get; }
        public abstract string Name { get; }
    }

    public abstract class TweenMember<T> : TweenMember
        where T : struct
    {
        protected TweenMember(object target, Func<object, object> getMethod, Action<object, object> setMethod)
            : base(target)
        {
            _getMethod = getMethod;
            _setMethod = setMethod;
        }

        private readonly Func<object, object> _getMethod;
        private readonly Action<object, object> _setMethod;

        public T Value
        {
            get { return (T) _getMethod(Target); }
            set { _setMethod(Target, value); }
        }
    }
}
