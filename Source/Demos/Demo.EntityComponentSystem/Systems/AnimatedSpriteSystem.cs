using Demo.EntityComponentSystem.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Demo.EntityComponentSystem.Systems
{
    [System(
        Layer = 0,
        GameLoopType = GameLoopType.Update,
        AspectType = AspectType.RequiresAllOf,
        ComponentTypes = new[]
        {
            typeof(SpriteComponent), typeof(AnimatedSpriteComponent)
        })]
    public class AnimatedSpriteSystem : MonoGame.Extended.Entities.System
    {
        protected override void Process(GameTime gameTime, Entity entity)
        {
            var sprite = entity.GetComponent<SpriteComponent>();
            var animatedSprite = entity.GetComponent<AnimatedSpriteComponent>();

            var animation = animatedSprite.CurrentAnimation;
            if (animation == null || animation.IsComplete)
                return;

            animation.Update(gameTime);
            sprite.Texture = animation.CurrentFrame.Texture;
            sprite.SourceRectangle = animation.CurrentFrame.Bounds;
        }
    }
}