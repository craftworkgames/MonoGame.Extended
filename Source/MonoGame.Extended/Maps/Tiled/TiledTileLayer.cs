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
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _tiles = CreateTiles(data);
        }

        public override void Dispose()
        {
            _spriteBatch.Dispose();
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly TiledMap _map;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly TiledTile[] _tiles;
        private readonly SpriteBatch _spriteBatch;
        
        public IEnumerable<TiledTile> Tiles
        {
            get { return _tiles; }
        }

        public int TileWidth
        {
            get { return _map.TileWidth; }
        }

        public int TileHeight
        {
            get { return _map.TileHeight; }
        }

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

        public override void Draw()
        {
            var renderOrderFunction = GetRenderOrderFunction();

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);

            foreach (var tile in renderOrderFunction())
            {
                var region = _map.GetTileRegion(tile.Id);

                if (region != null)
                    RenderLayer(_map, tile, region);
            }

            _spriteBatch.End();
        }

        [Obsolete("The camera is no longer required for drawing Tiled layers")]
        public override void Draw(Camera2D camera)
        {
            Draw();
        }

        private void RenderLayer(TiledMap map, TiledTile tile, TextureRegion2D region)
        {
            switch (map.Orientation)
            {
                case TiledMapOrientation.Orthogonal:
                    RenderOrthogonal(tile,region);
                    break;
                case TiledMapOrientation.Isometric:
                    RenderIsometric(tile, region);
                    break;
                case TiledMapOrientation.Staggered:
                    throw new NotImplementedException("Staggered maps are currently not supported");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RenderOrthogonal(TiledTile tile, TextureRegion2D region)
        {
            var tx = tile.X * _map.TileWidth;
            var ty = tile.Y * _map.TileHeight;

            _spriteBatch.Draw(region, new Rectangle(tx, ty, region.Width, region.Height), Color.White);
        }

        private void RenderIsometric(TiledTile tile, TextureRegion2D region)
        {
            var halfTileWidth = _map.TileWidth / 2;
            var halfTileHeight = _map.TileHeight / 2;
            var tx = tile.X * halfTileWidth - tile.Y * halfTileWidth + _map.Width * halfTileWidth;
            var ty = tile.Y * halfTileHeight + tile.X * halfTileHeight - _map.TileWidth + _map.TileHeight;

            _spriteBatch.Draw(region, new Rectangle(tx, ty, region.Width, region.Height), Color.White);
        }

        public TiledTile GetTile(int x, int y)
        {
            return _tiles[x + y*Width];
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