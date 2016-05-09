using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public static class FluentAnimations
    {
        internal static AnimationComponent AnimationComponent { get; set; }

        public static TweenAnimation<T> CreateTweenChain<T>(this T target)
            where T : class
        {
            var tween = new TweenChain<T>(target);
            AnimationComponent.Animations.Add(tween);
            return tween;
        }

        public static TweenAnimation<T> CreateTween<T>(this T target)
            where T : class 
        {
            var tween = new TweenGroup<T>(target);
            AnimationComponent.Animations.Add(tween);
            return tween;
        }

        public static TweenAnimation<T> CreateTween<T>(this TweenAnimation<T> tweenChain)
            where T : class
        {
            var tween = new TweenGroup<T>(tweenChain.Target);
            tweenChain.Tweens.Add(tween);
            return tween;
        }

        public static TweenAnimation<T> MoveTo<T>(this TweenAnimation<T> tween, Vector2 position, float duration, EasingFunction easingFunction)
            where T : IMovable
        {
            var movable = tween.Target;
            tween.Tweens.Add(new PropertyTween<Vector2>(() => movable.Position, v => movable.Position = v, position, duration, easingFunction));
            return tween;
        }

        public static TweenAnimation<T> MoveBy<T>(this TweenAnimation<T> tween, Vector2 direction, float duration, EasingFunction easingFunction)
            where T : IMovable
        {
            var movable = tween.Target;
            return MoveTo(tween, movable.Position + direction, duration, easingFunction);
        }
        
        public static TweenAnimation<T> RotateTo<T>(this TweenAnimation<T> tween, float radians, float duration, EasingFunction easingFunction)
            where T : IRotatable
        {
            var rotatable = tween.Target;
            tween.Tweens.Add(new PropertyTween<float>(() => rotatable.Rotation, v => rotatable.Rotation = v, radians, duration, easingFunction));
            return tween;
        }

        public static TweenAnimation<T> RotateBy<T>(this TweenAnimation<T> tween, float radians, float duration, EasingFunction easingFunction)
            where T : IRotatable
        {
            var rotatable = tween.Target;
            return RotateTo(tween, rotatable.Rotation + radians, duration, easingFunction);
        }

        public static TweenAnimation<T> ScaleTo<T>(this TweenAnimation<T> tween, Vector2 scale, float duration, EasingFunction easingFunction)
            where T : IScalable
        {
            var scalable = tween.Target;
            tween.Tweens.Add(new PropertyTween<Vector2>(() => scalable.Scale, v => scalable.Scale = v, scale, duration, easingFunction));
            return tween;
        }

        public static TweenAnimation<T> ScaleBy<T>(this TweenAnimation<T> tween, Vector2 scale, float duration, EasingFunction easingFunction)
            where T : IScalable
        {
            var scalable = tween.Target;
            return ScaleTo(tween, scalable.Scale * scale, duration, easingFunction);
        }

        public static TweenAnimation<T> Delay<T>(this TweenAnimation<T> tween, float duration)
        {
            tween.Tweens.Add(new DelayTween(duration));
            return tween;
        }

        public static TweenAnimation<T> FadeTo<T>(this TweenAnimation<T> tween, float alpha, float duration, EasingFunction easingFunction)
            where T : IColorable
        {
            var colorable = tween.Target;
            var initialColor = colorable.Color;
            tween.Tweens.Add(new PropertyTween<float>(() => initialColor.A / 255f, a => colorable.Color = initialColor * a, alpha, duration, easingFunction));
            return tween;
        }

        public static TweenAnimation<T> OnComplete<T>(this TweenAnimation<T> tween, Action action)
        {
            tween.OnCompleteAction = action;
            return tween;
        } 
    }
}