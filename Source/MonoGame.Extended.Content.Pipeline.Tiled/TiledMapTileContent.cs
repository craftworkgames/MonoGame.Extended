#region

using System.Xml.Serialization;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;

#endregion

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public struct TiledMapTileContent
    {
        [XmlAttribute(AttributeName = "gid")] public uint GlobalIdentifier;

        private const uint FlippedHorizontallyFlag = 0x80000000;
        private const uint FlippedVerticallyFlag = 0x40000000;
        private const uint FlippedDiagonallyFlag = 0x20000000;
        private const uint AllFlippedFlags = FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag;

        public int GlobalIdentifierWithoutFlags => (int)(GlobalIdentifier & ~AllFlippedFlags);
        public bool IsFlippedHorizontally => (GlobalIdentifier & FlippedHorizontallyFlag) != 0;
        public bool IsFlippedVertically => (GlobalIdentifier & FlippedVerticallyFlag) != 0;
        public bool IsFlippedDiagonally => (GlobalIdentifier & FlippedDiagonallyFlag) != 0;
        public bool IsBlank => GlobalIdentifier == 0;
        public FlipFlags Flags => (FlipFlags)((GlobalIdentifier & AllFlippedFlags) >> 29);

        internal TiledMapTileContent(uint globalTileIdentifierWithFlags)
        {
            GlobalIdentifier = globalTileIdentifierWithFlags;
        }

        public override string ToString()
        {
            return $"GlobalIdentifier: {GlobalIdentifierWithoutFlags}, Flags: {Flags}";
        }
    }
}