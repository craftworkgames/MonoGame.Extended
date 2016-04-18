using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening.Animation.Tracks;

namespace MonoGame.Extended.Tweening.Animation.Fluent
{
    public static class TrackCreator
    {
        public static FluentTrack<T, Vector2> Move<T>(this ITypeAnimation<T> anim) where T : class, IMovable {
            var result = new MovableTrack<T>();
            anim.Tracks.Add(result);
            return new FluentTrack<T, Vector2> { Track = result, TypeAnimation = anim };
        }
        public static FluentTrack<T, float> Rotate<T>(this ITypeAnimation<T> anim) where T : class, IRotatable {
            var result = new RotatableTrack<T>();
            anim.Tracks.Add(result);
            return new FluentTrack<T, float> { Track = result, TypeAnimation = anim };
        }
        public static FluentTrack<T, Vector2> Scale<T>(this ITypeAnimation<T> anim) where T : class, IScalable {
            var result = new ScalableTrack<T>();
            anim.Tracks.Add(result);
            return new FluentTrack<T, Vector2> { Track = result, TypeAnimation = anim };
        }
        public static FluentTrack<T, TValue> Property<T, TValue>(this ITypeAnimation<T> anim,
            Expression<Func<T, TValue>> selector) where T : class, IMovable {
            var result = new ReflectionTrack<T, TValue>(selector);
            anim.Tracks.Add(result);
            return new FluentTrack<T, TValue> { Track = result, TypeAnimation = anim };
        }
        public static FluentTrack<T, TValue> Property<T, TValue>(this ITypeAnimation<T> anim,
            Action<T, TValue> setter) where T : class, IMovable {
            var result = new DelegateTrack<T, TValue>(setter);
            anim.Tracks.Add(result);
            return new FluentTrack<T, TValue> { Track = result, TypeAnimation = anim };
        }

    }
}