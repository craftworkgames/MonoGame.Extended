using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public struct TiledMapTileContent
    {
        [XmlAttribute(AttributeName = "gid")] public uint GlobalIdentifier;
    }
}