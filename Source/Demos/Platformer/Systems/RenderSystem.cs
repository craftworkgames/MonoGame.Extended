using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Entities.Legacy;
using MonoGame.Extended.Sprites;

namespace Platformer.Systems
{
    [Aspect(AspectType.All, typeof(AnimatedSprite), typeof(Transform2))]
    [EntitySystem(GameLoopType.Draw, Layer = 0)]
    public class RenderSystem : EntityProcessingSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly OrthographicCamera _camera;

        public RenderSystem(SpriteBatch spriteBatch, OrthographicCamera camera)
        {
            _spriteBatch = spriteBatch;
            _camera = camera;
        }

        protected override void Begin(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
            var sprite = entity.Get<AnimatedSprite>();
            var transform = entity.Get<Transform2>();

            sprite.Update(gameTime.GetElapsedSeconds());
            //_spriteBatch.FillRectangle(new RectangleF(transform.Position, new Size2(32f, 32f)), sprite.Color);
            _spriteBatch.Draw(sprite, transform);
        }

        protected override void End(GameTime gameTime)
        {
            _spriteBatch.End();
        }
    }
}
