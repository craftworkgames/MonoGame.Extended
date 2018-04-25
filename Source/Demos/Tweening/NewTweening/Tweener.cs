using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Tweening.NewTweening
{
    public class Tweener
    {
        public Tweener()
        {
        }

        private readonly List<Tween> _activeTweens = new List<Tween>();

        public Tween Create<TTarget, TMember>(TTarget target, Expression<Func<TTarget, TMember>> expression, TMember toValue, float duration, float delay = 0f)
            where TTarget : class
            where TMember : struct
        {
            var memberExpression = (MemberExpression)expression.Body;
            var memberInfo = memberExpression.Member;
            var member = GetMember(target, memberInfo.Name, toValue);
            var tween = new Tween(target, duration, delay, new []{ member });
            _activeTweens.Add(tween);
            return tween;
        }

        public void Update(float elapsedSeconds)
        {
            foreach (var tween in _activeTweens)
                tween.Update(elapsedSeconds);
        }

        private static TweenMember<T> GetMember<T>(object target, string memberName, T toValue)
            where T : struct
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var type = target.GetType();
            var property = type.GetProperty(memberName, flags);

            if (property != null)
                return new TweenPropertyMember<T>(target, property, toValue);

            var field = type.GetField(memberName, flags);

            if (field != null)
                return new TweenFieldMember<T>(target, field, toValue);

            throw new InvalidOperationException($"'{memberName}' is not a property or field of the target");
        }
    }
}