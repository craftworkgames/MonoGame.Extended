using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations.Tweens;

namespace MonoGame.Extended.Animations
{
    public class AnimationComponent : GameComponent
    {
        public AnimationComponent(Game game)
            : base(game)
        {
            FluentAnimations.AnimationComponent = this;
            Animations = new List<IAnimation>();
        }

        public IList<IAnimation> Animations { get; } 

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var animation in Animations)
                animation.Update(gameTime);
        }
    }
}