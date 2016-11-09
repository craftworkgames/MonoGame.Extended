using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTileset
    {
        private readonly Dictionary<int, TextureRegion2D> _regions;
        private readonly List<TiledTilesetTile> _tiles;

        public TiledTileset(Texture2D texture, int firstId, int tileWidth, int tileHeight, int tileCount,
            int spacing = 2, int margin = 2)
        {
            Texture = texture;
            FirstId = firstId;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Spacing = spacing;
            Margin = margin;
            TileCount = tileCount;
            Properties = new TiledProperties();
            _tiles = new List<TiledTilesetTile>();

            var id = FirstId;
            _regions = new Dictionary<int, TextureRegion2D>();

            for (var y = Margin; y < texture.Height - Margin; y += TileHeight + Spacing)
                for (var x = Margin; x < texture.Width - Margin; x += TileWidth + Spacing)
                {
                    _regions.Add(id, new TextureRegion2D(Texture, x, y, TileWidth, TileHeight));
                    id++;
                }
        }

        public string Name => Texture.Name;
        public Texture2D Texture { get; }
        public int FirstId { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public int Spacing { get; }
        public int Margin { get; }
        public int TileCount { get; }
        public IReadOnlyList<TiledTilesetTile> Tiles => _tiles;
        public TiledProperties Properties { get; private set; }

        public TextureRegion2D GetTileRegion(int id)
        {
            return id == 0 ? null : _regions[id];
        }

        public TiledTilesetTile CreateTileSetTile(int id)
        {
            var tileSetTile = new TiledTilesetTile(id);
            _tiles.Add(tileSetTile);
            return tileSetTile;
        }

        public bool ContainsTileId(int tileId)
        {
            return (tileId >= FirstId) && (tileId < FirstId + TileCount);
        }
    }
}