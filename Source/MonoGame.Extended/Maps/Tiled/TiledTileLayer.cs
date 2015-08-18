using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTileLayer : TiledLayer
    {
        public TiledTileLayer(TiledMap map, GraphicsDevice graphicsDevice, string name, int width, int height, int[] data)
            : base(name)
        {
            Width = width;
            Height = height;
            
            _map = map;
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _tiles = CreateTiles(data);
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly TiledMap _map;
        private readonly TiledTile[] _tiles;
        private readonly SpriteBatch _spriteBatch;

        private TiledTile[] CreateTiles(int[] data)
        {
            var tiles = new TiledTile[data.Length];
            var index = 0;

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    tiles[x + y * Width] = new TiledTile(data[index], x, y);
                    index++;
                }
            }

            return tiles;
        }

        public override void Draw(Camera2D camera)
        {
            var renderOrderFunction = GetRenderOrderFunction();

            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp, transformMatrix: camera.GetViewMatrix());

            foreach (var tile in renderOrderFunction())
            {
                var region = _map.GetTileRegion(tile.Id);

                if (region != null)
                {
                    // not exactly sure why we need to compensate 1 pixel here. Could be a bug in MonoGame?
                    var tx = tile.X * (_map.TileWidth - 1);
                    var ty = tile.Y * (_map.TileHeight - 1);
                        
                    _spriteBatch.Draw(region, new Rectangle(tx, ty, region.Width, region.Height), Color.White);
                }
            }

            _spriteBatch.End();
        }
        
        public TiledTile GetTile(int x, int y)
        {
            return _tiles[x + y * Width];
        }

        private Func<IEnumerable<TiledTile>> GetRenderOrderFunction()
        {
            switch (_map.RenderOrder)
            {
                case TiledRenderOrder.LeftDown:
                    return GetTilesLeftDown;
                case TiledRenderOrder.LeftUp:
                    return GetTilesLeftUp;
                case TiledRenderOrder.RightDown:
                    return GetTilesRightDown;
                case TiledRenderOrder.RightUp:
                    return GetTilesRightUp;
            }

            throw new NotSupportedException(string.Format("{0} is not supported", _map.RenderOrder));
        }

        private IEnumerable<TiledTile> GetTilesRightDown()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                    yield return GetTile(x, y);
            }
        }

        private IEnumerable<TiledTile> GetTilesRightUp()
        {
            for (var y = Height - 1; y >= 0; y--)
            {
                for (var x = 0; x < Width; x++)
                    yield return GetTile(x, y);
            }
        }

        private IEnumerable<TiledTile> GetTilesLeftDown()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = Width - 1; x >= 0; x--)
                    yield return GetTile(x, y);
            }
        }
        
        private IEnumerable<TiledTile> GetTilesLeftUp()
        {
            for (var y = Height - 1; y >= 0; y--)
            {
                for (var x = Width - 1; x >= 0; x--)
                    yield return GetTile(x, y);
            }
        }
    }
}