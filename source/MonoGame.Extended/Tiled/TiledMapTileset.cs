using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Tiled
{
    public interface ITileset
    {
        int ActualWidth { get; }
        int Columns { get; }
        int ActualHeight { get; }
        int Rows { get; }
        int TileWidth { get; }
        int TileHeight { get; }
        Texture2D Texture { get; }
        Texture2DRegion GetRegion(int column, int row);
    }

    public class TiledMapTileset : ITileset
    {
        public TiledMapTileset(Texture2D texture, string type, int tileWidth, int tileHeight, int tileCount, int spacing, int margin, int columns)
        {
            Texture = texture;
            Type = type;
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

        public Texture2DRegion GetRegion(int column, int row)
        {
            var x = Margin + column * (TileWidth + Spacing);
            var y = Margin + row * (TileHeight + Spacing);
            return new Texture2DRegion(Texture, x, y, TileWidth, TileHeight);
        }

        public string Type { get; }
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
            return Texture is not null
                ? TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, TileWidth, TileHeight, Columns, Margin,
                    Spacing)
                : Tiles.FirstOrDefault(x => x.LocalTileIdentifier == localTileIdentifier).Texture.Bounds;
        }
    }
}
