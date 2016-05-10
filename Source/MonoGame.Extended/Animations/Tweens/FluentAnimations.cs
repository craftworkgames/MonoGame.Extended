using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public static class FluentAnimations
    {
        internal static AnimationComponent AnimationComponent { get; set; }

        //public static TweenAnimation<T> CreateTweenChain<T>(this T target)
        //    where T : class
        //{
        //    var tween = new TweenChain<T>(target);
        //    AnimationComponent.Animations.Add(tween);
        //    return tween;
        //}

        public static TweenAnimation<T> CreateTween<T>(this T target, Action onComplete = null)
            where T : class 
        {
            var tween = new TweenAnimation<T>(target, onComplete);
            AnimationComponent.Animations.Add(tween);
            return tween;
        }

        public static TweenAnimation<T> Chain<T>(this TweenAnimation<T> tweenAnimation)
        {
            return tweenAnimation;
        }
        //public static TweenAnimation<T> CreateTween<T>(this TweenAnimation<T> tweenChain)
        //    where T : class
        //{
        //    var tween = new TweenGroup<T>(tweenChain.Target, onComplete);
        //    tweenChain.Tweens.Add(tween);
        //    return tween;
        //}

        public static TweenAnimation<T> MoveTo<T>(this TweenAnimation<T> tweenAnimation, Vector2 position, float duration, EasingFunction easingFunction)
            where T : IMovable
        {
            var movable = tweenAnimation.Target;
            tweenAnimation.Tweens.Add(new PropertyTween<Vector2>(() => movable.Position, v => movable.Position = v, position, duration, easingFunction));
            return tweenAnimation;
        }

        public static TweenAnimation<T> MoveBy<T>(this TweenAnimation<T> tweenAnimation, Vector2 direction, float duration, EasingFunction easingFunction)
            where T : IMovable
        {
            var movable = tweenAnimation.Target;
            return MoveTo(tweenAnimation, movable.Position + direction, duration, easingFunction);
        }
        
        public static TweenAnimation<T> RotateTo<T>(this TweenAnimation<T> tweenAnimation, float radians, float duration, EasingFunction easingFunction)
            where T : IRotatable
        {
            var rotatable = tweenAnimation.Target;
            tweenAnimation.Tweens.Add(new PropertyTween<float>(() => rotatable.Rotation, v => rotatable.Rotation = v, radians, duration, easingFunction));
            return tweenAnimation;
        }

        public static TweenAnimation<T> RotateBy<T>(this TweenAnimation<T> tweenAnimation, float radians, float duration, EasingFunction easingFunction)
            where T : IRotatable
        {
            var rotatable = tweenAnimation.Target;
            return RotateTo(tweenAnimation, rotatable.Rotation + radians, duration, easingFunction);
        }

        public static TweenAnimation<T> ScaleTo<T>(this TweenAnimation<T> tweenAnimation, Vector2 scale, float duration, EasingFunction easingFunction)
            where T : IScalable
        {
            var scalable = tweenAnimation.Target;
            tweenAnimation.Tweens.Add(new PropertyTween<Vector2>(() => scalable.Scale, v => scalable.Scale = v, scale, duration, easingFunction));
            return tweenAnimation;
        }

        public static TweenAnimation<T> ScaleBy<T>(this TweenAnimation<T> tweenAnimation, Vector2 scale, float duration, EasingFunction easingFunction)
            where T : IScalable
        {
            var scalable = tweenAnimation.Target;
            return ScaleTo(tweenAnimation, scalable.Scale * scale, duration, easingFunction);
        }

        public static TweenAnimation<T> Delay<T>(this TweenAnimation<T> tweenAnimation, float duration)
        {
            tweenAnimation.Tweens.Add(new DelayTween(duration));
            return tweenAnimation;
        }

        public static TweenAnimation<T> FadeTo<T>(this TweenAnimation<T> tweenAnimation, float alpha, float duration, EasingFunction easingFunction)
            where T : IColorable
        {
            var colorable = tweenAnimation.Target;
            var initialColor = colorable.Color;
            tweenAnimation.Tweens.Add(new PropertyTween<float>(() => initialColor.A / 255f, a => colorable.Color = initialColor * a, alpha, duration, easingFunction));
            return tweenAnimation;
        }
    }
}