using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening
{
    /// <summary>
    /// Manages a collection of active tween animations and drives their updates each frame.
    /// Create animations using <see cref="TweenTo{TTarget,TMember}(TTarget,Expression{Func{TTarget,TMember}},TMember,float,float)"/>,
    /// then call <see cref="Update"/> once per frame. Completed animations are removed automatically.
    /// </summary>
    public class Tweener : IDisposable
    {
        /// <summary>
        /// Initializes a new instance with no active animations.
        /// </summary>
        public Tweener()
        {
        }

        /// <summary>
        /// Cancels all active animations, clears internal state, and releases resources.
        /// </summary>
        public void Dispose()
        {
            CancelAll();
            _activeTweens.Clear();
            _memberCache.Clear();
        }

        /// <summary>
        /// Returns a read-only view of the currently active tweens as a <see cref="ReadOnlySpan{T}"/>.
        /// This is a zero-allocation snapshot: it does not copy the list and cannot be used to modify
        /// tween state. Use <c>ActiveTweens.Length</c> to check how many tweens are running, or
        /// iterate with a <c>foreach</c> to inspect them.
        /// </summary>
        /// <example>
        /// <code>
        /// if (tweener.ActiveTweens.Length == 0)
        ///     OnAllTweensComplete();
        /// </code>
        /// </example>
        public ReadOnlySpan<Tween> ActiveTweens => CollectionsMarshal.AsSpan(_activeTweens);

        private readonly List<Tween> _activeTweens = new List<Tween>();

        /// <summary>
        /// Creates and starts an animation that interpolates the specified member of
        /// <paramref name="target"/> from its current value to <paramref name="toValue"/> over
        /// <paramref name="duration"/> seconds. If an animation is already running on the same
        /// member it is cancelled before the new one begins.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <typeparam name="TMember">The value type of the member being animated.</typeparam>
        /// <param name="target">The object whose member will be animated.</param>
        /// <param name="expression">A member-access expression identifying the property or field to animate.</param>
        /// <param name="toValue">The target value to animate towards.</param>
        /// <param name="duration">The duration of the animation in seconds.</param>
        /// <param name="delay">An optional delay before the animation begins, in seconds.</param>
        /// <returns>The created <see cref="Tween{TMember}"/> for fluent configuration.</returns>
        public Tween<TMember> TweenTo<TTarget, TMember>(TTarget target, Expression<Func<TTarget, TMember>> expression, TMember toValue, float duration, float delay = 0f)
            where TTarget : class
            where TMember : struct
        {
            switch (toValue)
            {
                case Color toValueColor:
                    return (Tween<TMember>)(object)TweenTo<TTarget, Color, ColorTween>(target, expression as Expression<Func<TTarget, Color>>, toValueColor, duration, delay);
                default:
                    return TweenTo<TTarget, TMember, LinearTween<TMember>>(target, expression, toValue, duration, delay);
            }

        }

        /// <summary>
        /// Creates and starts a tween of a specific <typeparamref name="TTween"/> type that
        /// interpolates the specified member of <paramref name="target"/> from its current value
        /// to <paramref name="toValue"/> over <paramref name="duration"/> seconds.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <typeparam name="TMember">The value type of the member being animated.</typeparam>
        /// <typeparam name="TTween">The concrete <see cref="Tween{TMember}"/> type to create.</typeparam>
        /// <param name="target">The object whose member will be animated.</param>
        /// <param name="expression">A member-access expression identifying the property or field to animate.</param>
        /// <param name="toValue">The target value to animate towards.</param>
        /// <param name="duration">The duration of the animation in seconds.</param>
        /// <param name="delay">An optional delay before the animation begins, in seconds.</param>
        /// <returns>The created <typeparamref name="TTween"/> instance for fluent configuration.</returns>
        public Tween<TMember> TweenTo<TTarget, TMember, TTween>(TTarget target, Expression<Func<TTarget, TMember>> expression, TMember toValue, float duration, float delay = 0f)
            where TTarget : class
            where TMember : struct
            where TTween : Tween<TMember>
        {
            var memberExpression = (MemberExpression)expression.Body;
            var memberInfo = memberExpression.Member;
            var member = GetMember<TMember>(target, memberInfo.Name);
            var activeTween = FindTween(target, member.Name);

            activeTween?.Cancel();

            var tween = (TTween)Activator.CreateInstance(typeof(TTween),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new object[]{target, duration, delay, member, toValue}, null);
            _activeTweens.Add(tween);
            return tween;
        }

        /// <summary>
        /// Advances all active animations by <paramref name="elapsedSeconds"/> and removes
        /// any that have completed.
        /// </summary>
        /// <param name="elapsedSeconds">The time elapsed since the last update, in seconds.</param>
        public void Update(float elapsedSeconds)
        {
            for (var i = _activeTweens.Count - 1; i >= 0; i--)
            {
                var tween = _activeTweens[i];

                tween.Update(elapsedSeconds);

                if (!tween.IsAlive)
                    _activeTweens.RemoveAt(i);
            }
        }

        /// <summary>
        /// Finds and returns an active animation targeting the specified member on the given object,
        /// or <c>null</c> if no matching animation exists.
        /// </summary>
        /// <param name="target">The object to search for.</param>
        /// <param name="memberName">The name of the member to search for.</param>
        /// <returns>The matching <see cref="Tween"/>, or <c>null</c> if not found.</returns>
        public Tween FindTween(object target, string memberName)
        {
            return _activeTweens.FirstOrDefault(t => t.Target == target && t.MemberName == memberName);
        }

        /// <summary>
        /// Cancels all active animations immediately without applying their final values.
        /// </summary>
        public void CancelAll()
        {
            foreach (var tween in _activeTweens)
                tween.Cancel();
        }

        /// <summary>
        /// Cancels all active animations, applying the final value of each before stopping.
        /// </summary>
        public void CancelAndCompleteAll()
        {
            foreach (var tween in _activeTweens)
                tween.CancelAndComplete();
        }

        private struct TweenMemberKey
        {
#pragma warning disable 414
            public object Target;
            public string MemberName;
#pragma warning restore 414
        }

        private readonly Dictionary<TweenMemberKey, TweenMember> _memberCache = new Dictionary<TweenMemberKey, TweenMember>();

        private TweenMember<T> GetMember<T>(object target, string memberName)
            where T : struct
        {
            var key = new TweenMemberKey { Target = target, MemberName = memberName };

            if (_memberCache.TryGetValue(key, out var member))
                return (TweenMember<T>) member;

            member = CreateMember<T>(target, memberName);
            _memberCache.Add(key, member);
            return (TweenMember<T>) member;
        }

        private TweenMember<T> CreateMember<T>(object target, string memberName)
            where T : struct
        {
            var type = target.GetType();
            var property = type.GetTypeInfo().GetProperty(memberName);

            if (property != null)
                return new TweenPropertyMember<T>(target, property);

            var field = type.GetTypeInfo().GetField(memberName);

            if (field != null)
                return new TweenFieldMember<T>(target, field);

            throw new InvalidOperationException($"'{memberName}' is not a property or field of the target");
        }
    }
}
