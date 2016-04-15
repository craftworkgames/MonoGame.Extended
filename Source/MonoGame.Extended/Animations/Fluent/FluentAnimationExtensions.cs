using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using Monogame.Extended.Animations.Transformations;
using MonoGame.Extended.Animations.Tracks;
using MonoGame.Extended.Animations.Transformations;
using MonoGame.Extended.Interpolation.Easing;

namespace MonoGame.Extended.Animations.Fluent
{
    public static class FluentAnimationExtensions
    {
        public static FluentAnimation<T> Transform<T>(this Animation animation, T transformable)
            where T : class {
            return new FluentAnimation<T> {
                Animation = animation,
                Transformable = transformable
            };
        }

        public static IFluentTweening<T> Tween<T, TValue>(this IFluentTweening<T> f, Expression<Func<T, TValue>> propertySelector, double time, TValue value, EasingFunction easing = null)
            where T : class {
            var vType = typeof(TValue);
            dynamic ps = propertySelector;
            ITweenTransform<T> result;

            if (vType == typeof(Vector2))
                result = new Vector2Transform<T>(ps, time, (Vector2)(object)value, easing);
            else if (vType == typeof(float))
                result = new FloatTransform<T>(ps, time, (float)(object)value, easing);
            else if (vType == typeof(double))
                result = new DoubleTransform<T>(ps, time, (double)(object)value, easing);
            else if (vType == typeof(Color))
                result = new ColorTransform<T>(ps, time, (Color)(object)value, easing);
            else
                result = new DynamicTransform<T, TValue>(ps, time, value, easing);

            f.Add(result);
            return f;
        }
        public static IFluentSetting<T> Set<T, TValue>(this IFluentSetting<T> f, Expression<Func<T, TValue>> propertySelector, double time, TValue value)
             where T : class {
            f.Add(new SetPropertyTransform<T, TValue>(propertySelector, time, value));
            return f;
        }

        public static IFluentTweening<T> Move<T>(this IFluentTweening<T> f, double time, Vector2 value, EasingFunction easing = null)
            where T : class, IMovable {
            f.Add(new MoveableTransform<T>(time, value, easing));
            return f;
        }
        public static IFluentTweening<T> Scale<T>(this IFluentTweening<T> f, double time, Vector2 value, EasingFunction easing = null)
            where T : class, IScalable {
            f.Add(new ScaleTransform<T>(time, value, easing));
            return f;
        }
        public static IFluentTweening<T> Rotate<T>(this IFluentTweening<T> f, double time, float value, EasingFunction easing = null)
            where T : class, IRotatable {
            f.Add(new RotationTransform<T>(time, value, easing));
            return f;
        }

        //for adding set/tween at the same time
        public static IFluentBoth<T> Set<T, TValue>(this IFluentBoth<T> f, Expression<Func<T, TValue>> propertySelector, double time, TValue value)
             where T : class {
            Set((IFluentSetting<T>)f, propertySelector, time, value);
            return f;
        }
        public static IFluentBoth<T> Tween<T, TValue>(this IFluentBoth<T> f, Expression<Func<T, TValue>> propertySelector, double time, TValue value, EasingFunction easing = null)
             where T : class {
            Tween((IFluentTweening<T>)f, propertySelector, time, value, easing);
            return f;
        }
        public static IFluentBoth<T> Move<T>(this IFluentBoth<T> f, double time, Vector2 value, EasingFunction easing = null)
             where T : class, IMovable {
            Move((IFluentTweening<T>)f, time, value, easing);
            return f;
        }
        public static IFluentBoth<T> Scale<T>(this IFluentBoth<T> f, double time, Vector2 value, EasingFunction easing = null)
             where T : class, IScalable {
            Scale((IFluentTweening<T>)f, time, value, easing);
            return f;
        }
        public static IFluentBoth<T> Rotate<T>(this IFluentBoth<T> f, double time, float value, EasingFunction easing = null)
             where T : class, IRotatable {
            Rotate((IFluentTweening<T>)f, time, value, easing);
            return f;
        }

    }
}