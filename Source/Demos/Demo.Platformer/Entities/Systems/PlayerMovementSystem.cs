using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
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
        private Entity _playerEntity;

        protected override void OnEntityCreated(Entity entity)
        {
            if (entity.Name == Entities.Player)
                _playerEntity = entity;

            base.OnEntityCreated(entity);
        }

        protected override void OnEntityDestroyed(Entity entity)
        {
            if (entity.Name == Entities.Player)
                _playerEntity = null;

            base.OnEntityDestroyed(entity);
        }

        public override void Update(GameTime gameTime)
        {
            if(_playerEntity == null)
                return;

            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var body = _playerEntity.GetComponent<BasicCollisionBody>();
            var playerState = _playerEntity.GetComponent<PlayerState>();
            var sprite = _playerEntity.GetComponent<Sprite>();
            var velocity = new Vector2(0, body.Velocity.Y);

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

            if (playerState.IsJumping)
                _jumpDelay -= deltaTime * 2.8f;
            else
                _jumpDelay = 1.0f;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                velocity = new Vector2(velocity.X, -_jumpSpeed * _jumpDelay);
                playerState.IsJumping = true;
            }
            else if (_previousKeyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                velocity = new Vector2(velocity.X, velocity.Y * 0.2f);
            }

            body.Velocity = velocity;

            _previousKeyboardState = keyboardState;
        }
    }
}