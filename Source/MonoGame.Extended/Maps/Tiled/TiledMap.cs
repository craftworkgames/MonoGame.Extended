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
            _renderTarget = new RenderTarget2D(graphicsDevice, width*tileWidth, height*tileHeight);
            //_spriteBatch = new SpriteBatch(graphicsDevice);
            _layers = new List<TiledLayer>();
            _objectGroups = new List<TiledObjectGroup>();
            _tilesets = new List<TiledTileset>();

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

        private readonly List<TiledTileset> _tilesets;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly List<TiledLayer> _layers;
        private readonly List<TiledObjectGroup> _objectGroups;
        private readonly RenderTarget2D _renderTarget;
        //private readonly SpriteBatch _spriteBatch;

        public int Width { get; }
        public int Height { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public Color? BackgroundColor { get; set; }
        public TiledRenderOrder RenderOrder { get; set; }
        public TiledProperties Properties { get; private set; }
        public TiledMapOrientation Orientation { get; private set; }

        public List<TiledObjectGroup> ObjectGroups => _objectGroups;
        public IEnumerable<TiledLayer> Layers => _layers;
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
            var layer = new TiledImageLayer(_graphicsDevice, name, texture, position);
            _layers.Add(layer);
            return layer;
        }

        public TiledObjectGroup CreateObjectGroup(string name, TiledObject[] objects)
        {
            var objectGroup = new TiledObjectGroup(name, objects);
            _objectGroups.Add(objectGroup);
            return objectGroup;
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

        public void Draw(SpriteBatch spriteBatch, Camera2D camera, bool useMapBackgroundColor = false)
        {
            // it's important to get the camera state before setting the render target
            // because the render target changes the size of the viewport
            var boundingRectangle = camera.GetBoundingRectangle();
            var viewport = _graphicsDevice.Viewport;
            var previousRenderTargetUsage = _graphicsDevice.PresentationParameters.RenderTargetUsage;

            _graphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            _graphicsDevice.SetRenderTarget(_renderTarget); 

            if (useMapBackgroundColor && BackgroundColor.HasValue)
                _graphicsDevice.Clear(BackgroundColor.Value);
            else
                _graphicsDevice.Clear(Color.Transparent);

            foreach (var layer in _layers)
                layer.Draw(boundingRectangle);

            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.PresentationParameters.RenderTargetUsage = previousRenderTargetUsage;
            _graphicsDevice.Viewport = viewport;

            spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
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
    }
}
