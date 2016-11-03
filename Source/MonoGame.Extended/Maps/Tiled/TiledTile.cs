using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTile
    {
        public TiledTile(int id, int x, int y, TiledTilesetTile tilesetTile = null)
        {
            Id = id;
            X = x;
            Y = y;
            TilesetTile = tilesetTile;
        }

        public int Id { get; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public TiledTilesetTile TilesetTile { get; set; }
        public int CurrentTileId
        {
            // Need to do the +1 here because of the way Tiled indexes TileSet Tiles in the TileLayer data
            get { return (TilesetTile == null || !TilesetTile.CurrentTileId.HasValue ? Id : TilesetTile.CurrentTileId.Value + 1); }
        }

        public bool HasAnimation
        {
            get { return (TilesetTile == null || TilesetTile.Animation.Count == 0) ? false : true; }
        }
        public override string ToString()
        {
            return $"{Id}";
        }
    }
}