using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;

namespace MonoGame.Extended.Entities.Systems
{
    public class SpriteAnimatorSystem : EntitySystem
    {
        protected override void Update(Entity entity, GameTime gameTime)
        {
            var sprites = entity.GetComponents<SpriteComponent>();
            foreach (SpriteComponent sprite in sprites)
            {
                if (!sprite.IsAnimated)
                    continue;

                if (sprite.CurrentAnimation != null && !sprite.CurrentAnimation.IsComplete)
                {
                    sprite.CurrentAnimation.Update(gameTime);
                    sprite.TextureRegion = sprite.CurrentAnimation.CurrentFrame;
                }
            }
        }
    }
}