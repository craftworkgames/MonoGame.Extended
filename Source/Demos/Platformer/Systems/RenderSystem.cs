using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;

namespace Platformer.Systems
{
    [Aspect(AspectType.All, typeof(Sprite), typeof(Transform2))]
    [EntitySystem(GameLoopType.Draw, Layer = 0)]
    public class RenderSystem : EntityProcessingSystem
    {
        private readonly SpriteBatch _spriteBatch;

        public RenderSystem(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        protected override void Begin(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
            var sprite = entity.Get<Sprite>();
            var transform = entity.Get<Transform2>();

            //_spriteBatch.FillRectangle(new RectangleF(transform.Position, new Size2(32f, 32f)), sprite.Color);
            _spriteBatch.Draw(sprite, transform);
        }

        protected override void End(GameTime gameTime)
        {
            _spriteBatch.End();
        }
    }
}
