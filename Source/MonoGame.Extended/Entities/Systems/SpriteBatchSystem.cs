using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Entities.Systems
{
    public class SpriteBatchSystem : EntitySystem
    {
        private readonly Camera2D _camera;
        private readonly SpriteBatch _spriteBatch;

        public SpriteBatchSystem(SpriteBatch spriteBatch, Camera2D camera)
        {
            _camera = camera;
            _spriteBatch = spriteBatch;
        }

        public Effect Effect { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public SamplerState SamplerState { get; set; }
        public BlendState BlendState { get; set; }
        public SpriteSortMode SortMode { get; set; } = SpriteSortMode.Deferred;

        public override void Draw(Entity entity, GameTime gameTime)
        {
            var transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect,
                transformMatrix);

            foreach (Sprite sprite in entity.GetComponents<Sprite>())
                _spriteBatch.Draw(sprite);

            foreach (ParticleEmitter particleEmitter in entity.GetComponents<ParticleEmitter>())
                _spriteBatch.Draw(particleEmitter);

            _spriteBatch.End();
        }
    }
}