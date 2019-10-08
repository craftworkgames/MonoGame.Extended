using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Input;
using MonoGame.Extended.Sprites;
using Platformer.Collisions;
using Platformer.Components;

namespace Platformer.Systems
{
    public class PlayerSystem : EntityProcessingSystem
    {
        private ComponentMapper<Player> _playerMapper;
        private ComponentMapper<AnimatedSprite> _spriteMapper;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Body> _bodyMapper;

        public PlayerSystem() 
            : base(Aspect.All(typeof(Body), typeof(Player), typeof(Transform2), typeof(AnimatedSprite)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _playerMapper = mapperService.GetMapper<Player>();
            _spriteMapper = mapperService.GetMapper<AnimatedSprite>();
            _transformMapper = mapperService.GetMapper<Transform2>();
            _bodyMapper = mapperService.GetMapper<Body>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var player = _playerMapper.Get(entityId);
            var sprite = _spriteMapper.Get(entityId);
            var transform = _transformMapper.Get(entityId);
            var body = _bodyMapper.Get(entityId);
            var keyboardState = KeyboardExtended.GetState();

            if (player.CanJump)
            {
                if (keyboardState.WasKeyJustUp(Keys.Up))
                    body.Velocity.Y -= 550 + Math.Abs(body.Velocity.X) * 0.4f;

                if (keyboardState.WasKeyJustUp(Keys.Z))
                {
                    body.Velocity.Y -= 550 + Math.Abs(body.Velocity.X) * 0.4f;
                    player.State = player.State == State.Idle ? State.Punching : State.Kicking;
                }
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                body.Velocity.X += 150;
                player.Facing = Facing.Right;
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                body.Velocity.X -= 150;
                player.Facing = Facing.Left;
            }

            if (!player.IsAttacking)
            {
                if (body.Velocity.X > 0 || body.Velocity.X < 0)
                    player.State = State.Walking;

                if (body.Velocity.Y < 0)
                    player.State = State.Jumping;

                if (body.Velocity.Y > 0)
                    player.State = State.Falling;

                if (body.Velocity.EqualsWithTolerence(Vector2.Zero, 5))
                    player.State = State.Idle;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
                player.State = State.Cool;

            switch (player.State)
            {
                case State.Jumping:
                    sprite.Play("jump");
                    break;
                case State.Walking:
                    sprite.Play("walk");
                    sprite.Effect = player.Facing == Facing.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    break;
                case State.Falling:
                    sprite.Play("fall");
                    break;
                case State.Idle:
                    sprite.Play("idle");
                    break;
                case State.Kicking:
                    sprite.Play("kick", () => player.State = State.Idle);
                    break;
                case State.Punching:
                    sprite.Play("punch", () => player.State = State.Idle);
                    break;
                case State.Cool:
                    sprite.Play("cool");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            body.Velocity.X *= 0.7f;

            // TODO: Can we remove this?
            //transform.Position = body.Position;
        }
    }
}