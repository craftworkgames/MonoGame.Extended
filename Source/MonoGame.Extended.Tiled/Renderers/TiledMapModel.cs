using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MonoGame.Extended.Tiled.Renderers
{
    public class TiledMapModel
    {
        private readonly TiledMap _map;
        private readonly Dictionary<TiledMapTileset, List<TiledMapTilesetAnimatedTile>> _animatedTilesByTileset;

        public TiledMapModel(TiledMap map, TiledMapLayerModel[][] layersOfLayerModels)
        {
            _map = map;
            LayersOfLayerModels = layersOfLayerModels;
            _animatedTilesByTileset = _map.Tilesets
                .ToDictionary(i => i, i => i.Tiles.OfType<TiledMapTilesetAnimatedTile>()
                .ToList());
        }

        public ReadOnlyCollection<TiledMapTileset> Tilesets => _map.Tilesets;
        public ReadOnlyCollection<TiledMapLayer> Layers => _map.Layers;

        // each layer has many models
        public TiledMapLayerModel[][] LayersOfLayerModels { get; }

        public IEnumerable<TiledMapTilesetAnimatedTile> GetAnimatedTiles(int tilesetIndex)
        {
            var tileset = _map.Tilesets[tilesetIndex];
            return _animatedTilesByTileset[tileset];
        }
    }
}