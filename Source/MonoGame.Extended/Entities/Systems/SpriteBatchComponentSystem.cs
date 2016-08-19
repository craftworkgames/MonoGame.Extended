using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Entities.Systems
{
    public class SpriteBatchComponentSystem : DrawableComponentSystem
    {
        public SpriteBatchComponentSystem(GraphicsDevice graphicsDevice, Camera2D camera)
        {
            _camera = camera;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        private readonly Camera2D _camera;
        private readonly SpriteBatch _spriteBatch;

        public override void Draw(GameTime gameTime)
        {
            var sprites = GetComponents<Sprite>();
            var emitters = GetComponents<ParticleEmitter>();

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            foreach (var sprite in sprites)
                _spriteBatch.Draw(sprite);

            foreach (var particleEmitter in emitters)
                _spriteBatch.Draw(particleEmitter);

            _spriteBatch.End();
        }
    }
}