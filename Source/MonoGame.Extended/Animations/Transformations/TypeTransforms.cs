using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations.Transformations;
using MonoGame.Extended.Tweening.Easing;

namespace Monogame.Extended.Animations.Transformations
{
    //These use reflection (propertyinfo) to set values on properties of supplied tranformable
    //Vector2, Color (uses Vector4), float and double

    public class Vector2Transform<T> : TweenTransformBase<T, Vector2> where T : class
    {
        private readonly PropertyInfo _property;
        public Vector2Transform(Expression<Func<T, Vector2>> propertySelector, double time, Vector2 value, EasingFunction easing = null)
            : base(time, value, easing) {
            _property = (propertySelector.Body as MemberExpression).Member as PropertyInfo;
        }
        protected override void SetValue(double t, T transformable, Vector2 previous) {
            _property.SetValue(transformable, (float)t * (Value - previous) + previous);
        }
    }
    public class ColorTransform<T> : TweenTransformBase<T, Vector4> where T : class
    {
        private readonly PropertyInfo _property;
        public ColorTransform(Expression<Func<T, Color>> propertySelector, double time, Color value, EasingFunction easing = null)
            : base(time, value.ToVector4(), easing) {
            _property = (propertySelector.Body as MemberExpression).Member as PropertyInfo;
        }
        protected override void SetValue(double t, T transformable, Vector4 previous) {
            _property.SetValue(transformable, new Color((float)t * (Value - previous) + previous));
        }
    }
    public class FloatTransform<T> : TweenTransformBase<T, float> where T : class
    {
        private readonly PropertyInfo _property;
        public FloatTransform(Expression<Func<T, float>> propertySelector, double time, float value, EasingFunction easing = null)
            : base(time, value, easing) {
            _property = (propertySelector.Body as MemberExpression).Member as PropertyInfo;
        }
        protected override void SetValue(double t, T transformable, float previous) {
            _property.SetValue(transformable, (float)(t * (Value - previous) + previous));
        }
    }
    public class DoubleTransform<T> : TweenTransformBase<T, double> where T : class
    {
        private readonly PropertyInfo _property;
        public DoubleTransform(Expression<Func<T, double>> propertySelector, double time, double value, EasingFunction easing = null)
            : base(time, value, easing) {
            _property = (propertySelector.Body as MemberExpression).Member as PropertyInfo;
        }
        protected override void SetValue(double t, T transformable, double previous) {
            _property.SetValue(transformable, t * (Value - previous) + previous);
        }
    }



}