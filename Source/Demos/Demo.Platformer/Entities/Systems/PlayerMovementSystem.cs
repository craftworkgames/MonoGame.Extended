using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace Demo.Platformer.Entities.Systems
{
    public class PlayerMovementSystem : ComponentSystem
    {
        private const float _walkSpeed = 220f;
        private const float _jumpSpeed = 425f;
        private KeyboardState _previousKeyboardState;
        private float _jumpDelay = 1.0f;

        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var entity = GetEntity(Entities.Player);
            var collisionComponent = entity.GetComponent<BasicCollisionBody>();
            var sprite = entity.GetComponent<Sprite>();
            var velocity = new Vector2(0, collisionComponent.Velocity.Y);

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                sprite.Effect = SpriteEffects.FlipHorizontally;
                velocity += new Vector2(-_walkSpeed, 0);
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                sprite.Effect = SpriteEffects.None;
                velocity += new Vector2(_walkSpeed, 0);
            }

            if (!collisionComponent.IsOnGround)
                _jumpDelay -= deltaTime * 3f;
            else
                _jumpDelay = 1.0f;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                velocity = new Vector2(velocity.X, -_jumpSpeed * _jumpDelay);
            else if (_previousKeyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                velocity = new Vector2(velocity.X, velocity.Y * 0.2f);

            collisionComponent.Velocity = velocity;
            _previousKeyboardState = keyboardState;
        }
    }
}