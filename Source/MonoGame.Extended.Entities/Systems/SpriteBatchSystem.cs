using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Entities.Systems
{
    public class SpriteBatchSystem : ComponentSystem
    {
        private readonly Camera2D _camera;
        private readonly SpriteBatch _spriteBatch;

        public SpriteBatchSystem(GraphicsDevice graphicsDevice, Camera2D camera)
        {
            _camera = camera;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public Effect Effect { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public SamplerState SamplerState { get; set; }
        public BlendState BlendState { get; set; }
        public SpriteSortMode SortMode { get; set; } = SpriteSortMode.Deferred;

        public override void Draw(GameTime gameTime)
        {
            var spriteComponents = GetComponents<TransformableComponent<Sprite>>();
            var particleEmitterComponents = GetComponents<TransformableComponent<ParticleEmitter>>();
            var transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, transformMatrix);

            foreach (var spriteComponent in spriteComponents)
            {
                var sprite = spriteComponent.Target;
                _spriteBatch.Draw(sprite);
            }

            foreach (var particleEmitterComponent in particleEmitterComponents)
            {
                var particleEmitter = particleEmitterComponent.Target;
                _spriteBatch.Draw(particleEmitter);
            }

            _spriteBatch.End();
        }
    }
}