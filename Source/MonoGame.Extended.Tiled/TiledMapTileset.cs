using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapTileset
    {
        public TiledMapTileset(Texture2D texture, int firstGlobalIdentifier, int tileWidth, int tileHeight, int tileCount, int spacing, int margin, int columns)
        {
            Texture = texture;
            FirstGlobalIdentifier = firstGlobalIdentifier;
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
        public int FirstGlobalIdentifier { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public int Spacing { get; }
        public int Margin { get; }
        public int TileCount { get; }
        public int Columns { get; }
        public List<TiledMapTilesetTile> Tiles { get; }
        public TiledMapProperties Properties { get; }

        public Rectangle GetTileRegion(int localTileIdentifier)
        {
            return TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, TileWidth, TileHeight, Columns, Margin, Spacing);
        }

        //public TiledMapTilesetAnimatedTile GetAnimatedTilesetTileByLocalTileIdentifier(int localTileIdentifier)
        //{
        //    throw new NotImplementedException();
        //    //TiledMapTilesetAnimatedTile animatedTile;
        //    //_animatedTilesByLocalTileIdentifier.TryGetValue(localTileIdentifier, out animatedTile);
        //    //return animatedTile;
        //}

        public bool ContainsGlobalIdentifier(int globalIdentifier)
        {
            return globalIdentifier >= FirstGlobalIdentifier && globalIdentifier < FirstGlobalIdentifier + TileCount;
        }
    }
}