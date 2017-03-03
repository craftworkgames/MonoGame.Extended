﻿using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public struct TiledMapTile
    {
        internal readonly uint GlobalTileIdentifierWithFlags;

        public readonly ushort X;
        public readonly ushort Y;

        public int GlobalIdentifier => (int)(GlobalTileIdentifierWithFlags & ~(uint)TiledMapTileFlipFlags.All);
        public bool IsFlippedHorizontally => (GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.FlipHorizontally) != 0;
        public bool IsFlippedVertically => (GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.FlipVertically) != 0;
        public bool IsFlippedDiagonally => (GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.FlipDiagonally) != 0;
        public bool IsBlank => GlobalIdentifier == 0;
        public TiledMapTileFlipFlags Flags => (TiledMapTileFlipFlags)(GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.All);

        public Rectangle GetBounds(int tileWidth, int tileHeight)
        {
            return new Rectangle(X * tileWidth, Y * tileHeight, tileWidth, tileHeight);
        }

        public Rectangle GetBounds(TiledMapTileLayer layer)
        {
            return GetBounds(layer.TileWidth, layer.TileHeight);
        }

        internal TiledMapTile(uint globalTileIdentifierWithFlags, ushort x, ushort y)
        {
            GlobalTileIdentifierWithFlags = globalTileIdentifierWithFlags;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"GlobalIdentifier: {GlobalIdentifier}, Flags: {Flags}";
        }
    }
}