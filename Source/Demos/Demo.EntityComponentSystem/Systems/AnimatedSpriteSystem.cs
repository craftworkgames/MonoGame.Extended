using Demo.EntityComponentSystem.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Demo.EntityComponentSystem.Systems
{
    [System(
        Layer = 0,
        GameLoopType = GameLoopType.Update,
        AspectType = AspectType.AllOf,
        ComponentTypes = new[]
        {
            typeof(SpriteComponent), typeof(AnimatedSpriteComponent)
        })]
    public class AnimatedSpriteSystem : EntityProcessingSystem<SpriteComponent, AnimatedSpriteComponent>
    {
        protected override void Process(GameTime gameTime, Entity entity, SpriteComponent sprite, AnimatedSpriteComponent animatedSprite)
        {
            var animation = animatedSprite.CurrentAnimation;
            if (animation == null || animation.IsComplete)
                return;

            animation.Update(gameTime);
            sprite.Texture = animation.CurrentFrame.Texture;
            sprite.SourceRectangle = animation.CurrentFrame.Bounds;
        }
    }
}