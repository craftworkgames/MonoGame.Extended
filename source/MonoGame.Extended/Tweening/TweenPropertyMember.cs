using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace MonoGame.Extended.Tweening
{
    public sealed class TweenPropertyMember<T> : TweenMember<T>
        where T : struct
    {
        private readonly PropertyInfo _propertyInfo;

        public TweenPropertyMember(object target, PropertyInfo propertyInfo)
            : base(target, CompileGetMethod(propertyInfo), CompileSetMethod(propertyInfo))
        {
            _propertyInfo = propertyInfo;
        }

        public override Type Type => _propertyInfo.PropertyType;
        public override string Name => _propertyInfo.Name;

        //TODO: Try to further optimize this. For example, it needs to do all this reflection compilation shenanigan every time.
        //Could we cache these instead?

        private static Func<object, T> CompileGetMethod(PropertyInfo propertyInfo)
        {
            var entityType = propertyInfo.DeclaringType!;
            var parameter = Expression.Parameter(typeof(object), "entity");
            var property = Expression.Property(Expression.Convert(parameter, entityType), propertyInfo);
            return Expression.Lambda<Func<object, T>>(property, parameter).Compile();
        }

        private static Action<object, T> CompileSetMethod(PropertyInfo propertyInfo)
        {
            Debug.Assert(propertyInfo.DeclaringType != null);

            var entityType = propertyInfo.DeclaringType!;
            var targetParam = Expression.Parameter(typeof(object), "target");
            var valueParam = Expression.Parameter(typeof(T), "value");
            var conversion = Expression.Convert(targetParam, entityType);
            var methodInfo = propertyInfo.SetMethod!;
            var set = Expression.Call(conversion, methodInfo, valueParam);

            return Expression.Lambda<Action<object, T>>(set, targetParam, valueParam).Compile();
        }
    }
}
