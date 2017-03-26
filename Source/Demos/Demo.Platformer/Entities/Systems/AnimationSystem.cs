using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using SpriteComponent = Demo.Platformer.Entities.Components.SpriteComponent;

namespace Demo.Platformer.Entities.Systems
{
    [System(
        Layer = 0,
        GameLoopType = GameLoopType.Update,
        AspectType = AspectType.RequiresAllOf,
        ComponentTypes = new[]
        {
            typeof(SpriteComponent), typeof(AnimationComponent)
        })]
    public class AnimationSystem : MonoGame.Extended.Entities.System
    {
        protected override void Process(GameTime gameTime, Entity entity)
        {
            var sprite = entity.GetComponent<SpriteComponent>();
            var animatedSprite = entity.GetComponent<AnimationComponent>();

            var animation = animatedSprite.CurrentAnimation;
            if (animation == null || animation.IsComplete)
                return;

            animation.Update(gameTime);
            sprite.Texture = animation.CurrentFrame.Texture;
            sprite.SourceRectangle = animation.CurrentFrame.Bounds;
        }
    }
}