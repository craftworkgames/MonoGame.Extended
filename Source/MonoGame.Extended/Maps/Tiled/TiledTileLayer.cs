using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTileLayer : TiledLayer
    {
        private readonly TiledMap _map;

        public TiledTileLayer(TiledMap map, string name, int width, int height, int[] data)
            : base(name)
        {
            Width = width;
            Height = height;

            _map = map;
            Tiles = CreateTiles(data);
        }

        public int Width { get; }
        public int Height { get; }

        public IReadOnlyList<TiledTile> Tiles { get; }

        public int TileWidth => _map.TileWidth;
        public int TileHeight => _map.TileHeight;

        public override void Dispose()
        {
        }

        private TiledTile[] CreateTiles(int[] data)
        {
            var tiles = new TiledTile[data.Length];
            var index = 0;

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var id = data[index];
                    var tilesetTile = _map.GetTilesetTileById(id);
                    tiles[x + y*Width] = new TiledTile(id, x, y, tilesetTile);
                    index++;
                }
            }

            return tiles;
        }

        public IEnumerable<TiledTile> GetTilesInRenderOrder(bool nonBlankOnly = true)
        {
            var vr = new Rectangle(0, 0, _map.WidthInPixels, _map.HeightInPixels);
            var firstCol = vr.Left < 0 ? 0 : (int) Math.Floor(vr.Left/(float) _map.TileWidth);
            var firstRow = vr.Top < 0 ? 0 : (int) Math.Floor(vr.Top/(float) _map.TileHeight);
            var columns = Math.Min(_map.Width, vr.Width/_map.TileWidth);
            var rows = Math.Min(_map.Height, vr.Height/_map.TileHeight);

            var renderOrderFunc = GetRenderOrderFunction();
            var tiles = renderOrderFunc(firstCol, firstRow, firstCol + columns, firstRow + rows);

            return nonBlankOnly ? tiles.Where(t => t.Id != 0) : tiles;
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
                    throw new NotImplementedException(@"Staggered maps are not yet implemented");
                default:
                    throw new NotSupportedException($"{_map.Orientation} is not supported");
            }
        }

        private Point GetOrthogonalLocation(TiledTile tile)
        {
            var tx = tile.X*_map.TileWidth;
            var ty = tile.Y*_map.TileHeight;
            return new Point(tx, ty);
        }

        private Point GetIsometricLocation(TiledTile tile)
        {
            var halfTileWidth = _map.TileWidth/2;
            var halfTileHeight = _map.TileHeight/2;
            var tx = tile.X*halfTileWidth - tile.Y*halfTileWidth + _map.Width*halfTileWidth;
            var ty = tile.Y*halfTileHeight + tile.X*halfTileHeight - _map.TileWidth + _map.TileHeight;
            return new Point(tx, ty);
        }

        public TiledTile GetTile(int x, int y)
        {
            var index = x + y*Width;
            return (index < 0) || (index >= Tiles.Count) ? null : Tiles[index];
        }

        public Point GetTileLocation(TiledTile tile)
        {
            return GetTileLocationFunction()(tile);
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
                for (var x = left; x < right; x++)
                    yield return GetTile(x, y);
        }

        private IEnumerable<TiledTile> GetTilesRightUp(int left, int top, int right, int bottom)
        {
            for (var y = bottom - 1; y >= top; y--)
                for (var x = left; x < right; x++)
                    yield return GetTile(x, y);
        }

        private IEnumerable<TiledTile> GetTilesLeftDown(int left, int top, int right, int bottom)
        {
            for (var y = top; y < bottom; y++)
                for (var x = right - 1; x >= left; x--)
                    yield return GetTile(x, y);
        }

        private IEnumerable<TiledTile> GetTilesLeftUp(int left, int top, int right, int bottom)
        {
            for (var y = bottom - 1; y >= top; y--)
                for (var x = right - 1; x >= left; x--)
                    yield return GetTile(x, y);
        }
    }
}