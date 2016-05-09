using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public class TweenGroup<T> : TweenAnimation<T>
    {
        public TweenGroup(T target)
            : base(target)
        {
        } 

        public override void Update(GameTime gameTime)
        {
            if(IsComplete)
                return;

            foreach (var animation in Tweens)
                animation.Update(gameTime);

            IsComplete = Tweens.All(t => t.IsComplete);
        }
    }
}