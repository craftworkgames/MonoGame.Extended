using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations.Tweens;

namespace MonoGame.Extended.Animations
{
    public interface IAnimationService
    {
        IList<Animation> Animations { get; }
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