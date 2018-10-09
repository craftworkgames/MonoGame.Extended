using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public struct TiledMapTileContent
    {
        [XmlAttribute(AttributeName = "gid")] public uint GlobalIdentifier;
    }
}