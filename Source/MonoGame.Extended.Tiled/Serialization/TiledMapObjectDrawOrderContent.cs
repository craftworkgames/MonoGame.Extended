using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public enum TiledMapObjectDrawOrderContent : byte
    {
        [XmlEnum(Name = "topdown")] TopDown,
        [XmlEnum(Name = "index")] Manual
    }
}