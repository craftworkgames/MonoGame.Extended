using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public static class FluentAnimations
    {
        internal static AnimationComponent AnimationComponent { get; set; }

        public static ITweenAnimation<T> TweenSequence<T>(this T target)
            where T : class
        {
            var tween = new TweenSequence<T>(target);
            AnimationComponent.Animations.Add(tween);
            return tween;
        }

        public static ITweenAnimation<T> Tween<T>(this T target)
            where T : class 
        {
            var tween = new TweenAnimation<T>(target);
            AnimationComponent.Animations.Add(tween);
            return tween;
        }

        public static ITweenAnimation<T> Tween<T>(this ITweenAnimation<T> tweenSequence)
            where T : class
        {
            var tween = new TweenAnimation<T>(tweenSequence.Target);
            tweenSequence.Tweens.Add(tween);
            return tween;
        }

        public static ITweenAnimation<T> MoveTo<T>(this ITweenAnimation<T> tween, Vector2 position, float duration, EasingFunction easingFunction)
            where T : IMovable
        {
            var movable = tween.Target;
            tween.Tweens.Add(new PropertyTween<Vector2>(movable.Position, v => movable.Position = v, position, duration, easingFunction));
            return tween;
        }

        public static ITweenAnimation<T> Move<T>(this ITweenAnimation<T> tween, Vector2 direction, float duration, EasingFunction easingFunction)
            where T : IMovable
        {
            var movable = tween.Target;
            return MoveTo(tween, movable.Position + direction, duration, easingFunction);
        }
        
        public static ITweenAnimation<T> RotateTo<T>(this ITweenAnimation<T> tween, float radians, float duration, EasingFunction easingFunction)
            where T : IRotatable
        {
            var rotatable = tween.Target;
            tween.Tweens.Add(new PropertyTween<float>(rotatable.Rotation, v => rotatable.Rotation = v, radians, duration, easingFunction));
            return tween;
        }

        public static ITweenAnimation<T> Rotate<T>(this ITweenAnimation<T> tween, float radians, float duration, EasingFunction easingFunction)
            where T : IRotatable
        {
            var rotatable = tween.Target;
            return RotateTo(tween, rotatable.Rotation + radians, duration, easingFunction);
        }

        public static ITweenAnimation<T> ScaleTo<T>(this ITweenAnimation<T> tween, Vector2 scale, float duration, EasingFunction easingFunction)
            where T : IScalable
        {
            var scalable = tween.Target;
            tween.Tweens.Add(new PropertyTween<Vector2>(scalable.Scale, v => scalable.Scale = v, scale, duration, easingFunction));
            return tween;
        }

        public static ITweenAnimation<T> Scale<T>(this ITweenAnimation<T> tween, Vector2 scale, float duration, EasingFunction easingFunction)
            where T : IScalable
        {
            var scalable = tween.Target;
            return ScaleTo(tween, scalable.Scale * scale, duration, easingFunction);
        }

        public static ITweenAnimation<T> Delay<T>(this ITweenAnimation<T> tween, float duration)
        {
            tween.Tweens.Add(new DelayTween(duration));
            return tween;
        }

        public static ITweenAnimation<T> FadeTo<T>(this ITweenAnimation<T> tween, float alpha, float duration, EasingFunction easingFunction)
            where T : IColorable
        {
            var colorable = tween.Target;
            tween.Tweens.Add(new PropertyTween<float>(colorable.Color.A / 255f, a => colorable.Color = colorable.Color * a, alpha, duration, easingFunction));
            return tween;
        }
    }
}