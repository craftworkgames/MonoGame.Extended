using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
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

            _depthBufferState = new DepthStencilState();
            _depthBufferState.DepthBufferEnable = true;

            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Properties = new TiledProperties();
            _tilesets = new List<TiledTileset>();
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
        private int _tilesPrimitivesCount;

        private Matrix _worldMatrix;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;
        private BasicEffect _basicEffect;
        private readonly DepthStencilState _depthBufferState;

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

        public TiledTileLayer CreateTileLayer(string name, int width, int height, int[] data, int z)
        {
            var layer = new TiledTileLayer(this, _graphicsDevice, name, width, height, data, z);
            _layers.Add(layer);
            return layer;
        }

        public TiledImageLayer CreateImageLayer(string name, Texture2D texture, Vector2 position, int z)
        {
            var layer = new TiledImageLayer(name, texture, position, z);
            _layers.Add(layer);
            return layer;
        }

        public TiledObjectGroup CreateObjectGroup(string name, TiledObject[] objects, bool isVisible)
        {
            var objectGroup = new TiledObjectGroup(name, objects) {IsVisible = isVisible};
            _objectGroups.Add(objectGroup);
            return objectGroup;
        }

        public TiledMap Build()
        {
            var tileVertices = new List<VertexPositionTexture>();
            var tileIndexes = new List<short>();
            var tileLayers = _layers.Where(layer => layer is TiledTileLayer).ToList();
            var index = 0;
            foreach (var tileLayer in tileLayers)
            {
                var layer = (TiledTileLayer)tileLayer;
                tileVertices.AddRange(layer.RenderVertices());
                var tilesCount = layer.Tiles.Where(x => x.Id != 0).ToList().Count;
                for (var i = 0; i < tilesCount; i++)
                {
                    var thisTileIndexes = new short[6];
                    thisTileIndexes[0] = (short)(4 * index);
                    thisTileIndexes[1] = (short)(4 * index + 1);
                    thisTileIndexes[2] = (short)(4 * index + 2);
                    thisTileIndexes[3] = (short)(4 * index + 1);
                    thisTileIndexes[4] = (short)(4 * index + 3);
                    thisTileIndexes[5] = (short)(4 * index + 2);
                    tileIndexes.AddRange(thisTileIndexes);
                    _tilesPrimitivesCount += 2;
                    index++;
                }
            }

            _tilesVertices = tileVertices.ToArray();
            _tilesIndexes = tileIndexes.ToArray();
            _tilesVertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionTexture), tileVertices.Count, BufferUsage.WriteOnly);
            _tilesIndexBuffer = new IndexBuffer(_graphicsDevice, typeof(short), tileIndexes.Count, BufferUsage.WriteOnly);
            _tilesVertexBuffer.SetData(_tilesVertices);
            _tilesIndexBuffer.SetData(_tilesIndexes);

            _worldMatrix = Matrix.CreateTranslation(Vector3.Zero);

            _viewMatrix = Matrix.CreateLookAt(
                new Vector3(0f, 0f, _layers.Count),
                Vector3.Zero,
                Vector3.Up
            );

            _projectionMatrix = 
                Matrix.CreateTranslation(-0.5f, -0.5f, 0f) * Matrix.CreateOrthographicOffCenter(
                    0,
                    (float)_graphicsDevice.Viewport.Width,
                    (float)_graphicsDevice.Viewport.Height,
                    0,
                    1.0f, 1000.0f
                );

            _basicEffect = new BasicEffect(_graphicsDevice);
            _basicEffect.World = _worldMatrix;
            _basicEffect.View = _viewMatrix;
            _basicEffect.Projection = _projectionMatrix;
            _basicEffect.TextureEnabled = true;
            _basicEffect.Texture = _tilesets[0].Texture;

            return this;
        }

        public TiledLayer GetLayer(string name)
        {
            return _layers.FirstOrDefault(i => i.Name == name);
        }

        public T GetLayer<T>(string name)
            where T : TiledLayer
        {
            return (T) GetLayer(name);
        }

        public TiledObjectGroup GetObjectGroup(string name)
        {
            return _objectGroups.FirstOrDefault(i => i.Name == name);
        }
        
        public void Draw(SpriteBatch spriteBatch, Rectangle? visibleRectangle = null, float? zoom = null, GameTime gameTime = null)
        {
            /*
            foreach (var layer in _layers.Where(i => i.IsVisible))
                layer.Draw(spriteBatch, visibleRectangle, gameTime: gameTime);
            */

            var rect = visibleRectangle.HasValue ? visibleRectangle.Value : Rectangle.Empty;

            _worldMatrix.Translation = new Vector3(-rect.X, -rect.Y, 0);
            _worldMatrix.Scale = new Vector3(zoom.HasValue ? new Vector2(zoom.Value, zoom.Value) : Vector2.One, 1f);
            _basicEffect.World = _worldMatrix;

            _graphicsDevice.DepthStencilState = _depthBufferState;
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            // Images draw
            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                var imageLayers = _layers.Where(layer => layer is TiledImageLayer);
                foreach (var layer in imageLayers)
                {
                    var imageLayer = (TiledImageLayer)layer;
                    if (!imageLayer.IsVisible)
                        continue;
                    _basicEffect.Texture = imageLayer.Texture;
                    pass.Apply();
                    _graphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        imageLayer.ImageVertices,
                        0,
                        imageLayer.ImageVertices.Length,
                        imageLayer.ImageVerticesIndex,
                        0,
                        2
                    );
                }
            }

            // Tiles draw
            _graphicsDevice.SetVertexBuffer(_tilesVertexBuffer);
            _graphicsDevice.Indices = _tilesIndexBuffer;

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                _basicEffect.Texture = _tilesets[0].Texture;
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0, 0,
                    _tilesPrimitivesCount
                );
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera2D camera, GameTime gameTime = null)
        {
            var visibleRectangle = camera.GetBoundingRectangle().ToRectangle();
            Draw(spriteBatch, visibleRectangle, camera.Zoom, gameTime: gameTime);
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
            TiledTileSetTile tile = _tilesets.SelectMany(ts => ts.Tiles, (ts, t) => t).Where(t => t.Id == tileSetTileId-1).FirstOrDefault();
            return tile;
        }
    }
}
