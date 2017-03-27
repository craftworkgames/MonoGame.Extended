using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using SpriteComponent = Demo.Platformer.Entities.Components.SpriteComponent;

namespace Demo.Platformer.Entities.Systems
{
    [System(
        Layer = 0,
        GameLoopType = GameLoopType.Update,
        AspectType = AspectType.AllOf,
        ComponentTypes = new[]
        {
            typeof(SpriteComponent), typeof(AnimationComponent)
        })]
    public class AnimationSystem : EntityProcessingSystem<SpriteComponent, AnimationComponent>
    {
        protected override void Process(GameTime gameTime, Entity entity, SpriteComponent sprite, AnimationComponent animation)
        {
            var currentAnimation = animation.CurrentAnimation;
            if (currentAnimation == null || currentAnimation.IsComplete)
                return;

            currentAnimation.Update(gameTime);
            sprite.Texture = currentAnimation.CurrentFrame.Texture;
            sprite.SourceRectangle = currentAnimation.CurrentFrame.Bounds;
        }
    }
}