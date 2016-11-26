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

        private readonly List<TiledTilesetTile> _animatedTilesetTiles;
        private readonly MapRendererConfig _config;
        private readonly VertexPositionTexture[] _dynamicVertices = new VertexPositionTexture[4];

        private readonly GraphicsDevice _graphicsDevice;
        private readonly Dictionary<string, MapRenderDetails> _renderDetailsCache;
        private readonly List<VertexPositionTexture> _updatedDynamicVertices = new List<VertexPositionTexture>();
        private BasicEffect _basicEffect;
        private MapRenderDetails _currentRenderDetails;
        private readonly DepthStencilState _depthBufferState;

        private TiledMap _map;

        public FullMapRenderer(GraphicsDevice graphicsDevice, MapRendererConfig config)
        {
            _graphicsDevice = graphicsDevice;
            _config = config;

            _renderDetailsCache = new Dictionary<string, MapRenderDetails>();
            _depthBufferState = new DepthStencilState { DepthBufferEnable = true };

            _animatedTilesetTiles = new List<TiledTilesetTile>();
        }

        public FullMapRenderer(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, new MapRendererConfig())
        {
        }

        public virtual void SwapMap(TiledMap newMap)
        {
            _map = newMap;

            if (!_renderDetailsCache.TryGetValue(_map.Name, out _currentRenderDetails))
            {
                _currentRenderDetails = BuildRenderDetails();

                if (_config.CacheRenderDetails)
                    _renderDetailsCache[_map.Name] = _currentRenderDetails;
            }

            _basicEffect = new BasicEffect(_graphicsDevice)
            {
                World = Matrix.CreateLookAt(new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -2f), Vector3.Up),
                TextureEnabled = true
            };
        }

        public void Update(GameTime gameTime)
        {
            foreach (var animatedTile in _animatedTilesetTiles)
                animatedTile.Update(gameTime.ElapsedGameTime.Milliseconds);

            RebuildDynamicRenderDetails();
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

                        var maxTilesPerDraw = ushort.MaxValue / _verticiesPerTile + 1;
                        var drawCalls = renderDetails.TileCount / maxTilesPerDraw + 1;

                        for (var i = 0; i < drawCalls; i++)
                        {
                            var currentIterTiles = maxTilesPerDraw;
                            if (i == drawCalls - 1)
                                currentIterTiles = renderDetails.TileCount - i * maxTilesPerDraw;

                            _graphicsDevice.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList,
                                i * maxTilesPerDraw * _verticiesPerTile,
                                i * maxTilesPerDraw * _indexesPerTile,
                                currentIterTiles * _primitivesPerTile
                            );
                        }
                    }
                }
        }

        protected virtual MapRenderDetails BuildRenderDetails()
        {
            var mapDetails = new MapRenderDetails();
            _animatedTilesetTiles.Clear();

            foreach (var layer in _map.Layers.OrderByDescending(l => l.Depth))
            {
                var tileLayer = layer as TiledTileLayer;
                var imageLayer = layer as TiledImageLayer;
                var objectLayer = layer as TiledObjectLayer;

                if (imageLayer != null)
                {
                    var region = new TextureRegion2D(imageLayer.Texture);
                    var point = imageLayer.Position.ToPoint();

                    VertexPositionTexture[] vertices;
                    ushort[] indexes;
                    CreatePrimitives(point, region, 0, layer.Depth, out vertices, out indexes);

                    var group = new GroupRenderDetails(_graphicsDevice, vertices, indexes, region.Texture, 1);
                    group.SetIndexes(indexes);
                    group.Opacity = layer.Opacity;

                    mapDetails.AddGroup(group);
                }
                else if (objectLayer != null)
                {
                    if (!_config.DrawObjectLayers)
                        continue;
                    
                    var groups = CreateGroupsByTileset(
                        animatedObjects: objectLayer.Objects.Reverse(),
                        layer: objectLayer, 
                        filterOut: o => (o.ObjectType != TiledObjectType.Tile) || !o.IsVisible,
                        getPosition: o => (o.Position - new Vector2(0, o.Height)).ToPoint(),
                        getSize: o => new SizeF(o.Width, o.Height));

                    foreach (var groupRenderDetails in groups)
                        mapDetails.AddGroup(groupRenderDetails);
                }
                else if (tileLayer != null)
                {
                    var groups = CreateGroupsByTileset(
                        animatedObjects: GetTilesGroupedByTileset(tileLayer),
                        layer: tileLayer,
                        filterOut: t => t.IsBlank,
                        getPosition: t => tileLayer.GetTileLocation(t));

                    foreach (var groupRenderDetails in groups)
                        mapDetails.AddGroup(groupRenderDetails);
                }
            }

            return mapDetails;
        }

        public void RebuildDynamicRenderDetails()
        {
            if (_currentRenderDetails == null)
            {
                return;
            }

            var camPosition = Vector2.Negate(Vector2.Transform(Vector2.Zero, _basicEffect.View));
            var camViewport = new Rectangle((int)camPosition.X, (int)camPosition.Y, _graphicsDevice.Viewport.Width,
                _graphicsDevice.Viewport.Height);

            foreach (var renderDetails in _currentRenderDetails)
            {
                var dynamicRenderDetails = renderDetails as DynamicGroupRenderDetails;
                if ((dynamicRenderDetails == null) || (dynamicRenderDetails.TileCount == 0))
                    continue;

                _updatedDynamicVertices.Clear();
                _updatedDynamicVertices.Capacity = dynamicRenderDetails.TileCount * 4;

                for (var i = 0; i < dynamicRenderDetails.TileCount; i++)
                {
                    var tile = dynamicRenderDetails.AnimatedTiles[i];

                    _dynamicVertices[0] = dynamicRenderDetails.Vertices[i * 4];
                    _dynamicVertices[1] = dynamicRenderDetails.Vertices[i * 4 + 1];
                    _dynamicVertices[2] = dynamicRenderDetails.Vertices[i * 4 + 2];
                    _dynamicVertices[3] = dynamicRenderDetails.Vertices[i * 4 + 3];

                    var rectangle = new Rectangle((int)tile.Position.X * _map.TileWidth, (int)tile.Position.Y * _map.TileHeight, _map.TileWidth, _map.TileHeight);

                    if (camViewport.Contains(rectangle))
                    {
                        var tileset = _map.GetTilesetByTileId(tile.Gid ?? 0);
                        var tileId = tile.TilesetTile.CurrentTileId + 1 ?? tile.CurrentTileId ?? 0;

                        var region = tileset.GetTileRegion(tileId);
                        Vector2 tc0, tc1;
                        tc0.X = (region.X + 0.5f) / region.Texture.Width;
                        tc0.Y = (region.Y + 0.5f) / region.Texture.Height;
                        tc1.X = (float)(region.X + region.Width) / region.Texture.Width;
                        tc1.Y = (float)(region.Y + region.Height) / region.Texture.Height;

                        _dynamicVertices[0].TextureCoordinate = tc0;
                        _dynamicVertices[1].TextureCoordinate = new Vector2(tc1.X, tc0.Y);
                        _dynamicVertices[2].TextureCoordinate = new Vector2(tc0.X, tc1.Y);
                        _dynamicVertices[3].TextureCoordinate = tc1;
                    }

                    _updatedDynamicVertices.AddRange(_dynamicVertices);
                }
                dynamicRenderDetails.SetVertices(_updatedDynamicVertices);
            }
        }

        protected virtual List<TiledTile> GetTilesGroupedByTileset(TiledTileLayer layer)
        {
            var tilesByTileset = _map.Tilesets.ToDictionary(ts => ts, ts => new List<TiledTile>());

            foreach (var tile in layer.GetTilesInRenderOrder())
            {
                var tileset = _map.GetTilesetByTileId(tile.Id);

                if (tileset != null)
                    tilesByTileset[tileset].Add(tile);
            }

            var tiles = new List<TiledTile>();

            foreach (var chunk in tilesByTileset.Values)
                tiles.AddRange(chunk);

            return tiles;
        }

        protected virtual IEnumerable<GroupRenderDetails> CreateGroupsByTileset<T>(IEnumerable<T> animatedObjects, TiledLayer layer,
            Func<T, bool> filterOut, Func<T, Point> getPosition,
            Func<T, SizeF> getSize = null)
            where T : ITiledAnimated
        {
            var staticVerticesByTileset = _map.Tilesets.ToDictionary(ts => ts, ts => new List<VertexPositionTexture>());
            var staticIndexesByTileset = _map.Tilesets.ToDictionary(ts => ts, ts => new List<ushort>());
            var staticTileCountByTileset = _map.Tilesets.ToDictionary(ts => ts, ts => 0);

            var dynamicVerticesByTileset = _map.Tilesets.ToDictionary(ts => ts, ts => new List<VertexPositionTexture>());
            var dynamicIndexesByTileset = _map.Tilesets.ToDictionary(ts => ts, ts => new List<ushort>());
            var dynamicTileCountByTileset = _map.Tilesets.ToDictionary(ts => ts, ts => 0);

            var animatedTiles = new List<ITiledAnimated>();

            foreach (var animatedObject in animatedObjects)
            {
                if (!animatedObject.Gid.HasValue || filterOut(animatedObject))
                    continue;

                var tileId = animatedObject.Gid.Value;
                var tileset = _map.GetTilesetByTileId(tileId);

                var region = tileset.GetTileRegion(tileId);
                var point = getPosition(animatedObject);

                VertexPositionTexture[] vertices;
                ushort[] indexes;

                if (animatedObject.HasAnimation)
                {
                    CreatePrimitives(point, region, dynamicTileCountByTileset[tileset], layer.Depth, out vertices, out indexes, getSize?.Invoke(animatedObject));

                    dynamicVerticesByTileset[tileset].AddRange(vertices);
                    dynamicIndexesByTileset[tileset].AddRange(indexes);
                    dynamicTileCountByTileset[tileset]++;

                    var tile = animatedObject;

                    if (!_animatedTilesetTiles.Contains(tile.TilesetTile))
                        _animatedTilesetTiles.Add(tile.TilesetTile);

                    animatedTiles.Add(tile);
                }
                else
                {
                    CreatePrimitives(point, region, staticTileCountByTileset[tileset], layer.Depth, out vertices, out indexes, getSize?.Invoke(animatedObject));

                    staticVerticesByTileset[tileset].AddRange(vertices);
                    staticIndexesByTileset[tileset].AddRange(indexes);
                    staticTileCountByTileset[tileset]++;
                }
            }

            var groups = new List<GroupRenderDetails>();

            foreach (var tileset in _map.Tilesets)
            {
                if (staticTileCountByTileset[tileset] == 0)
                    continue;

                var staticGroup = new GroupRenderDetails(_graphicsDevice, staticVerticesByTileset[tileset],
                    staticIndexesByTileset[tileset], tileset.Texture, staticTileCountByTileset[tileset])
                {
                    Opacity = layer.Opacity
                };

                groups.Add(staticGroup);


                if (dynamicTileCountByTileset[tileset] == 0)
                    continue;

                var dynamicGroup = new DynamicGroupRenderDetails(_graphicsDevice, dynamicVerticesByTileset[tileset], dynamicIndexesByTileset[tileset], tileset.Texture, animatedTiles)
                {
                    Opacity = layer.Opacity
                };

                groups.Add(dynamicGroup);
            }

            return groups;
        }

        protected virtual void CreatePrimitives(Point point, TextureRegion2D region, int offset, float depth,
            out VertexPositionTexture[] vertices, out ushort[] indexes, SizeF? size = null)
        {
            var tileWidth = size?.Width ?? region.Width;
            var tileHeight = size?.Height ?? region.Height;
            var tc0 = Vector2.Zero;
            var tc1 = Vector2.One;

            tc0.X = (region.X + 0.5f) / region.Texture.Width;
            tc0.Y = (region.Y + 0.5f) / region.Texture.Height;
            tc1.X = (float)(region.X + region.Width) / region.Texture.Width;
            tc1.Y = (float)(region.Y + region.Height) / region.Texture.Height;
            vertices = new VertexPositionTexture[4];
            vertices[0] = new VertexPositionTexture(new Vector3(point.X, point.Y, depth), tc0);
            vertices[1] = new VertexPositionTexture(new Vector3(point.X + tileWidth, point.Y, depth), new Vector2(tc1.X, tc0.Y));
            vertices[2] = new VertexPositionTexture(new Vector3(point.X, point.Y + tileHeight, depth), new Vector2(tc0.X, tc1.Y));
            vertices[3] = new VertexPositionTexture(new Vector3(point.X + tileWidth, point.Y + tileHeight, depth), tc1);

            indexes = new ushort[6];
            indexes[0] = (ushort)(4 * offset);
            indexes[1] = (ushort)(4 * offset + 1);
            indexes[2] = (ushort)(4 * offset + 2);
            indexes[3] = (ushort)(4 * offset + 1);
            indexes[4] = (ushort)(4 * offset + 3);
            indexes[5] = (ushort)(4 * offset + 2);
        }
    }
}