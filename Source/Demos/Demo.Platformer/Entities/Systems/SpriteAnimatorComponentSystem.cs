using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;

namespace Demo.Platformer.Entities.Systems
{
    public class SpriteAnimatorComponentSystem : UpdatableComponentSystem
    {
        public SpriteAnimatorComponentSystem()
        {
        }

        public override void Update(GameTime gameTime)
        {
            var components = GetComponents<SpriteAnimatorComponent>();

            foreach (var component in components)
                component.Animator.Update(gameTime);
        }
    }
}