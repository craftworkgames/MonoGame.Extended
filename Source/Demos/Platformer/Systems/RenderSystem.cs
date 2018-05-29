using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using Platformer.Components;

namespace Platformer.Systems
{
    [Aspect(AspectType.All, typeof(SpriteComponent), typeof(Transform2))]
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
            _spriteBatch.Begin();
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
            var sprite = entity.Get<SpriteComponent>();
            var transform = entity.Get<Transform2>();

            _spriteBatch.FillRectangle(new RectangleF(transform.Position, new Size2(32f, 32f)), sprite.Color);
            //_spriteBatch.Draw(sprite.Sprite);
        }

        protected override void End(GameTime gameTime)
        {
            _spriteBatch.End();
        }
    }
}
