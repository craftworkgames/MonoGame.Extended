using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations
{
    public interface IAnimationService
    {
        IList<Animation> Animations { get; }
    }

    public class MoveAnimation : Animation
    {
        private readonly IMovable _movable;
        private readonly Vector2 _targetPosition;
        private readonly Vector2 _sourcePosition;
        private readonly Vector2 _increment;
        private readonly float _duration;

        public MoveAnimation(IMovable movable, Vector2 targetPosition, float duration)
        {
            _movable = movable;
            _sourcePosition = _movable.Position;
            _targetPosition = targetPosition;
            _duration = duration;

            _increment = (_targetPosition - _sourcePosition)*_duration;
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            _movable.Position += _increment*deltaTime;
        }
    }

    public static class FluentAnimations
    {
        internal static IAnimationService AnimationService { get; set; }

        public static MoveAnimation Move(this IMovable movable, Vector2 direction, float duration)
        {
            var animation = new MoveAnimation(movable, movable.Position + direction, duration);
            AnimationService.Animations.Add(animation);
            return animation;
        }
    }

    public class AnimationComponent : GameComponent, IAnimationService
    {
        public AnimationComponent(Game game)
            : base(game)
        {
            FluentAnimations.AnimationService = this;
            Animations = new List<Animation>();
        }

        public IList<Animation> Animations { get; } 

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var animation in Animations)
                animation.Update(gameTime);
        }
    }
}