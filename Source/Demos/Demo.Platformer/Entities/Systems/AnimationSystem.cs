using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using SpriteComponent = Demo.Platformer.Entities.Components.SpriteComponent;

namespace Demo.Platformer.Entities.Systems
{
    //[Aspect(AspectType.All, typeof(SpriteComponent), typeof(AnimationComponent))]
    //[EntitySystem(GameLoopType.Draw, Layer = 0)]
    //public class AnimationSystem : EntityProcessingSystem
    //{
    //    protected override void Process(GameTime gameTime, Entity entity)
    //    {
    //        var sprite = entity.Get<SpriteComponent>();
    //        var animation = entity.Get<AnimationComponent>();

    //        var currentAnimation = animation.CurrentAnimationName;
    //        if (currentAnimation == null || currentAnimation.IsComplete)
    //            return;

    //        currentAnimation.Update(gameTime);
    //        sprite.Texture = currentAnimation.CurrentFrame.Texture;
    //        sprite.SourceRectangle = currentAnimation.CurrentFrame.Bounds;
    //    }
    //}
}