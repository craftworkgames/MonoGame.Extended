using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.TextureAtlases;

namespace JamGame.Systems
{
    public class MapRenderingSystem : DrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Tileset _tileset;
        private readonly SpriteBatch _spriteBatch;
        private readonly FastRandom _random  = new FastRandom();
        
        private const int _mapWidth = 30;
        private const int _mapHeight = 30;
        private readonly int[,] _mapData = new int[_mapWidth, _mapHeight];

        public MapRenderingSystem(GraphicsDevice graphicsDevice, Tileset tileset)
        {
            _graphicsDevice = graphicsDevice;
            _tileset = tileset;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            for (var x = 0; x < _mapWidth; x++)
            {
                for (var y = 0; y < _mapHeight; y++)
                    _mapData[x, y] = GetTileIndexAt(x, y);
            }
        }

        private int GetTileIndexAt(int x, int y)
        {
            if (x < 3 || x > 19)
                return -1;

            if (y < 2 || y > 19)
                return -1;

            switch (y)
            {
                case 2: return x == 3 ? 0 : (x == 19 ? 1 : 2);
                case 3: return x == 3 ? 16 : (x == 19 ? 18 : 17);
                case 4: return _random.Next(32, 34);
                default: return 50;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.DarkBlue * 0.2f);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateScale(2));

            for (var x = 0; x < _mapWidth; x++)
            {
                for (var y = 0; y < _mapHeight; y++)
                {
                    var tileIndex = _mapData[x, y];

                    if (tileIndex >= 0)
                    {
                        var tile = _tileset.GetTile(tileIndex);
                        var position = new Vector2(x * _tileset.TileWidth, y * _tileset.TileHeight);

                        _spriteBatch.Draw(tile, position, Color.White);
                    }
                }
            }

            _spriteBatch.End();
        }
    }
}