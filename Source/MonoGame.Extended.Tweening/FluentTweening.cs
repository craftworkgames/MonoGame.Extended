using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening
{
    public static class FluentTweening
    {
        internal static TweeningComponent TweeningComponent { get; set; }

        public static TweenGroup<T> CreateTweenGroup<T>(this T target, Action onCompleteAction = null,
            bool disposeOnComplete = true)
            where T : class
        {
            var tweenGroup = new TweenGroup<T>(target, onCompleteAction, disposeOnComplete);
            TweeningComponent.AddTween(tweenGroup);
            return tweenGroup;
        }

        public static TweenChain<T> CreateTweenChain<T>(this T target, Action onCompleteAction = null,
            bool disposeOnComplete = true)
        {
            var tweenGroup = new TweenChain<T>(target, onCompleteAction, disposeOnComplete);
            TweeningComponent.AddTween(tweenGroup);
            return tweenGroup;
        }

        public static TweenAnimation<T> MoveTo<T>(this TweenAnimation<T> tweenAnimation, Vector2 position,
            float duration, EasingFunction easingFunction)
            where T : IMovable
        {
            var movable = tweenAnimation.Target;
            tweenAnimation.Tweens.Add(new PropertyTween<Vector2>(() => movable.Position, v => movable.Position = v,
                position, duration, easingFunction));
            return tweenAnimation;
        }

        public static TweenAnimation<T> Move<T>(this TweenAnimation<T> tweenAnimation, Vector2 direction, float duration,
            EasingFunction easingFunction)
            where T : IMovable
        {
            var movable = tweenAnimation.Target;
            return MoveTo(tweenAnimation, movable.Position + direction, duration, easingFunction);
        }

        public static TweenAnimation<T> RotateTo<T>(this TweenAnimation<T> tweenAnimation, float radians, float duration,
            EasingFunction easingFunction)
            where T : IRotatable
        {
            var rotatable = tweenAnimation.Target;
            tweenAnimation.Tweens.Add(new PropertyTween<float>(() => rotatable.Rotation, v => rotatable.Rotation = v,
                radians, duration, easingFunction));
            return tweenAnimation;
        }

        public static TweenAnimation<T> Rotate<T>(this TweenAnimation<T> tweenAnimation, float radians, float duration,
            EasingFunction easingFunction)
            where T : IRotatable
        {
            var rotatable = tweenAnimation.Target;
            return RotateTo(tweenAnimation, rotatable.Rotation + radians, duration, easingFunction);
        }

        public static TweenAnimation<T> ScaleTo<T>(this TweenAnimation<T> tweenAnimation, Vector2 scale, float duration,
            EasingFunction easingFunction)
            where T : IScalable
        {
            var scalable = tweenAnimation.Target;
            tweenAnimation.Tweens.Add(new PropertyTween<Vector2>(() => scalable.Scale, v => scalable.Scale = v, scale,
                duration, easingFunction));
            return tweenAnimation;
        }

        public static TweenAnimation<T> Scale<T>(this TweenAnimation<T> tweenAnimation, Vector2 scale, float duration,
            EasingFunction easingFunction)
            where T : IScalable
        {
            var scalable = tweenAnimation.Target;
            return ScaleTo(tweenAnimation, scalable.Scale*scale, duration, easingFunction);
        }

        public static TweenAnimation<T> Delay<T>(this TweenAnimation<T> tweenAnimation, float duration)
        {
            tweenAnimation.Tweens.Add(new DelayTween(duration));
            return tweenAnimation;
        }

        public static TweenAnimation<T> FadeTo<T>(this TweenAnimation<T> tweenAnimation, float alpha, float duration,
            EasingFunction easingFunction)
            where T : IColorable
        {
            var colorable = tweenAnimation.Target;
            var initialColor = colorable.Color;
            tweenAnimation.Tweens.Add(new PropertyTween<float>(() => initialColor.A/255f,
                a => colorable.Color = initialColor*a, alpha, duration, easingFunction));
            return tweenAnimation;
        }
    }
}