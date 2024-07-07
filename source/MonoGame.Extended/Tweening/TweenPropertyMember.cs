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

        private static Func<object, object> CompileGetMethod(PropertyInfo propertyInfo)
        {
            var param = Expression.Parameter(typeof(object));
            var instance = Expression.Convert(param, propertyInfo.DeclaringType);
            var convert = Expression.TypeAs(Expression.Property(instance, propertyInfo), typeof(object));
            return Expression.Lambda<Func<object, object>>(convert, param).Compile();
        }

        private static Action<object, object> CompileSetMethod(PropertyInfo propertyInfo)
        {
            Debug.Assert(propertyInfo.DeclaringType != null);

            var param = Expression.Parameter(typeof(object));
            var argument = Expression.Parameter(typeof(object));
            var expression = Expression.Convert(param, propertyInfo.DeclaringType);
            var methodInfo = propertyInfo.SetMethod;
            var arguments = Expression.Convert(argument, propertyInfo.PropertyType);
            var setterCall = Expression.Call(expression, methodInfo, arguments);
            return Expression.Lambda<Action<object, object>>(setterCall, param, argument).Compile();
        }
    }
}