using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace MonoGame.Extended.Tweening
{
    public sealed class TweenFieldMember<T> : TweenMember<T>
        where T : struct 
    {
        private readonly FieldInfo _fieldInfo;

        public TweenFieldMember(object target, FieldInfo fieldInfo) 
            : base(target, CompileGetMethod(fieldInfo), CompileSetMethod(fieldInfo))
        {
            _fieldInfo = fieldInfo;
        }

        private static Func<object, object> CompileGetMethod(FieldInfo fieldInfo)
        {
            var self = Expression.Parameter(typeof(object));
            var instance = Expression.Convert(self, fieldInfo.DeclaringType);
            var field = Expression.Field(instance, fieldInfo);
            var convert = Expression.TypeAs(field, typeof(object));

            return Expression.Lambda<Func<object, object>>(convert, self).Compile();
        }

        private static Action<object, object> CompileSetMethod(FieldInfo fieldInfo)
        {
            Debug.Assert(fieldInfo.DeclaringType != null);

            var self = Expression.Parameter(typeof(object));
            var value = Expression.Parameter(typeof(object));
            var fieldExp = Expression.Field(Expression.Convert(self, fieldInfo.DeclaringType), fieldInfo);
            var assignExp = Expression.Assign(fieldExp, Expression.Convert(value, fieldInfo.FieldType));

            return Expression.Lambda<Action<object, object>>(assignExp, self, value).Compile();
        }

        public override Type Type => _fieldInfo.FieldType;
        public override string Name => _fieldInfo.Name;
    }
}