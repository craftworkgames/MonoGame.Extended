using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledMap : IDisposable
    {
        public TiledMap(GraphicsDevice graphicsDevice, int width, int height, int tileWidth, int tileHeight,
            TiledMapOrientation orientation = TiledMapOrientation.Orthogonal)
        {
            _graphicsDevice = graphicsDevice;
            _layers = new List<TiledLayer>();
            _objectGroups = new List<TiledObjectGroup>();
            _tilesets = new List<TiledTileset>();
            _depthBufferState = new DepthStencilState { DepthBufferEnable = true };

            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Properties = new TiledProperties();
            Orientation = orientation;
        }

        public void Dispose()
        {
            foreach (var tiledLayer in _layers)
                tiledLayer.Dispose();
        }

        private readonly GraphicsDevice _graphicsDevice;
        private readonly List<TiledLayer> _layers;
        private readonly List<TiledTileset> _tilesets;
        private readonly List<TiledObjectGroup> _objectGroups;

        private VertexBuffer _tilesVertexBuffer;
        private IndexBuffer _tilesIndexBuffer;
        private VertexPositionTexture[] _tilesVertices;
        private short[] _tilesIndexes;

        private readonly DepthStencilState _depthBufferState;
        //private Matrix _worldMatrix;
        //private Matrix _viewMatrix;
        //private Matrix _projectionMatrix;
        private BasicEffect _basicEffect;
        //private float _highestZ;

        public int Width { get; }
        public int Height { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }

        public Color? BackgroundColor { get; set; }
        public TiledRenderOrder RenderOrder { get; set; }
        public TiledProperties Properties { get; private set; }
        public TiledMapOrientation Orientation { get; private set; }

        public IReadOnlyList<TiledTileset> Tilesets => _tilesets;
        public IReadOnlyList<TiledObjectGroup> ObjectGroups => _objectGroups;
        public IReadOnlyList<TiledLayer> Layers => _layers;
        public IEnumerable<TiledImageLayer> ImageLayers => _layers.OfType<TiledImageLayer>();
        public IEnumerable<TiledTileLayer> TileLayers => _layers.OfType<TiledTileLayer>();
        public int WidthInPixels => Width * TileWidth;
        public int HeightInPixels => Height * TileHeight;

        public TiledTileset CreateTileset(Texture2D texture, int firstId, int tileWidth, int tileHeight, int spacing = 2, int margin = 2)
        {
            var tileset = new TiledTileset(texture, firstId, tileWidth, tileHeight, spacing, margin);
            _tilesets.Add(tileset);
            return tileset;
        }

        public TiledTileLayer CreateTileLayer(string name, int width, int height, int[] data)
        {
            var layer = new TiledTileLayer(this, _graphicsDevice, name, width, height, data);
            _layers.Add(layer);
            return layer;
        }

        public TiledImageLayer CreateImageLayer(string name, Texture2D texture, Vector2 position)
        {
            var layer = new TiledImageLayer(name, texture, position);
            _layers.Add(layer);
            return layer;
        }

        public void AddObjectGroup(List<TiledObjectGroup> objectGroups)
        {
            _objectGroups.AddRange(objectGroups);
        }

        public TiledMap Build()
        {
            var tileVertices = new List<VertexPositionTexture>();
            var tileIndexes = new List<short>();
            var tileLayers = _layers.OfType<TiledTileLayer>().ToArray();
            var depth = 0f;
            var depthInc = 1.0f / (_layers.Count - 1);
            var indexOffset = 0;

            foreach (var layer in tileLayers)
            {
                var vertices = layer.BuildVertices(depth);
                tileVertices.AddRange(vertices);
                var tilesCount = layer.Tiles.Where(x => x.Id != 0).ToList().Count;

                for (var i = 0; i < tilesCount; i++)
                {
                    var thisTileIndexes = new short[6];
                    thisTileIndexes[0] = (short) (4*indexOffset);
                    thisTileIndexes[1] = (short) (4*indexOffset + 1);
                    thisTileIndexes[2] = (short) (4*indexOffset + 2);
                    thisTileIndexes[3] = (short) (4*indexOffset + 1);
                    thisTileIndexes[4] = (short) (4*indexOffset + 3);
                    thisTileIndexes[5] = (short) (4*indexOffset + 2);
                    tileIndexes.AddRange(thisTileIndexes);
                    indexOffset++;
                }

                depth -= depthInc;
            }

            _tilesVertices = tileVertices.ToArray();
            _tilesIndexes = tileIndexes.ToArray();
            _tilesVertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionTexture), _tilesVertices.Length, BufferUsage.WriteOnly);
            _tilesIndexBuffer = new IndexBuffer(_graphicsDevice, typeof(short), _tilesIndexes.Length, BufferUsage.WriteOnly);
            _tilesVertexBuffer.SetData(_tilesVertices);
            _tilesIndexBuffer.SetData(_tilesIndexes);

            //_highestZ = _layers.Max(layer => layer.Depth) + 1;

            // Projection vs View vs World http://gamedev.stackexchange.com/a/56203/22277

            _basicEffect = new BasicEffect(_graphicsDevice)
            {
                World = Matrix.CreateLookAt(cameraPosition: new Vector3(0f, 0f, -1f), cameraTarget: new Vector3(0f, 0f, -2f), cameraUpVector: Vector3.Up),
                TextureEnabled = true,
                Texture = _tilesets[0].Texture
            };

            return this;
        }

        public TiledLayer GetLayer(string name)
        {
            return _layers.FirstOrDefault(i => i.Name == name);
        }

        public T GetLayer<T>(string name)
            where T : TiledLayer
        {
            return (T)GetLayer(name);
        }

        public TiledObjectGroup GetObjectGroup(string name)
        {
            return _objectGroups.FirstOrDefault(i => i.Name == name);
        }

        [Obsolete]
        public void Draw(Camera2D camera)
        {
            Draw(camera.GetViewMatrix());
        }

        public void Draw(Matrix viewMatrix)
        {
            _basicEffect.View = viewMatrix;
            _basicEffect.Projection = Matrix.CreateOrthographicOffCenter(left: 0, right: _graphicsDevice.Viewport.Width,
                bottom: _graphicsDevice.Viewport.Height, top: 0, zNearPlane: 0f, zFarPlane: -1f);

            _graphicsDevice.SetVertexBuffer(_tilesVertexBuffer);
            _graphicsDevice.Indices = _tilesIndexBuffer;
            _graphicsDevice.DepthStencilState = _depthBufferState;
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                var tilesIndexesSoFar = 0;

                foreach (var layer in _layers)
                {
                    var tiledTileLayer = layer as TiledTileLayer;

                    if (tiledTileLayer != null)
                    {
                        var tileLayer = tiledTileLayer;
                        var indexCount = tileLayer.NotBlankTilesCount * 6;
                        var primitivesCount = tileLayer.NotBlankTilesCount * 2;

                        if (tileLayer.IsVisible && tileLayer.NotBlankTilesCount > 0)
                        {
                            if (_basicEffect.Texture != _tilesets[0].Texture)
                                _basicEffect.Texture = _tilesets[0].Texture;

                            pass.Apply();
                            _graphicsDevice.DrawIndexedPrimitives(
                                primitiveType: PrimitiveType.TriangleList,
                                baseVertex: 0,
                                startIndex: tilesIndexesSoFar,
                                primitiveCount: primitivesCount
                            );
                        }

                        tilesIndexesSoFar += indexCount;
                    }
                    else if (layer is TiledImageLayer)
                    {
                        if (!layer.IsVisible)
                            continue;

                        var imageLayer = (TiledImageLayer)layer;
                        _basicEffect.Texture = imageLayer.Texture;
                        pass.Apply();
                        _graphicsDevice.DrawUserIndexedPrimitives(
                            primitiveType: PrimitiveType.TriangleList,
                            vertexData: imageLayer.ImageVertices,
                            vertexOffset: 0,
                            numVertices: imageLayer.ImageVertices.Length,
                            indexData: imageLayer.ImageVerticesIndexes,
                            indexOffset: 0,
                            primitiveCount: 2
                        );
                    }
                }
            }
        }

        public TextureRegion2D GetTileRegion(int id)
        {
            if (id == 0)
                return null;

            var tileset = _tilesets.LastOrDefault(i => i.FirstId <= id);

            if (tileset == null)
                throw new InvalidOperationException($"No tileset found for id {id}");

            return tileset.GetTileRegion(id);
        }

        public TiledTileSetTile GetTileSetTileById(int tileSetTileId)
        {
            return _tilesets
                .SelectMany(ts => ts.Tiles, (ts, t) => t)
                .FirstOrDefault(t => t.Id == tileSetTileId - 1);
        }
    }
}
