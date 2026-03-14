using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualBasic;

namespace MonoGame.Extended.Tweening
{
    /// <summary>
    /// Provides access to a public field on a target object using expression-tree compiled
    /// delegates for efficient get and set operations.
    /// </summary>
    /// <typeparam name="T">The value type of the field being accessed.</typeparam>
    public sealed class TweenFieldMember<T> : TweenMember<T>
        where T : struct
    {
        private readonly FieldInfo _fieldInfo;

        /// <summary>
        /// Initializes a new instance targeting the specified field on the given object.
        /// </summary>
        /// <param name="target">The object that owns the field.</param>
        /// <param name="fieldInfo">Reflection metadata for the field to access.</param>
        public TweenFieldMember(object target, FieldInfo fieldInfo)
            : base(target, CompileGetMethod(fieldInfo), CompileSetMethod(fieldInfo))
        {
            _fieldInfo = fieldInfo;
        }

        private static Func<object, T> CompileGetMethod(FieldInfo fieldInfo)
        {
            var entityType = fieldInfo.DeclaringType!;
            var parameter = Expression.Parameter(typeof(object), "entity");
            var property = Expression.Field(Expression.Convert(parameter, entityType), fieldInfo);
            return Expression.Lambda<Func<object, T>>(property, parameter).Compile();
        }

        private static Action<object, T> CompileSetMethod(FieldInfo fieldInfo)
        {
            Debug.Assert(fieldInfo.DeclaringType != null);

            var entityType = fieldInfo.DeclaringType!;
            var targetParam = Expression.Parameter(typeof(object), "target");
            var valueParam = Expression.Parameter(typeof(T), "value");
            var conversion = Expression.Convert(targetParam, entityType);

            var field = Expression.Field(conversion, fieldInfo);
            var assignation = Expression.Assign(field, valueParam);

            return Expression.Lambda<Action<object, T>>(assignation, targetParam, valueParam).Compile();
        }

        /// <summary>
        /// Gets the declared type of the field.
        /// </summary>
        public override Type Type => _fieldInfo.FieldType;

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        public override string Name => _fieldInfo.Name;
    }
}
