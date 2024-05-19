using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening
{
    public class Tweener : IDisposable
    {
        public Tweener()
        {
        }

        public void Dispose()
        {
            CancelAll();
            _activeTweens.Clear();
            _memberCache.Clear();
        }

        public long AllocationCount { get; private set; }

        private readonly List<Tween> _activeTweens = new List<Tween>();

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

            AllocationCount++;
            var tween = (TTween)Activator.CreateInstance(typeof(TTween),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new object[]{target, duration, delay, member, toValue}, null);
            _activeTweens.Add(tween);
            return tween;
        }

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

        public Tween FindTween(object target, string memberName)
        {
            return _activeTweens.FirstOrDefault(t => t.Target == target && t.MemberName == memberName);
        }

        public void CancelAll()
        {
            foreach (var tween in _activeTweens)
                tween.Cancel();
        }

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
            AllocationCount++;

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
