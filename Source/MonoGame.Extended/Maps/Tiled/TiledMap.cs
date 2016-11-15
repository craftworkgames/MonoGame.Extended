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
        private readonly List<TiledLayer> _layers;
        private readonly List<TiledTileset> _tilesets;

        public TiledMap(string name, int width, int height, int tileWidth, int tileHeight,
            TiledMapOrientation orientation = TiledMapOrientation.Orthogonal)
        {
            _layers = new List<TiledLayer>();
            _tilesets = new List<TiledTileset>();

            Name = name;
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Properties = new TiledProperties();
            Orientation = orientation;
        }

        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }

        public Color? BackgroundColor { get; set; }
        public TiledRenderOrder RenderOrder { get; set; }
        public TiledProperties Properties { get; private set; }
        public TiledMapOrientation Orientation { get; private set; }

        public IReadOnlyList<TiledTileset> Tilesets => _tilesets;
        public IReadOnlyList<TiledLayer> Layers => _layers;
        public IReadOnlyList<TiledImageLayer> ImageLayers => _layers.OfType<TiledImageLayer>().ToList();
        public IReadOnlyList<TiledTileLayer> TileLayers => _layers.OfType<TiledTileLayer>().ToList();
        public IReadOnlyList<TiledObjectLayer> ObjectLayers => _layers.OfType<TiledObjectLayer>().ToList();
        public int WidthInPixels => Width*TileWidth;
        public int HeightInPixels => Height*TileHeight;

        public void Dispose()
        {
            foreach (var tiledLayer in _layers)
                tiledLayer.Dispose();
        }

        public TiledTileset CreateTileset(Texture2D texture, int firstId, int tileWidth, int tileHeight, int tileCount,
            int spacing = 2, int margin = 2)
        {
            var tileset = new TiledTileset(texture, firstId, tileWidth, tileHeight, tileCount, spacing, margin);
            _tilesets.Add(tileset);
            return tileset;
        }

        public TiledTileLayer CreateTileLayer(string name, int width, int height, int[] data)
        {
            var layer = new TiledTileLayer(this, name, width, height, data);
            _layers.Add(layer);
            return layer;
        }

        public TiledImageLayer CreateImageLayer(string name, Texture2D texture, Vector2 position)
        {
            var layer = new TiledImageLayer(name, texture, position);
            _layers.Add(layer);
            return layer;
        }

        public void AddLayer(TiledLayer layer)
        {
            _layers.Add(layer);
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

        public TiledObjectLayer GetObjectGroup(string name)
        {
            return ObjectLayers.FirstOrDefault(i => i.Name == name);
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

        public TiledTilesetTile GetTilesetTileById(int tilesetTileId)
        {
            return _tilesets
                .SelectMany(ts => ts.Tiles, (ts, t) => t)
                .FirstOrDefault(t => t.Id == tilesetTileId - 1);
        }

        public TiledTileset GetTilesetByTileId(int tileId)
        {
            return _tilesets.FirstOrDefault(ts => ts.ContainsTileId(tileId));
        }
    }
}