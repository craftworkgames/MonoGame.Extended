using System;
using System.Linq.Expressions;
using System.Reflection;
using MonoGame.Extended.Animations.Transformations;

namespace Monogame.Extended.Animations.Transformations
{
    public class DynamicTransform<TTransformable,TValue> : TweenTransformBase<TTransformable,TValue> 
        where TTransformable:class
    {
        private readonly PropertyInfo _property;
        public DynamicTransform(Expression<Func<TTransformable,TValue>> propertySelector, double time, TValue value, Easing easing = null)
        : base(time,value,easing){
            _property = (propertySelector.Body as MemberExpression).Member as PropertyInfo;
        }
        protected override void SetValue(double t, TTransformable transformable, TValue previous){
            dynamic p = previous, v = Value;
            _property.SetValue(transformable,(float)t*(v-p) +p);
        }
    }    
}