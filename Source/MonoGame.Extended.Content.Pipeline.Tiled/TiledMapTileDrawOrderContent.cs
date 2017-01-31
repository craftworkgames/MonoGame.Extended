using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public enum TiledMapTileDrawOrderContent : byte
    {
        [XmlEnum(Name = "right-down")] RightDown,
        [XmlEnum(Name = "right-up")] RightUp,
        [XmlEnum(Name = "left-down")] LeftDown,
        [XmlEnum(Name = "left-up")] LeftUp
    }
}