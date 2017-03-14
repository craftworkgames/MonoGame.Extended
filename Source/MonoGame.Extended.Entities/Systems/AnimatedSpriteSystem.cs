using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Entities.Systems
{
    public class AnimatedSpriteSystem : ComponentSystem
    {
        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var sprites = GetComponents<TransformableComponent<Sprite>>();

            foreach (var animatedSprite in sprites.Select(c => c.Target).OfType<AnimatedSprite>())
                animatedSprite.Update(deltaTime);
        }
    }
}