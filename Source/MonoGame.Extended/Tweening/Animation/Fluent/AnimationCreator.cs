using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tweening.Animation.Tracks;
using MonoGame.Extended.Tweening.Easing;

namespace MonoGame.Extended.Tweening.Animation.Fluent
{
    public static class AnimationCreator
    {
        public static TypeAnimation<TTransformable> AddAnimation<TTransformable>(this IAnimation animation, TTransformable transformable) where TTransformable : class {
            var result = new TypeAnimation<TTransformable> { ParentAnimation = animation };
            result.SetTransformable(transformable);
            animation.AddChildAnimation(result);
            return result;
        }

        public static TypeAnimation<T2> SubType<T, T2>(this TypeAnimation<T> animation, Expression<Func<T, T2>> selector)
            where T : class where T2 : class {
            var sub = new TypeAnimation<T2> { ParentAnimation = animation };
            animation.AddSubTypeAnimation(selector, sub);
            return sub;
        }

        public static TTrack Set<TValue, TTrack>(this TTrack track, TValue value, double time)
            where TTrack : ITrackValue<TValue> {
            var transform = new Transformation<TValue> {
                Value = value,
                Tween = false,
                Time = time,
                Easing = null,
            };
            track.Transforms.Add(transform);
            return track;
        }

        public static TTrack Tween<TValue, TTrack>(this TTrack track, TValue value, double time,
            EasingFunction easing = null)
            where TTrack : ITrackValue<TValue> {
            var transform = new Transformation<TValue> {
                Value = value,
                Tween = true,
                Time = time,
                Easing = easing,
            };
            track.Transforms.Add(transform);
            return track;
        }

    }
}