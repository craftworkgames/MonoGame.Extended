namespace MonoGame.Extended.Tiled
{
    public struct TiledMapTile
    {
        public readonly ushort X;
        public readonly ushort Y;
        public readonly uint GlobalTileIdentifierWithFlags;

        public int GlobalIdentifier => (int)(GlobalTileIdentifierWithFlags & ~(uint)TiledMapTileFlipFlags.All);
        public bool IsFlippedHorizontally => (GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.FlipHorizontally) != 0;
        public bool IsFlippedVertically => (GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.FlipVertically) != 0;
        public bool IsFlippedDiagonally => (GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.FlipDiagonally) != 0;
        public bool IsBlank => GlobalIdentifier == 0;
        public TiledMapTileFlipFlags Flags => (TiledMapTileFlipFlags)(GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.All);

        public TiledMapTile(uint globalTileIdentifierWithFlags, ushort x, ushort y)
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