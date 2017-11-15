using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapTileLayer : TiledMapLayer
    {
        public TiledMapTileLayer(string name, int width, int height, int tileWidth, int tileHeight, IList<TiledMapTile> tiles, 
            Vector2? offset = null, float opacity = 1, bool isVisible = true) 
            : base(name, offset, opacity, isVisible)
        {
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Tiles = new ReadOnlyCollection<TiledMapTile>(tiles);
        }

        public int Width { get; }
        public int Height { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public ReadOnlyCollection<TiledMapTile> Tiles { get; }

        public bool TryGetTile(int x, int y, out TiledMapTile? tile)
        {
            var index = x + y * Width;

            if (index < 0 || index >= Tiles.Count)
            {
                tile = null;
                return false;
            }

            tile = Tiles[index];
            return true;
        }
    }
}