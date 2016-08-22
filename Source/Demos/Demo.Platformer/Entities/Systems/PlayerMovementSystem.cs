using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities.Systems;

namespace Demo.Platformer.Entities.Systems
{
    public class PlayerMovementSystem : UpdatableComponentSystem
    {
        private const float _walkSpeed = 150f;
        private const float _jumpSpeed = 300f;

        public override void Update(GameTime gameTime)
        {
            var components = GetComponents<BasicCollisionComponent>();
            var keyboardState = Keyboard.GetState();

            foreach (var component in components)
            {
                var velocity = new Vector2(0, component.Velocity.Y);

                if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                    velocity += new Vector2(-_walkSpeed, 0);

                if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                    velocity += new Vector2(_walkSpeed, 0);

                if (component.IsOnGround)
                {
                    if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                        velocity += new Vector2(0, -_jumpSpeed);
                }

                component.Velocity = velocity;
            }
        }
    }
}