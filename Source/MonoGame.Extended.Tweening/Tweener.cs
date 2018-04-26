using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MonoGame.Extended.Tweening
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
            for (var i = _activeTweens.Count - 1; i >= 0; i--)
            {
                var tween = _activeTweens[i];
                tween.Update(elapsedSeconds);
            }
        }

        private static TweenMember<T> GetMember<T>(object target, string memberName, T toValue)
            where T : struct
        {
            var type = target.GetType();
            var property = type.GetTypeInfo().GetDeclaredProperty(memberName);

            if (property != null)
                return new TweenPropertyMember<T>(target, property, toValue);

            var field = type.GetTypeInfo().GetDeclaredField(memberName);

            if (field != null)
                return new TweenFieldMember<T>(target, field, toValue);

            throw new InvalidOperationException($"'{memberName}' is not a property or field of the target");
        }
    }
}