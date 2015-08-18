using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledMap
    {
        public TiledMap(GraphicsDevice graphicsDevice, int width, int height, int tileWidth, int tileHeight)
        {
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Properties = new TiledProperties();

            _graphicsDevice = graphicsDevice;
            _layers = new List<TiledLayer>();
            _tilesets = new List<TiledTileset>();
        }
        
        private readonly List<TiledTileset> _tilesets;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly List<TiledLayer> _layers;
        
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public Color? BackgroundColor { get; set; }
        public TiledRenderOrder RenderOrder { get; set; }
        public TiledProperties Properties { get; private set; }

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
            get { return Width * TileWidth - Width; }       // annoyingly we have to compensate 1 pixel per tile, seems to be a bug in MonoGame?
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

        public void Draw(Camera2D camera, bool useMapBackgroundColor = false)
        {
            if(useMapBackgroundColor && BackgroundColor.HasValue)
                _graphicsDevice.Clear(BackgroundColor.Value);

            foreach (var layer in _layers)
                layer.Draw(camera);
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
