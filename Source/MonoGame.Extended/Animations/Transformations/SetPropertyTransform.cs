using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MonoGame.Extended.Animations.Transformations
{
    //uses reflection to set a property at a certain time of a transformable
    public class SetPropertyTransform<TTransformable, TValue> : ISetTransform<TTransformable> where TTransformable : class
    {
        private readonly PropertyInfo _propertyInfo;
        public SetPropertyTransform(Expression<Func<TTransformable, TValue>> propertySelector, TValue value, double time) {
            var member = propertySelector.Body as MemberExpression;
            _propertyInfo = member?.Member as PropertyInfo;
            Value = value;
            ValueType = typeof(TValue);
            Time = time;
        }
        public object ValueObject => Value;
        public Type ValueType { get; }
        public double Time { get; set; }
        public void Set(TTransformable transformable) {
            if (!_propertyInfo.GetValue(transformable).Equals(Value))
                _propertyInfo.SetValue(transformable, Value);
        }

        public TValue Value { get; set; }
        public override string ToString() => $"{typeof(TTransformable)}-Transform: {Time}ms, {Value}";

    }
}