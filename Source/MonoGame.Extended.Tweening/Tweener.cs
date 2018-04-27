using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
        }

        private readonly List<Tween> _activeTweens = new List<Tween>();

        public Tween TweenTo<TTarget, TMember>(TTarget target, Expression<Func<TTarget, TMember>> expression, TMember toValue, float duration, float delay = 0f)
            where TTarget : class
            where TMember : struct
        {
            var memberExpression = (MemberExpression)expression.Body;
            var memberInfo = memberExpression.Member;
            var member = GetMember(target, memberInfo.Name, toValue);
            var activeTween = FindTween(target, member.Name);

            activeTween?.Cancel();

            var tween = new Tween(target, duration, delay, member);
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
            return _activeTweens.FirstOrDefault(t => t.Target == target && t.Member.Name == memberName);
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