using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled
{
    public sealed class TiledMap : IDisposable
    {
        private readonly List<TiledMapImageLayer> _imageLayers;
        private readonly List<TiledMapLayer> _layers;
        private readonly Dictionary<string, TiledMapLayer> _layersByName;
        private readonly List<TiledMapObjectLayer> _objectLayers;
        private readonly List<TiledMapTileLayer> _tileLayers;
        private readonly List<TiledMapTileset> _tilesets;

        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public TiledMapTileDrawOrder RenderOrder { get; }
        public TiledMapOrientation Orientation { get; }
        public TiledMapProperties Properties { get; }
        public ReadOnlyCollection<TiledMapTileset> Tilesets { get; }
        public ReadOnlyCollection<TiledMapLayer> Layers { get; }
        public ReadOnlyCollection<TiledMapImageLayer> ImageLayers { get; }
        public ReadOnlyCollection<TiledMapTileLayer> TileLayers { get; }
        public ReadOnlyCollection<TiledMapObjectLayer> ObjectLayers { get; }

        public Color? BackgroundColor { get; set; }

        public int WidthInPixels => Width * TileWidth;
        public int HeightInPixels => Height * TileHeight;

        private TiledMap()
        {
            _layers = new List<TiledMapLayer>();
            Layers = new ReadOnlyCollection<TiledMapLayer>(_layers);
            _imageLayers = new List<TiledMapImageLayer>();
            ImageLayers = new ReadOnlyCollection<TiledMapImageLayer>(_imageLayers);
            _tileLayers = new List<TiledMapTileLayer>();
            TileLayers = new ReadOnlyCollection<TiledMapTileLayer>(_tileLayers);
            _objectLayers = new List<TiledMapObjectLayer>();
            ObjectLayers = new ReadOnlyCollection<TiledMapObjectLayer>(_objectLayers);
            _layersByName = new Dictionary<string, TiledMapLayer>();
            _tilesets = new List<TiledMapTileset>();
            Tilesets = new ReadOnlyCollection<TiledMapTileset>(_tilesets);
            Properties = new TiledMapProperties();
        }

        public TiledMap(string name, int width, int height, int tileWidth, int tileHeight,
            TiledMapTileDrawOrder renderOrder, TiledMapOrientation orientation)
            : this()
        {
            Name = name;
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            RenderOrder = renderOrder;
            Orientation = orientation;
        }

        public void Dispose()
        {
            foreach (var layer in _layers)
                layer.Dispose();
        }

        internal void AddTileset(TiledMapTileset tileset)
        {
            _tilesets.Add(tileset);
        }

        public void AddLayer(TiledMapLayer layer)
        {
            _layers.Add(layer);
            _layersByName.Add(layer.Name, layer);

            var imageLayer = layer as TiledMapImageLayer;
            if (imageLayer != null)
                _imageLayers.Add(imageLayer);

            var tileLayer = layer as TiledMapTileLayer;
            if (tileLayer != null)
                _tileLayers.Add(tileLayer);

            var objectLayer = layer as TiledMapObjectLayer;
            if (objectLayer != null)
                _objectLayers.Add(objectLayer);
        }

        public TiledMapLayer GetLayer(string layerName)
        {
            TiledMapLayer layer;
            _layersByName.TryGetValue(layerName, out layer);
            return layer;
        }

        public T GetLayer<T>(string layerName)
            where T : TiledMapLayer
        {
            return GetLayer(layerName) as T;
        }

        public TiledMapTileset GetTilesetByTileGlobalIdentifier(int tileIdentifier)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var tileset in _tilesets)
                if (tileset.ContainsGlobalIdentifier(tileIdentifier))
                    return tileset;
            return null;
        }
    }
}