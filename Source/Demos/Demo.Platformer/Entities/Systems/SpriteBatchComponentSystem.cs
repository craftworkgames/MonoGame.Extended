using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Demo.Platformer.Entities.Systems
{
    public class SpriteBatchComponentSystem : DrawableComponentSystem
    {
        public SpriteBatchComponentSystem(GraphicsDevice graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        private readonly SpriteBatch _spriteBatch;

        public override void Draw(GameTime gameTime)
        {
            var spriteComponents = GetComponents<SpriteComponent>();

            _spriteBatch.Begin();

            foreach (var sprite in spriteComponents)
            {
                if (sprite.IsVisible)
                {
                    var texture = sprite.TextureRegion.Texture;
                    var sourceRectangle = sprite.TextureRegion.Bounds;

                    _spriteBatch.Draw(texture, sprite.Position, sourceRectangle, sprite.Color * sprite.Alpha, sprite.Rotation, sprite.Origin,
                        sprite.Scale, sprite.Effect, sprite.Depth);
                }
            }
            _spriteBatch.End();
        }
    }
}