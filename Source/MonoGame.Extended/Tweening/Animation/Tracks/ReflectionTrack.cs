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
            _propertyInfo = (propertySelector.Body as MemberExpression)?.Member as PropertyInfo;
            if (_propertyInfo == null) throw new ArgumentException("Supply a valid property selector");
        }
        protected override void SetValue(TValue value) =>
            _propertyInfo.SetValue(Transformable, value);

        protected override TValue GetValue() =>
            (TValue)_propertyInfo.GetValue(Transformable);
    }
}