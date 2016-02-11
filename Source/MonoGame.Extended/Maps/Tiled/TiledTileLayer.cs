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

        public override void Dispose()
        {
            _spriteBatch.Dispose();
        }

        public int Width { get; }
        public int Height { get; }

        private readonly TiledMap _map;
        private readonly TiledTile[] _tiles;
        private readonly SpriteBatch _spriteBatch;
        private RenderTarget2D _renderTarget;

        public IEnumerable<TiledTile> Tiles => _tiles;
        public int TileWidth => _map.TileWidth;
        public int TileHeight => _map.TileHeight;

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

        public override void Draw(Rectangle visibleRectangle)
        {
            var renderOrderFunction = GetRenderOrderFunction();
            var tileLocationFunction = GetTileLocationFunction();
            var firstCol = (int)Math.Floor(visibleRectangle.Left / (float)_map.TileWidth);
            var firstRow = (int)Math.Floor(visibleRectangle.Top / (float)_map.TileHeight);

            // +3 to cover any gaps
            var columns = Math.Min(_map.Width, visibleRectangle.Width / _map.TileWidth) + 3;
            var rows = Math.Min(_map.Height, visibleRectangle.Height / _map.TileHeight) + 3;

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            foreach (var tile in renderOrderFunction(firstCol, firstRow, firstCol + columns, firstRow + rows))
            {
                var region = tile != null ? _map.GetTileRegion(tile.Id) : null;

                if (region != null)
                {
                    var point = tileLocationFunction(tile);
                    var destinationRectangle = new Rectangle(point.X, point.Y, region.Width, region.Height);
                    _spriteBatch.Draw(region, destinationRectangle, Color.White);
                }
            }

            _spriteBatch.End();
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle visibleRectangle)
        {
            if(_renderTarget == null)
                _renderTarget = new RenderTarget2D(_spriteBatch.GraphicsDevice, Width * TileWidth, Height * TileHeight);

            using (_renderTarget.BeginDraw(_spriteBatch.GraphicsDevice, Color.Transparent))
            {
                Draw(visibleRectangle);
            }
        }

        private Func<TiledTile, Point> GetTileLocationFunction()
        {
            switch (_map.Orientation)
            {
                case TiledMapOrientation.Orthogonal:
                    return GetOrthogonalLocation;
                case TiledMapOrientation.Isometric:
                    return GetIsometricLocation;
                case TiledMapOrientation.Staggered:
                    throw new NotImplementedException("Staggered maps are not yet implemented");
                default:
                    throw new NotSupportedException($"{_map.Orientation} is not supported");
            }
        }

        private Point GetOrthogonalLocation(TiledTile tile)
        {
            var tx = tile.X * _map.TileWidth;
            var ty = tile.Y * _map.TileHeight;
            return new Point(tx, ty);
        }

        private Point GetIsometricLocation(TiledTile tile)
        {
            var halfTileWidth = _map.TileWidth / 2;
            var halfTileHeight = _map.TileHeight / 2;
            var tx = tile.X * halfTileWidth - tile.Y * halfTileWidth + _map.Width * halfTileWidth;
            var ty = tile.Y * halfTileHeight + tile.X * halfTileHeight - _map.TileWidth + _map.TileHeight;
            return new Point(tx, ty);
        }

        public TiledTile GetTile(int x, int y)
        {
            var index = x + y * Width;
            return index < 0 || index >= _tiles.Length ? null : _tiles[index];
        }

        private Func<int, int, int, int, IEnumerable<TiledTile>> GetRenderOrderFunction()
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
                default:
                    throw new NotSupportedException($"{_map.RenderOrder} is not supported");
            }
        }

        private IEnumerable<TiledTile> GetTilesRightDown(int left, int top, int right, int bottom)
        {
            for (var y = top; y < bottom; y++)
            {
                for (var x = left; x < right; x++)
                    yield return GetTile(x, y);
            }
        }

        private IEnumerable<TiledTile> GetTilesRightUp(int left, int top, int right, int bottom)
        {
            for (var y = bottom - 1; y >= top; y--)
            {
                for (var x = left; x < right; x++)
                    yield return GetTile(x, y);
            }
        }

        private IEnumerable<TiledTile> GetTilesLeftDown(int left, int top, int right, int bottom)
        {
            for (var y = top; y < bottom; y++)
            {
                for (var x = right - 1; x >= left; x--)
                    yield return GetTile(x, y);
            }
        }

        private IEnumerable<TiledTile> GetTilesLeftUp(int left, int top, int right, int bottom)
        {
            for (var y = bottom - 1; y >= top; y--)
            {
                for (var x = right - 1; x >= left; x--)
                    yield return GetTile(x, y);
            }
        }
    }
}