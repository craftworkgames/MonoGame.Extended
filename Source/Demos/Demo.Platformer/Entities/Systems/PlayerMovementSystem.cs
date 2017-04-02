using System;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Systems
{
    [Aspect(AspectType.All, typeof(PlayerComponent), /*typeof(AnimationComponent),*/ typeof(CollisionBodyComponent), typeof(SpriteComponent))]
    [EntitySystem(GameLoopType.Update, Layer = 0)]
    public class PlayerMovementSystem : EntityProcessingSystem
    {
        private KeyboardState _previousKeyboardState;
        private KeyboardState _keyboardState;

        protected override void Begin(GameTime gameTime)
        {
            base.Begin(gameTime);

            _previousKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
            var sprite = entity.Get<SpriteComponent>();
            //var animation = entity.Get<AnimationComponent>();

            var physics = entity.Get<CollisionBodyComponent>();
            var player = entity.Get<PlayerComponent>();
            var velocity = new Vector2(0, physics.Velocity.Y);

            if (_keyboardState.IsKeyDown(Keys.Left) || _keyboardState.IsKeyDown(Keys.A))
            {
                sprite.Effects = SpriteEffects.FlipHorizontally;
                //animation.Play("walk");
                velocity.X -= player.WalkSpeed;
            }

            if (_keyboardState.IsKeyDown(Keys.Right) || _keyboardState.IsKeyDown(Keys.D))
            {
                sprite.Effects = SpriteEffects.None;
                //animation.Play("walk");
                velocity.X += player.WalkSpeed;
            }

            if (_keyboardState.IsKeyDown(Keys.W) || _keyboardState.IsKeyDown(Keys.Up))
            {
                //if (!player.IsJumping)
                //    animation.Play("jump");

                velocity.Y = -player.JumpSpeed;
                player.IsJumping = true;
            }
            else if (_previousKeyboardState.IsKeyDown(Keys.W) || _previousKeyboardState.IsKeyDown(Keys.Up))
            {
                // when the jump button is released we kill most of the upward velocity
                velocity.Y *= 0.2f;
            }

            //if (!player.IsJumping && Math.Abs(physics.Velocity.X) < float.Epsilon)
            //    animation.Play("idle");

            physics.Velocity = velocity;
        }
    }
}