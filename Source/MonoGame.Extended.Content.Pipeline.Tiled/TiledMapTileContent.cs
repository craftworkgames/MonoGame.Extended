using System.Xml.Serialization;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public struct TiledMapTileContent
    {
        [XmlAttribute(AttributeName = "gid")] public uint GlobalIdentifier;

        private const uint _flippedHorizontallyFlag = 0x80000000;
        private const uint _flippedVerticallyFlag = 0x40000000;
        private const uint _flippedDiagonallyFlag = 0x20000000;
        private const uint _allFlippedFlags = _flippedHorizontallyFlag | _flippedVerticallyFlag | _flippedDiagonallyFlag;

        public int GlobalIdentifierWithoutFlags => (int)(GlobalIdentifier & ~_allFlippedFlags);
        public bool IsFlippedHorizontally => (GlobalIdentifier & _flippedHorizontallyFlag) != 0;
        public bool IsFlippedVertically => (GlobalIdentifier & _flippedVerticallyFlag) != 0;
        public bool IsFlippedDiagonally => (GlobalIdentifier & _flippedDiagonallyFlag) != 0;
        public bool IsBlank => GlobalIdentifier == 0;
        public FlipFlags Flags => (FlipFlags)((GlobalIdentifier & _allFlippedFlags) >> 29);

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