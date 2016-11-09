using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Entities.Systems
{
    public class AnimatedSpriteSystem : ComponentSystem
    {
        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var sprites = GetComponents<AnimatedSprite>();

            foreach (var animatedSprite in sprites)
                animatedSprite.Update(deltaTime);
        }
    }
}