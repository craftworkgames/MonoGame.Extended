using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sandbox.Systems
{
    public class HudSystem : IDrawSystem
    {
        private readonly BitmapFont _font;
        private readonly SpriteBatch _spriteBatch;
        private World _world;

        public HudSystem(GraphicsDevice graphicsDevice, BitmapFont font)
        {
            _font = font;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public void Dispose()
        {
        }

        public void Initialize(World world)
        {
            _world = world;
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.FillRectangle(0, 0, 800, 20, Color.Black * 0.4f);
            _spriteBatch.DrawString(_font, $"entities: {_world.EntityCount}", Vector2.One, Color.White);
            _spriteBatch.End();
        }
    }
}