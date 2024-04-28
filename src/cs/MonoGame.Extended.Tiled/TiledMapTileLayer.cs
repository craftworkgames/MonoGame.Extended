using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapTileLayer : TiledMapLayer
    {
        public TiledMapTileLayer(string name, string type, int width, int height, int tileWidth, int tileHeight, Vector2? offset = null,
            Vector2? parallaxFactor = null, float opacity = 1, bool isVisible = true)
            : base(name, type, offset, parallaxFactor, opacity, isVisible)
        {
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Tiles = new TiledMapTile[Width * Height];
        }

        public int Width { get; }
        public int Height { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public TiledMapTile[] Tiles { get; }

        public int GetTileIndex(ushort x, ushort y)
        {
            return x + y * Width;
        }

        public bool TryGetTile(ushort x, ushort y, out TiledMapTile? tile)
        {
            if (x >= Width)
            {
                tile = null;
                return false;
            }
            var index = GetTileIndex(x, y);

            if (index < 0 || index >= Tiles.Length)
            {
                tile = null;
                return false;
            }

            tile = Tiles[index];
            return true;
        }

        public TiledMapTile GetTile(ushort x, ushort y)
        {
            var index = GetTileIndex(x, y);
            return Tiles[index];
        }

        public void SetTile(ushort x, ushort y, uint globalIdentifier)
        {
            var index = GetTileIndex(x, y);
            Tiles[index] = new TiledMapTile(globalIdentifier, x, y);
        }

        public void RemoveTile(ushort x, ushort y)
        {
            SetTile(x, y, 0);
        }
    }
}
