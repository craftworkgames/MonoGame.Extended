using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public sealed class TiledMap
    {
        private readonly List<TiledMapImageLayer> _imageLayers = new List<TiledMapImageLayer>();
        private readonly List<TiledMapLayer> _layers = new List<TiledMapLayer>();
        private readonly Dictionary<string, TiledMapLayer> _layersByName = new Dictionary<string, TiledMapLayer>();
        private readonly List<TiledMapObjectLayer> _objectLayers = new List<TiledMapObjectLayer>();
        private readonly List<TiledMapTileLayer> _tileLayers = new List<TiledMapTileLayer>();
        private readonly List<TiledMapTileset> _tilesets = new List<TiledMapTileset>();
		private readonly List<Tuple<TiledMapTileset, int>> _firstGlobalIdentifiers = new List<Tuple<TiledMapTileset, int>>();

        public string Name { get; }
        public string Type { get; }
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
            Layers = new ReadOnlyCollection<TiledMapLayer>(_layers);
            ImageLayers = new ReadOnlyCollection<TiledMapImageLayer>(_imageLayers);
            TileLayers = new ReadOnlyCollection<TiledMapTileLayer>(_tileLayers);
            ObjectLayers = new ReadOnlyCollection<TiledMapObjectLayer>(_objectLayers);
            Tilesets = new ReadOnlyCollection<TiledMapTileset>(_tilesets);
            Properties = new TiledMapProperties();
        }

        public TiledMap(string name, string type, int width, int height, int tileWidth, int tileHeight, TiledMapTileDrawOrder renderOrder, TiledMapOrientation orientation, Color? backgroundColor = null)
            : this()
        {
            Name = name;
            Type = type;
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            RenderOrder = renderOrder;
            Orientation = orientation;
            BackgroundColor = backgroundColor;
        }

        public void AddTileset(TiledMapTileset tileset, int firstGlobalIdentifier)
        {
            _tilesets.Add(tileset);
			_firstGlobalIdentifiers.Add(new Tuple<TiledMapTileset, int>(tileset, firstGlobalIdentifier));
        }

		public void AddLayer(TiledMapLayer layer)
			=> AddLayer(layer, true);

		private void AddLayer(TiledMapLayer layer, bool root)
        {
			if (root) _layers.Add(layer);
			
			if (_layersByName.ContainsKey(layer.Name))
				throw new ArgumentException($"The TiledMap '{Name}' contains two or more layers named '{layer.Name}'. Please ensure all layers have unique names.");

			_layersByName.Add(layer.Name, layer);

			switch(layer)
			{
				case TiledMapImageLayer imageLayer:
					_imageLayers.Add(imageLayer);
					break;
				case TiledMapTileLayer tileLayer:
					_tileLayers.Add(tileLayer);
					break;
				case TiledMapObjectLayer objectLayer:
					_objectLayers.Add(objectLayer);
					break;
				case TiledMapGroupLayer groupLayer:
					foreach (var subLayer in groupLayer.Layers)
						AddLayer(subLayer, false);
					break;
			}
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
			foreach (var tileset in _firstGlobalIdentifiers)
			{
			    if (tileIdentifier >= tileset.Item2 && tileIdentifier < tileset.Item2 + tileset.Item1.TileCount)
			        return tileset.Item1;
			}

            return null;
        }

		public int GetTilesetFirstGlobalIdentifier(TiledMapTileset tileset)
		{
			return _firstGlobalIdentifiers.FirstOrDefault(t => t.Item1 == tileset).Item2;
		}

		private static int CountLayers(TiledMapLayer layer)
		{
			var value = 0;
			if (layer is TiledMapGroupLayer groupLayer)
				foreach (var subLayer in groupLayer.Layers)
					value += CountLayers(subLayer);
			else
				value = 1;

			return value;
		}
    }
}
