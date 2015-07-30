using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.TileMaps
{
    public class TileLayer
    {
        public TileLayer(TileMap tileMap, string name, int width, int height, int[] data)
        {
            Name = name;
            Width = width;
            Height = height;
            _tileMap = tileMap;
            _data = data;
        }

        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly TileMap _tileMap;
        private readonly int[] _data;

        public void Draw(SpriteBatch spriteBatch)
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var id = _data[x + y * Width];
                    var region = _tileMap.GetTileRegion(id);

                    if (region != null)
                    {
                        var tx = x * region.Width;
                        var ty = y * region.Height;
                        spriteBatch.Draw(region, new Vector2(tx, ty), Color.White);
                    }
                }
            }
        }
    }
}