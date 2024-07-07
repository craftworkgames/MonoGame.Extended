using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MonoGame.Extended.Tiled.Renderers
{
    public class TiledMapModel : IDisposable
    {
        private readonly TiledMap _map;
        private readonly Dictionary<TiledMapTileset, List<TiledMapTilesetAnimatedTile>> _animatedTilesByTileset;

        public TiledMapModel(TiledMap map, Dictionary<TiledMapLayer, TiledMapLayerModel[]> layersOfLayerModels)
        {
            _map = map;
            LayersOfLayerModels = layersOfLayerModels;
            _animatedTilesByTileset = _map.Tilesets
                .ToDictionary(i => i, i => i.Tiles.OfType<TiledMapTilesetAnimatedTile>()
                .ToList());
        }

        public void Dispose()
        {
			foreach (var layerModel in LayersOfLayerModels)
				foreach (var model in layerModel.Value)
					model.Dispose();
        }

        public ReadOnlyCollection<TiledMapTileset> Tilesets => _map.Tilesets;
        public ReadOnlyCollection<TiledMapLayer> Layers => _map.Layers;

        // each layer has many models
        public Dictionary<TiledMapLayer, TiledMapLayerModel[]> LayersOfLayerModels { get; }

        public IEnumerable<TiledMapTilesetAnimatedTile> GetAnimatedTiles(int tilesetIndex)
        {
            var tileset = _map.Tilesets[tilesetIndex];
            return _animatedTilesByTileset[tileset];
        }
    }
}