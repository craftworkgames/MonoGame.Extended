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
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _layers = new List<TiledLayer>();
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
        private readonly RenderTarget2D _renderTarget;
        private readonly SpriteBatch _spriteBatch;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public Color? BackgroundColor { get; set; }
        public TiledRenderOrder RenderOrder { get; set; }
        public TiledProperties Properties { get; private set; }
        public TiledMapOrientation Orientation { get; private set; }

        public IEnumerable<TiledLayer> Layers
        {
            get { return _layers; }
        }

        public IEnumerable<TiledImageLayer> ImageLayers
        {
            get { return _layers.OfType<TiledImageLayer>(); }
        }

        public IEnumerable<TiledTileLayer> TileLayers
        {
            get { return _layers.OfType<TiledTileLayer>(); }
        }

        public int WidthInPixels
        {
            // annoyingly we have to compensate 1 pixel per tile, seems to be a bug in MonoGame?
            get { return Width * TileWidth - Width; }       
        }

        public int HeightInPixels
        {
            get { return Height * TileHeight - Height; }
        }

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

        public TiledLayer GetLayer(string name)
        {
            return _layers.FirstOrDefault(i => i.Name == name);
        }

        public T GetLayer<T>(string name)
            where T : TiledLayer
        {
            return (T) GetLayer(name);
        }

        public void Draw(Camera2D camera, bool useMapBackgroundColor = false)
        {
            if(useMapBackgroundColor && BackgroundColor.HasValue)
                _graphicsDevice.Clear(BackgroundColor.Value);

            _graphicsDevice.SetRenderTarget(_renderTarget);

            foreach (var layer in _layers)
                layer.Draw();

            _graphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.NonPremultiplied,
                samplerState: SamplerState.PointClamp, transformMatrix: camera.GetViewMatrix());
            _spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        public TextureRegion2D GetTileRegion(int id)
        {
            if (id == 0)
                return null;

            var tileset = _tilesets.LastOrDefault(i => i.FirstId <= id);

            if (tileset == null)
                throw new InvalidOperationException(string.Format("No tileset found for id {0}", id));

            return tileset.GetTileRegion(id);
        }
    }
}
