using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapTileset
    {
        public TiledMapTileset(Texture2D texture, int tileWidth, int tileHeight, int tileCount, int spacing, int margin, int columns)
        {
            Texture = texture;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            TileCount = tileCount;
            Spacing = spacing;
            Margin = margin;
            Columns = columns;
            Properties = new TiledMapProperties();
            Tiles = new List<TiledMapTilesetTile>();
        }

        public string Name => Texture.Name;
        public Texture2D Texture { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public int Spacing { get; }
        public int Margin { get; }
        public int TileCount { get; }
        public int Columns { get; }
        public List<TiledMapTilesetTile> Tiles { get; }
        public TiledMapProperties Properties { get; }

        public int Rows => (int)Math.Ceiling((double) TileCount / Columns);
        public int ActualWidth => TileWidth * Columns;
        public int ActualHeight => TileHeight * Rows;

        public Rectangle GetTileRegion(int localTileIdentifier)
        {
            return TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, TileWidth, TileHeight, Columns, Margin, Spacing);
        }
    }
}