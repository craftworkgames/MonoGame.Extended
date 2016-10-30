using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using System.Linq;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTileLayer : TiledLayer
    {
        public TiledTileLayer(TiledMap map, GraphicsDevice graphicsDevice, string name, int width, int height, int[] data)
            : base(name)
        {
            Width = width;
            Height = height;

            _renderTargetSpriteBatch = new SpriteBatch(graphicsDevice);
            _map = map;
            _tiles = CreateTiles(data, map.Tilesets);
            _animatedTiles = new List<TiledTile>();
        }

        public override void Dispose()
        {
            _renderTargetSpriteBatch.Dispose();
        }

        public int Width { get; }
        public int Height { get; }

        private readonly TiledMap _map;
        private readonly TiledTile[] _tiles;
        private readonly SpriteBatch _renderTargetSpriteBatch;
        private RenderTarget2D _renderTarget;
        private readonly List<TiledTile> _animatedTiles;
        private List<TiledTileSetTile> _uniqueTileSetTiles = new List<TiledTileSetTile>();
        private readonly Dictionary<TiledTileset, int> tileCountByTileset = new Dictionary<TiledTileset, int>();

        public VertexPositionTexture[] Vertices { get; private set; }
        public int NotBlankTilesCount { get; private set; }

        public IEnumerable<TiledTile> Tiles => _tiles;
        public int TileWidth => _map.TileWidth;
        public int TileHeight => _map.TileHeight;

        private TiledTile[] CreateTiles(int[] data, IReadOnlyList<TiledTileset> tilesets)
        {
            var tileData = new TiledTile[data.Length];
            var tiles = tilesets.Select(tileset => new List<TiledTile>()).ToList();
            var tileDataIndex = 0;

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    int tilesetIndex;

                    //find which list to add it to
                    for (tilesetIndex = tilesets.Count - 1; tilesetIndex > 0; tilesetIndex--)
                        if (tilesets[tilesetIndex].FirstId <= data[tileDataIndex])
                            break;

                    TiledTileset ts = tilesets[tilesetIndex];
                    if (!this.tileCountByTileset.ContainsKey(ts))
                    {
                        this.tileCountByTileset[ts] = 0;
                    }

                    this.tileCountByTileset[ts]++;

                    tiles.ElementAt(tilesetIndex).Add(new TiledTile(data[tileDataIndex], x, y, _map.GetTileSetTileById(data[tileDataIndex])));
                    tileDataIndex++;
                }
            }

            //Place each Tile in order of their respective Tileset for optimal drawing
            var tiledDataIndex = 0;
            foreach (var tileChunk in tiles)
                foreach (var tile in tileChunk)
                {
                    tileData[tiledDataIndex] = tile;
                    tiledDataIndex++;
                }

            NotBlankTilesCount = tileData.Count(x => x.Id != 0);
            return tileData;
        }

        internal VertexPositionTexture[] BuildVertices(float depth)
        {
            var verticesList = new List<VertexPositionTexture>();

            var vr = new Rectangle(0, 0, _map.WidthInPixels, _map.HeightInPixels);
            var firstCol = vr.Left < 0 ? 0 : (int)Math.Floor(vr.Left / (float)_map.TileWidth);
            var firstRow = vr.Top < 0 ? 0 : (int)Math.Floor(vr.Top / (float)_map.TileHeight);
            var columns = Math.Min(_map.Width, vr.Width / _map.TileWidth);
            var rows = Math.Min(_map.Height, vr.Height / _map.TileHeight);
            var renderOrderFunc = GetRenderOrderFunction();
            var tiles = renderOrderFunc(firstCol, firstRow, firstCol + columns, firstRow + rows);

            foreach (var tile in tiles)
            {
                if (tile.Id == 0)
                    continue;

                var region = _map.GetTileRegion(tile.Id);
                var point = GetTileLocationFunction()(tile);
                var tileWidth = region.Width;
                var tileHeight = region.Height;
                var tc0 = Vector2.Zero;
                var tc1 = Vector2.One;

                tc0.X = (region.X + 0.5f) / region.Texture.Width;
                tc0.Y = (region.Y + 0.5f) / region.Texture.Height;
                tc1.X = (float)(region.X + region.Width) / region.Texture.Width;
                tc1.Y = (float)(region.Y + region.Height) / region.Texture.Height;
                var vertices = new VertexPositionTexture[4];
                vertices[0] = new VertexPositionTexture(new Vector3(point.X, point.Y, depth), tc0);
                vertices[1] = new VertexPositionTexture(new Vector3(point.X + tileWidth, point.Y, depth), new Vector2(tc1.X, tc0.Y));
                vertices[2] = new VertexPositionTexture(new Vector3(point.X, point.Y + tileHeight, depth), new Vector2(tc0.X, tc1.Y));
                vertices[3] = new VertexPositionTexture(new Vector3(point.X + tileWidth, point.Y + tileHeight, depth), tc1);
                verticesList.AddRange(vertices);
            }
            Vertices = verticesList.ToArray();
            return Vertices;
        }

        public int GetTileCountForTileset(TiledTileset tileset)
        {
            if(!this.tileCountByTileset.ContainsKey(tileset))
            {
                return 0;
            }

            return this.tileCountByTileset[tileset];
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle? visibleRectangle = null, Color? backgroundColor = null, GameTime gameTime = null)
        {
            if (!IsVisible)
                return;

            var tileLocationFunction = GetTileLocationFunction();

            if (_renderTarget == null)
            {
                // create and render the entire map to a single render target.
                // this gives the best frame rate performance at the cost of memory.
                // ideally, we'd like to have a couple of different draw strategies for different situations.
                _renderTarget = new RenderTarget2D(_renderTargetSpriteBatch.GraphicsDevice, _map.WidthInPixels, _map.HeightInPixels);

                using (_renderTarget.BeginDraw(_renderTargetSpriteBatch.GraphicsDevice, backgroundColor ?? Color.Transparent))
                {
                    //var vr = visibleRectangle ?? new Rectangle(0, 0, _map.WidthInPixels, _map.HeightInPixels);
                    var vr = new Rectangle(0, 0, _map.WidthInPixels, _map.HeightInPixels);
                    var renderOrderFunction = GetRenderOrderFunction();
                    var firstCol = vr.Left < 0 ? 0 : (int)Math.Floor(vr.Left / (float)_map.TileWidth);
                    var firstRow = vr.Top < 0 ? 0 : (int)Math.Floor(vr.Top / (float)_map.TileHeight);

                    // +3 to cover any gaps
                    var columns = Math.Min(_map.Width, vr.Width / _map.TileWidth);
                    var rows = Math.Min(_map.Height, vr.Height / _map.TileHeight);

                    _renderTargetSpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

                    foreach (var tile in renderOrderFunction(firstCol, firstRow, firstCol + columns, firstRow + rows))
                    {
                        var tileId = tile?.CurrentTileId;
                        if (tile != null && tile.HasAnimation)
                        {
                            _animatedTiles.Add(tile);
                        }
                        else
                        {
                            UpdateRenderTarget(_renderTargetSpriteBatch, tileLocationFunction, tile, tileId);
                        }
                    }

                    _renderTargetSpriteBatch.End();
                }
                _uniqueTileSetTiles = _animatedTiles.Select(t => t.TileSetTile).Distinct().ToList();
                spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
            }
            else
            {
                spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
                if (gameTime == null || _animatedTiles.Count == 0) return;
                double deltaTime = gameTime.ElapsedGameTime.TotalMilliseconds;
                foreach (var tileSetTile in _uniqueTileSetTiles)
                {
                    tileSetTile.Update(deltaTime);
                }
                foreach (var animatedTile in _animatedTiles)
                {
                    UpdateRenderTarget(spriteBatch, tileLocationFunction, animatedTile, animatedTile.CurrentTileId);
                }
            }
        }

        private void UpdateRenderTarget(SpriteBatch spriteBatch, Func<TiledTile, Point> tileLocationFunction, TiledTile tile, int? tileId)
        {
            var region = tileId.HasValue ? _map.GetTileRegion(tileId.Value) : null;

            if (region != null)
            {
                var point = tileLocationFunction(tile);

                // Tiled draws tiles from the lower left of the block instead of the upper left. Adjust the Y position to account for this.
                point.Y -= region.Height - TileHeight;

                var destinationRectangle = new Rectangle(point.X, point.Y, region.Width, region.Height);
                spriteBatch.Draw(region, destinationRectangle, Color.White * Opacity);
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
                    throw new NotImplementedException(@"Staggered maps are not yet implemented");
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