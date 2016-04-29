using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public static class FluentAnimations
    {
        internal static IAnimationService AnimationService { get; set; }

        private static void AddTween(Animation animation)
        {
            AnimationService.Animations.Add(animation);
        }

        public static T MoveTo<T>(this T movable, Vector2 position, float duration, EasingFunction easingFunction)
            where T : IMovable
        {
            AddTween(new Tween<Vector2>(movable.Position, v => movable.Position = v, position, duration, easingFunction));
            return movable;
        }

        public static T Move<T>(this T movable, Vector2 direction, float duration, EasingFunction easingFunction)
            where T : IMovable
        {
            return MoveTo(movable, movable.Position + direction, duration, easingFunction);
        }
        
        public static T RotateTo<T>(this T rotatable, float radians, float duration, EasingFunction easingFunction)
            where T : IRotatable
        {
            AddTween(new Tween<float>(rotatable.Rotation, v => rotatable.Rotation = v, radians, duration, easingFunction));
            return rotatable;
        }

        public static T Rotate<T>(this T rotatable, float radians, float duration, EasingFunction easingFunction)
            where T : IRotatable
        {
            return RotateTo(rotatable, rotatable.Rotation + radians, duration, easingFunction);
        }

        public static T ScaleTo<T>(this T scalable, Vector2 scale, float duration, EasingFunction easingFunction)
            where T : IScalable
        {
            AddTween(new Tween<Vector2>(scalable.Scale, v => scalable.Scale = v, scale, duration, easingFunction));
            return scalable;
        }

        public static T Scale<T>(this T scalable, Vector2 scale, float duration, EasingFunction easingFunction)
            where T : IScalable
        {
            return ScaleTo(scalable, scalable.Scale * scale, duration, easingFunction);
        }
    }
}