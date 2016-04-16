using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MonoGame.Extended.Tweening.Animation.Tracks
{
    public class ReflectionTrack<TTransformable, TValue> : Track<TTransformable, TValue>
        where TTransformable : class
    {
        private readonly PropertyInfo _propertyInfo;

        public ReflectionTrack(Expression<Func<TTransformable, TValue>> propertySelector) {
            _propertyInfo = (propertySelector.Body as MemberExpression).Member as PropertyInfo;
        }
        protected override void Set(TValue value) => _propertyInfo.SetValue(Transformable, value);
    }
}