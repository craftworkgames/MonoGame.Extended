using System.Diagnostics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Tiled
{
    public struct TiledMapTile
    {
        internal readonly uint GlobalTileIdentifierWithFlags;

        public readonly ushort X;
        public readonly ushort Y;

        private const uint FlippedHorizontallyFlag = 0x80000000;
        private const uint FlippedVerticallyFlag = 0x40000000;
        private const uint FlippedDiagonallyFlag = 0x20000000;
        private const uint AllFlippedFlags = FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag;

        public int GlobalIdentifier => (int)(GlobalTileIdentifierWithFlags & ~AllFlippedFlags);
        public bool IsFlippedHorizontally => (GlobalTileIdentifierWithFlags & FlippedHorizontallyFlag) != 0;
        public bool IsFlippedVertically => (GlobalTileIdentifierWithFlags & FlippedVerticallyFlag) != 0;
        public bool IsFlippedDiagonally => (GlobalTileIdentifierWithFlags & FlippedDiagonallyFlag) != 0;
        public bool IsBlank => GlobalIdentifier == 0;
        public FlipFlags Flags => (FlipFlags)((GlobalTileIdentifierWithFlags & AllFlippedFlags) >> 29);

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