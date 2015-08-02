using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.TileMaps
{
    public class TileLayer
    {
        public TileLayer(TileMap tileMap, GraphicsDevice graphicsDevice, string name, int width, int height, int[] data)
        {
            Name = name;
            Width = width;
            Height = height;

            _tileMap = tileMap;
            _data = data;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly TileMap _tileMap;
        private readonly int[] _data;
        private readonly SpriteBatch _spriteBatch;

        public void Draw(Camera2D camera)
        {
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp, transformMatrix: camera.GetViewMatrix());

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var id = _data[x + y * Width];
                    var region = _tileMap.GetTileRegion(id);

                    if (region != null)
                    {
                        // not exactly sure why we need to compensate 1 pixel here. Could be a bug in MonoGame?
                        var tx = x * (region.Width - 1);
                        var ty = y * (region.Height - 1);
                        
                        _spriteBatch.Draw(region, new Rectangle(tx, ty, region.Width, region.Height), Color.White);
                    }
                }
            }

            _spriteBatch.End();
        }
    }
}