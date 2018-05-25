using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using Platformer.Components;

namespace Platformer.Systems
{
    [Aspect(AspectType.All, typeof(VelocityComponent), typeof(PlayerComponent))]
    [EntitySystem(GameLoopType.Update, Layer = 0)]
    public class PlayerSystem : EntityProcessingSystem
    {
        private readonly KeyboardInputService _inputService;

        public PlayerSystem(KeyboardInputService inputService)
        {
            _inputService = inputService;
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
            var movement = entity.Get<VelocityComponent>();

            movement.Velocity = new Vector2(0, movement.Velocity.Y);

            if (_inputService.IsKeyDown(Keys.Left) || _inputService.IsKeyDown(Keys.A))
                movement.Velocity = new Vector2(-260, movement.Velocity.Y);

            if (_inputService.IsKeyDown(Keys.Right) || _inputService.IsKeyDown(Keys.D))
                movement.Velocity = new Vector2(260, movement.Velocity.Y);

            if (_inputService.IsKeyTapped(Keys.Up) || _inputService.IsKeyTapped(Keys.W))
                movement.Velocity = new Vector2(movement.Velocity.X, -600);
        }
    }
}