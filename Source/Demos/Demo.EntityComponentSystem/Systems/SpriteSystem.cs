using Demo.EntityComponentSystem.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Demo.EntityComponentSystem.Systems
{
    [System(
        Layer = 0,
        GameLoopType = GameLoopType.Draw,
        AspectType = AspectType.AllOf,
        ComponentTypes = new[]
        {
            typeof(SpriteComponent), typeof(TransformComponent)
        })]
    public class SpriteSystem : EntityProcessingSystem<SpriteComponent, TransformComponent>
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

        protected override void Process(GameTime gameTime, Entity entity, SpriteComponent sprite, TransformComponent transform)
        {
            if (sprite.Texture == null)
                return;

            _spriteBatch.Draw(sprite.Texture, transform.WorldPosition, sprite.SourceRectangle, sprite.Color,
                transform.WorldRotation, sprite.Origin, transform.WorldScale, sprite.Effects, sprite.Depth);
        }

        protected override void End(GameTime gameTime)
        {
            _spriteBatch.End();
        }
    }
}