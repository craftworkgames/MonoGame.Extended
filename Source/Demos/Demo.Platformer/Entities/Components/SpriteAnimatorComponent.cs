using MonoGame.Extended.Animations.SpriteSheets;

namespace Demo.Platformer.Entities.Components
{
    public class SpriteAnimatorComponent : EntityComponent
    {
        public SpriteAnimatorComponent(SpriteSheetAnimationFactory animationFactory, string playAnimation = null)
        {
            Animator = new SpriteSheetAnimator(animationFactory);

            if (playAnimation != null)
                Animator.Play(playAnimation);
        }

        public SpriteSheetAnimator Animator { get; }
    }
}