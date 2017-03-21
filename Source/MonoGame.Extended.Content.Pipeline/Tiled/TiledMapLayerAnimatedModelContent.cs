using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapLayerAnimatedModelContent : TiledMapLayerModelContent
    {
        private readonly List<TiledMapTilesetTileContent> _animatedTilesetTiles;

        public ReadOnlyCollection<TiledMapTilesetTileContent> AnimatedTilesetTiles { get; }
        public TiledMapTilesetContent Tileset { get; }

        public TiledMapLayerAnimatedModelContent(string layerName, TiledMapTilesetContent tileset)
            : base(layerName, tileset)
        {
            _animatedTilesetTiles = new List<TiledMapTilesetTileContent>();
            AnimatedTilesetTiles = new ReadOnlyCollection<TiledMapTilesetTileContent>(_animatedTilesetTiles);
            Tileset = tileset;
        }

        internal void AddAnimatedTile(TiledMapTilesetTileContent tile)
        {
            _animatedTilesetTiles.Add(tile);
        }
    }
}
