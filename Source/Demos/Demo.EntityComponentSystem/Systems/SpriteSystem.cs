using Demo.EntityComponentSystem.Components;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Demo.EntityComponentSystem.Systems
{
    [System(
        Layer = 0,
        GameLoopType = GameLoopType.Draw,
        AspectType = AspectType.RequiresAllOf,
        ComponentTypes = new[]
        {
            typeof(SpriteComponent), typeof(TransformComponent)
        })]
    public class SpriteSystem : MonoGame.Extended.Entities.System
    {
        private Camera2D _camera;
        private SpriteBatch _spriteBatch;

        public SpriteSortMode SortMode { get; set; } = SpriteSortMode.Deferred;
        public BlendState BlendState { get; set; }
        public SamplerState SamplerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public Effect Effect { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            _camera = Game.Services.GetService<Camera2D>();
            _spriteBatch = Game.Services.GetService<SpriteBatch>();
        }

        protected override void Begin(GameTime gameTime)
        {
            var transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, transformMatrix);
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
            var sprite = entity.GetComponent<SpriteComponent>();
            if (sprite.Texture == null)
                return;
            var transform = entity.GetComponent<TransformComponent>();

            _spriteBatch.Draw(sprite.Texture, transform.WorldPosition, sprite.SourceRectangle, sprite.Color,
                transform.WorldRotation, sprite.Origin, transform.WorldScale, sprite.Effects, sprite.Depth);
        }

        protected override void End(GameTime gameTime)
        {
            _spriteBatch.End();
        }
    }
}