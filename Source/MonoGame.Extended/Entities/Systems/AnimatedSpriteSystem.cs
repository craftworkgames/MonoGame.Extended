using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Entities.Systems
{
    public class AnimatedSpriteSystem : EntitySystem
    {
        public override void Update(Entity entity, GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            foreach (AnimatedSprite sprite in entity.GetComponents<Sprite>())
                sprite.Update(deltaTime);
        }
    }
}