using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Maps.Renderers
{
    public class FullMapRenderer : IMapRenderer
    {
        private const int _indexesPerTile = 6;
        private const int _verticiesPerTile = 4;
        private const int _primitivesPerTile = 2;
        private readonly bool _cacheRenderDetails;

        private readonly GraphicsDevice _graphicsDevice;
        private readonly Dictionary<string, MapRenderDetails> _renderDetailsCache;
        private BasicEffect _basicEffect;
        private MapRenderDetails _currentRenderDetails;
        private readonly DepthStencilState _depthBufferState;

        private TiledMap _map;

        public FullMapRenderer(GraphicsDevice graphicsDevice, bool cacheRenderDetails = true)
        {
            _graphicsDevice = graphicsDevice;
            _cacheRenderDetails = cacheRenderDetails;

            _renderDetailsCache = new Dictionary<string, MapRenderDetails>();
            _depthBufferState = new DepthStencilState {DepthBufferEnable = true};
        }

        public virtual void SwapMap(TiledMap newMap)
        {
            _map = newMap;

            if (!_renderDetailsCache.TryGetValue(_map.Name, out _currentRenderDetails))
            {
                _currentRenderDetails = BuildRenderDetails();

                if (_cacheRenderDetails)
                    _renderDetailsCache[_map.Name] = _currentRenderDetails;
            }

            _basicEffect = new BasicEffect(_graphicsDevice)
            {
                World = Matrix.CreateLookAt(new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -2f), Vector3.Up),
                TextureEnabled = true
            };
        }

        [Obsolete]
        public void Draw(Camera2D camera)
        {
            Draw(camera.GetViewMatrix());
        }

        public virtual void Draw(Matrix viewMatrix)
        {
            if (_map == null)
                return;

            _basicEffect.View = viewMatrix;
            _basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.Viewport.Width,
                _graphicsDevice.Viewport.Height, 0, 0f, -1f);

            _graphicsDevice.DepthStencilState = _depthBufferState;
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
                foreach (var renderDetails in _currentRenderDetails)
                {
                    _graphicsDevice.SetVertexBuffer(renderDetails.VertexBuffer);
                    _graphicsDevice.Indices = renderDetails.IndexBuffer;

                    if (renderDetails.TileCount > 0)
                    {
                        _basicEffect.Alpha = renderDetails.Opacity;

                        if (_basicEffect.Texture != renderDetails.Texture)
                            _basicEffect.Texture = renderDetails.Texture;

                        pass.Apply();

                        var maxTilesPerDraw = ushort.MaxValue/_verticiesPerTile + 1;
                        var drawCalls = renderDetails.TileCount/maxTilesPerDraw + 1;

                        for (var i = 0; i < drawCalls; i++)
                        {
                            var currentIterTiles = maxTilesPerDraw;
                            if (i == drawCalls - 1)
                                currentIterTiles = renderDetails.TileCount - i*maxTilesPerDraw;

                            _graphicsDevice.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList,
                                i*maxTilesPerDraw*_verticiesPerTile,
                                i*maxTilesPerDraw*_indexesPerTile,
                                currentIterTiles*_primitivesPerTile
                            );
                        }
                    }
                }
        }

        protected virtual MapRenderDetails BuildRenderDetails()
        {
            var mapDetails = new MapRenderDetails();

            foreach (var layer in _map.Layers.OrderByDescending(l => l.Depth))
            {
                var tileLayer = layer as TiledTileLayer;
                var imageLayer = layer as TiledImageLayer;

                if (imageLayer != null)
                {
                    var region = new TextureRegion2D(imageLayer.Texture);
                    var point = imageLayer.Position.ToPoint();

                    VertexPositionTexture[] vertices;
                    ushort[] indexes;
                    CreatePrimitives(point, region, 0, layer.Depth, out vertices, out indexes);

                    var group = new GroupRenderDetails(region.Texture, 1);
                    group.SetVertices(vertices, _graphicsDevice);
                    group.SetIndexes(indexes, _graphicsDevice);
                    group.Opacity = layer.Opacity;

                    mapDetails.AddGroup(group);
                }
                else if (tileLayer != null)
                {
                    var verticesByTileset =
                        _map.Tilesets.ToDictionary(ts => ts, ts => new List<VertexPositionTexture>());
                    var indexesByTileset =
                        _map.Tilesets.ToDictionary(ts => ts, ts => new List<ushort>());
                    var tileCountByTileset =
                        _map.Tilesets.ToDictionary(ts => ts, ts => 0);

                    foreach (var tile in GetTilesGroupedByTileset(tileLayer))
                    {
                        if (tile.IsBlank)
                            continue;

                        var tileset = _map.GetTilesetByTileId(tile.Id);

                        var region = tileset.GetTileRegion(tile.Id);
                        var point = tileLayer.GetTileLocation(tile);

                        VertexPositionTexture[] vertices;
                        ushort[] indexes;
                        CreatePrimitives(point, region, tileCountByTileset[tileset], layer.Depth, out vertices,
                            out indexes);

                        verticesByTileset[tileset].AddRange(vertices);
                        indexesByTileset[tileset].AddRange(indexes);
                        tileCountByTileset[tileset]++;
                    }

                    foreach (var tileset in _map.Tilesets)
                    {
                        if (tileCountByTileset[tileset] == 0)
                            continue;

                        var group = new GroupRenderDetails(tileset.Texture, tileCountByTileset[tileset]);
                        group.SetVertices(verticesByTileset[tileset], _graphicsDevice);
                        group.SetIndexes(indexesByTileset[tileset], _graphicsDevice);
                        group.Opacity = layer.Opacity;

                        mapDetails.AddGroup(group);
                    }
                }
            }

            return mapDetails;
        }

        protected virtual List<TiledTile> GetTilesGroupedByTileset(TiledTileLayer layer)
        {
            var tilesByTileset =
                _map.Tilesets.ToDictionary(ts => ts, ts => new List<TiledTile>());

            foreach (var tile in layer.GetTilesInRenderOrder())
            {
                var ts = _map.GetTilesetByTileId(tile.Id);

                if (ts != null)
                    tilesByTileset[ts].Add(tile);
            }

            var tiles = new List<TiledTile>();

            foreach (var chunk in tilesByTileset.Values)
                tiles.AddRange(chunk);

            return tiles;
        }

        protected virtual void CreatePrimitives(Point point, TextureRegion2D region, int offset, float depth,
            out VertexPositionTexture[] vertices, out ushort[] indexes)
        {
            var tileWidth = region.Width;
            var tileHeight = region.Height;
            var tc0 = Vector2.Zero;
            var tc1 = Vector2.One;

            tc0.X = (region.X + 0.5f)/region.Texture.Width;
            tc0.Y = (region.Y + 0.5f)/region.Texture.Height;
            tc1.X = (float) (region.X + region.Width)/region.Texture.Width;
            tc1.Y = (float) (region.Y + region.Height)/region.Texture.Height;
            vertices = new VertexPositionTexture[4];
            vertices[0] = new VertexPositionTexture(new Vector3(point.X, point.Y, depth), tc0);
            vertices[1] = new VertexPositionTexture(new Vector3(point.X + tileWidth, point.Y, depth),
                new Vector2(tc1.X, tc0.Y));
            vertices[2] = new VertexPositionTexture(new Vector3(point.X, point.Y + tileHeight, depth),
                new Vector2(tc0.X, tc1.Y));
            vertices[3] = new VertexPositionTexture(new Vector3(point.X + tileWidth, point.Y + tileHeight, depth), tc1);

            indexes = new ushort[6];
            indexes[0] = (ushort) (4*offset);
            indexes[1] = (ushort) (4*offset + 1);
            indexes[2] = (ushort) (4*offset + 2);
            indexes[3] = (ushort) (4*offset + 1);
            indexes[4] = (ushort) (4*offset + 3);
            indexes[5] = (ushort) (4*offset + 2);
        }
    }
}