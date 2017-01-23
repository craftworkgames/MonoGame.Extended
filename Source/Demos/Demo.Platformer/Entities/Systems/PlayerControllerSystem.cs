using System;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Entities.Components;

namespace Demo.Platformer.Entities.Systems
{
    public sealed class PlayerControllerSystem : EntitySystem
    {
        private const float _walkSpeed = 220f;
        private const float _jumpSpeed = 425f;
        private KeyboardState _previousKeyboardState;
        private float _jumpDelay = 1.0f;

        protected override void Update(Entity entity, GameTime gameTime)
        {
            if (!entity.HasComponent<Player>())
                return;

            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var body = entity.GetComponent<CollisionBody>();
            var playerState = entity.GetComponent<CharacterState>();
            var sprite = entity.GetComponent<SpriteComponent>();
            var velocity = new Vector2(0, body.Velocity.Y);

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                sprite.Effect = SpriteEffects.FlipHorizontally;
                sprite.Play("walk");
                velocity += new Vector2(-_walkSpeed, 0);
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                sprite.Effect = SpriteEffects.None;
                sprite.Play("walk");
                velocity += new Vector2(_walkSpeed, 0);
            }
            
            if (playerState.IsJumping)
                _jumpDelay -= deltaTime * 2.8f;
            else
                _jumpDelay = 1.0f;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                if(!playerState.IsJumping)
                    sprite.Play("jump");

                velocity = new Vector2(velocity.X, -_jumpSpeed * _jumpDelay);
                playerState.IsJumping = true;
            }
            else if (_previousKeyboardState.IsKeyDown(Keys.W) || _previousKeyboardState.IsKeyDown(Keys.Up))
            {
                // when the jump button is released we kill most of the upward velocity
                velocity = new Vector2(velocity.X, velocity.Y * 0.2f);
            }

            if (!playerState.IsJumping && Math.Abs(body.Velocity.X) < float.Epsilon)
                sprite.Play("idle");

            body.Velocity = velocity;
            
            _previousKeyboardState = keyboardState;
        }
    }
}