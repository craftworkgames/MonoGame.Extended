using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input;
using Platformer.Collisions;
using Platformer.Components;

namespace Platformer.Systems
{
    [Aspect(AspectType.All, typeof(Body), typeof(PlayerState), typeof(Transform2))]
    [EntitySystem(GameLoopType.Update, Layer = 0)]
    public class PlayerSystem : EntityProcessingSystem
    {
        protected override void Process(GameTime gameTime, Entity entity)
        {
            var player = entity.Get<PlayerState>();
            var transform = entity.Get<Transform2>();
            var body = entity.Get<Body>();
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.IsKeyDown(Keys.Right))
                body.Velocity.X += 150;

            if (keyboardState.IsKeyDown(Keys.Left))
                body.Velocity.X -= 150;

            if (keyboardState.WasKeyJustUp(Keys.Up))
                body.Velocity.Y -= 550 + Math.Abs(body.Velocity.X) * 0.4f;

            body.Velocity.X *= 0.7f;

            // TODO: Can we remove this?
            transform.Position = body.Position;
        }
    }
}