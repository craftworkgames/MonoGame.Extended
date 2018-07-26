using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.TextureAtlases;

namespace JamGame.Systems
{
    public class MapRenderingSystem : DrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Tileset _tileset;
        private readonly SpriteBatch _spriteBatch;

        public MapRenderingSystem(GraphicsDevice graphicsDevice, Tileset tileset)
        {
            _graphicsDevice = graphicsDevice;
            _tileset = tileset;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.DarkBlue * 0.2f);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateScale(2));

            for (var x = 3; x < 20; x++)
            {
                for(var y = 3; y < 20; y++)
                    _spriteBatch.Draw(_tileset.GetTile(50), new Vector2(x * _tileset.TileWidth, y * _tileset.TileHeight), Color.White);
            }

            _spriteBatch.End();
        }
    }
}