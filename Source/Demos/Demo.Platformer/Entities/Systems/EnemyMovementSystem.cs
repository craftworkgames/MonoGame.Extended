using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace Demo.Platformer.Entities.Systems
{
    public class EnemyMovementSystem : ComponentSystem
    {
        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();

            foreach (var component in GetComponents<EnemyAi>())
            {
                component.Position += component.Direction * deltaTime;
                component.WalkTimeRemaining -= deltaTime;

                if (component.WalkTimeRemaining <= 0)
                {
                    var transformableComponent = component.Entity.GetComponent<TransformableComponent<Sprite>>();
                    var sprite = transformableComponent.Target;
                    sprite.Effect = sprite.Effect == SpriteEffects.None ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    component.Direction = -component.Direction;
                    component.WalkTimeRemaining = component.WalkTime;
                }
            }
        }
    }
}