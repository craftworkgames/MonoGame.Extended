using System;
using System.Linq.Expressions;

namespace MonoGame.Extended.Tweening
{
    /// <summary>
    /// Abstract base representing a property or field on a target object that a tween can animate.
    /// </summary>
    public abstract class TweenMember
    {
        /// <summary>
        /// Initializes a new instance targeting the specified object.
        /// </summary>
        /// <param name="target">The object whose member will be animated.</param>
        protected TweenMember(object target)
        {
            Target = target;
        }

        /// <summary>
        /// Gets the target object whose member is being animated.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Gets the declared type of the member being animated.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Gets the name of the member being animated.
        /// </summary>
        public abstract string Name { get; }
    }

    /// <summary>
    /// Abstract base for typed member access, using compiled get and set delegates to read and
    /// write a value of type <typeparamref name="T"/> on the target object.
    /// </summary>
    /// <typeparam name="T">The value type of the member being animated.</typeparam>
    public abstract class TweenMember<T> : TweenMember
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance with the given target object and compiled accessor delegates.
        /// </summary>
        /// <param name="target">The object whose member will be animated.</param>
        /// <param name="getMethod">A compiled delegate that reads the member value from the target.</param>
        /// <param name="setMethod">A compiled delegate that writes a value to the member on the target.</param>
        protected TweenMember(object target, Func<object, T> getMethod, Action<object, T> setMethod)
            : base(target)
        {
            _getMethod = getMethod;
            _setMethod = setMethod;
        }

        private readonly Func<object, T> _getMethod;
        private readonly Action<object, T> _setMethod;

        /// <summary>
        /// Gets or sets the current value of the member on the target object.
        /// </summary>
        public T Value
        {
            get { return _getMethod(Target); }
            set { _setMethod(Target, value); }
        }
    }
}
