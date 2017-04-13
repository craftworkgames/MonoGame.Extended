using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using SpriteComponent = Demo.Platformer.Entities.Components.SpriteComponent;
using TransformComponent = Demo.Platformer.Entities.Components.TransformComponent;

namespace Demo.Platformer.Entities.Systems
{
    [Aspect(AspectType.All, typeof(SpriteComponent), typeof(TransformComponent))]
    [EntitySystem(GameLoopType.Draw, Layer = 0)]
    public class SpriteSystem : EntityProcessingSystem
    {
        private Camera2D _camera;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixelTexture;

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

        public override void LoadContent()
        {
            base.LoadContent();

            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new [] { Color.White });
        }

        public override void UnloadContent()
        {
            _pixelTexture.Dispose();

            base.UnloadContent();
        }

        protected override void Begin(GameTime gameTime)
        {
            var transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, transformMatrix);
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
            var sprite = entity.Get<SpriteComponent>();
            if (sprite.Texture == null)
                return;
            var transform = entity.Get<TransformComponent>();

            _spriteBatch.Draw(sprite.Texture, transform.WorldPosition, sprite.SourceRectangle, sprite.Color,
                transform.WorldRotation, sprite.Origin, transform.WorldScale, sprite.Effects, sprite.Depth);
        }

        protected override void End(GameTime gameTime)
        {
            _spriteBatch.End();
        }
    }
}