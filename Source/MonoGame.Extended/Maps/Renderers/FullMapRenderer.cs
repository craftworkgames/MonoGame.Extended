﻿using System;
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
        private const int IndexesPerTile = 6;
        private const int VerticiesPerTile = 4;
        private const int PrimitivesPerTile = 2;

        private readonly GraphicsDevice _graphicsDevice;
        private readonly MapRendererConfig _config;

        private TiledMap _map;
        private BasicEffect _basicEffect;
        private DepthStencilState _depthBufferState;
        private readonly Dictionary<string, MapRenderDetails> _renderDetailsCache;
        private MapRenderDetails _currentRenderDetails;

        public FullMapRenderer(GraphicsDevice graphicsDevice, MapRendererConfig config)
        {
            _graphicsDevice = graphicsDevice;
            _config = config;

            _renderDetailsCache = new Dictionary<string, MapRenderDetails>();
            _depthBufferState = new DepthStencilState {DepthBufferEnable = true};
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
                {
                    _renderDetailsCache[_map.Name] = _currentRenderDetails;
                }
            }

            _basicEffect = new BasicEffect(_graphicsDevice)
            {
                World = Matrix.CreateLookAt(cameraPosition: new Vector3(0f, 0f, -1f), cameraTarget: new Vector3(0f, 0f, -2f), cameraUpVector: Vector3.Up),
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
            {
                return;
            }

            _basicEffect.View = viewMatrix;
            _basicEffect.Projection = Matrix.CreateOrthographicOffCenter(left: 0, right: _graphicsDevice.Viewport.Width,
                bottom: _graphicsDevice.Viewport.Height, top: 0, zNearPlane: 0f, zFarPlane: -1f);

            _graphicsDevice.DepthStencilState = _depthBufferState;
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                foreach (GroupRenderDetails renderDetails in _currentRenderDetails)
                {
                    _graphicsDevice.SetVertexBuffer(renderDetails.VertexBuffer);
                    _graphicsDevice.Indices = renderDetails.IndexBuffer;

                    if (renderDetails.TileCount > 0)
                    {
                        _basicEffect.Alpha = renderDetails.Opacity;

                        if (_basicEffect.Texture != renderDetails.Texture)
                            _basicEffect.Texture = renderDetails.Texture;

                        pass.Apply();

                        int maxTilesPerDraw = ushort.MaxValue / VerticiesPerTile + 1;
                        int drawCalls = renderDetails.TileCount / maxTilesPerDraw + 1;

                        for (int i = 0; i < drawCalls; i++)
                        {
                            int currentIterTiles = maxTilesPerDraw;
                            if (i == drawCalls - 1)
                            {
                                currentIterTiles = renderDetails.TileCount - i * maxTilesPerDraw;
                            }

                            _graphicsDevice.DrawIndexedPrimitives(
                                primitiveType: PrimitiveType.TriangleList,
                                baseVertex: i * maxTilesPerDraw * VerticiesPerTile,
                                startIndex: i * maxTilesPerDraw * IndexesPerTile,
                                primitiveCount: currentIterTiles * PrimitivesPerTile
                            );
                        }
                    }
                }
            }
        }

        protected virtual MapRenderDetails BuildRenderDetails()
        {
            MapRenderDetails mapDetails = new MapRenderDetails();

            foreach (TiledLayer layer in _map.Layers.OrderByDescending(l => l.Depth))
            {
                TiledTileLayer tileLayer = layer as TiledTileLayer;
                TiledImageLayer imageLayer = layer as TiledImageLayer;
                TiledObjectGroup objectGroup = layer as TiledObjectGroup;

                if (imageLayer != null)
                {
                    TextureRegion2D region = new TextureRegion2D(imageLayer.Texture);
                    Point point = imageLayer.Position.ToPoint();

                    VertexPositionTexture[] vertices;
                    ushort[] indexes;
                    CreatePrimitives(point, region, 0, layer.Depth, out vertices, out indexes);

                    GroupRenderDetails group = new GroupRenderDetails(region.Texture, 1);
                    group.SetVertices(vertices, _graphicsDevice);
                    group.SetIndexes(indexes, _graphicsDevice);
                    group.Opacity = layer.Opacity;

                    mapDetails.AddGroup(group);
                }
                else if (objectGroup != null)
                {
                    if (!_config.DrawObjectLayers)
                    {
                        continue;
                    }

                    Func<TiledObject, bool> f =
                        o => o.ObjectType != TiledObjectType.Tile || !o.IsVisible || !o.Gid.HasValue;

                    IEnumerable<GroupRenderDetails> groups =
                        CreateGroupsByTileset(objectGroup.Objects.Reverse(), objectGroup, f, o => o.Gid.Value,
                            o => (o.Position - new Vector2(0, o.Height)).ToPoint(), o => new SizeF(o.Width, o.Height));

                    foreach (GroupRenderDetails g in groups)
                    {
                        mapDetails.AddGroup(g);
                    }
                }
                else if (tileLayer != null)
                {
                    IEnumerable<GroupRenderDetails> groups =
                        CreateGroupsByTileset(GetTilesGroupedByTileset(tileLayer), tileLayer, t => t.IsBlank, t => t.Id,
                            t => tileLayer.GetTileLocation(t));

                    foreach (GroupRenderDetails g in groups)
                    {
                        mapDetails.AddGroup(g);
                    }
                }
            }

            return mapDetails;
        }

        protected virtual List<TiledTile> GetTilesGroupedByTileset(TiledTileLayer layer)
        {
            Dictionary<TiledTileset, List<TiledTile>> tilesByTileset =
                _map.Tilesets.ToDictionary(ts => ts, ts => new List<TiledTile>());

            foreach (TiledTile tile in layer.GetTilesInRenderOrder())
            {
                TiledTileset ts = _map.GetTilesetByTileId(tile.Id);

                if (ts != null)
                {
                    tilesByTileset[ts].Add(tile);
                }
            }

            List<TiledTile> tiles = new List<TiledTile>();

            foreach (List<TiledTile> chunk in tilesByTileset.Values)
            {
                tiles.AddRange(chunk);
            }

            return tiles;
        }

        protected virtual IEnumerable<GroupRenderDetails> CreateGroupsByTileset<T>(IEnumerable<T> objs, TiledLayer layer,
            Func<T, bool> filterOut, Func<T, int> getTileId, Func<T, Point> getPosition, Func<T, SizeF> getSize = null)
        {
            Dictionary<TiledTileset, List<VertexPositionTexture>> verticesByTileset =
                _map.Tilesets.ToDictionary(ts => ts, ts => new List<VertexPositionTexture>());
            Dictionary<TiledTileset, List<ushort>> indexesByTileset =
                _map.Tilesets.ToDictionary(ts => ts, ts => new List<ushort>());
            Dictionary<TiledTileset, int> tileCountByTileset =
                _map.Tilesets.ToDictionary(ts => ts, ts => 0);

            foreach (T obj in objs)
            {
                if (filterOut(obj))
                {
                    continue;
                }

                int tileId = getTileId(obj);
                TiledTileset tileset = _map.GetTilesetByTileId(tileId);

                var region = tileset.GetTileRegion(tileId);
                var point = getPosition(obj);

                VertexPositionTexture[] vertices;
                ushort[] indexes;
                CreatePrimitives(point, region, tileCountByTileset[tileset], layer.Depth,
                    out vertices, out indexes, getSize?.Invoke(obj));

                verticesByTileset[tileset].AddRange(vertices);
                indexesByTileset[tileset].AddRange(indexes);
                tileCountByTileset[tileset]++;
            }

            List<GroupRenderDetails> groups = new List<GroupRenderDetails>();

            foreach (TiledTileset tileset in _map.Tilesets)
            {
                if (tileCountByTileset[tileset] == 0)
                {
                    continue;
                }

                GroupRenderDetails group = new GroupRenderDetails(tileset.Texture, tileCountByTileset[tileset]);
                group.SetVertices(verticesByTileset[tileset], _graphicsDevice);
                group.SetIndexes(indexesByTileset[tileset], _graphicsDevice);
                group.Opacity = layer.Opacity;

                groups.Add(group);
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
            indexes[0] = (ushort) (4 * offset);
            indexes[1] = (ushort) (4 * offset + 1);
            indexes[2] = (ushort) (4 * offset + 2);
            indexes[3] = (ushort) (4 * offset + 1);
            indexes[4] = (ushort) (4 * offset + 3);
            indexes[5] = (ushort) (4 * offset + 2);
        }
    }
}