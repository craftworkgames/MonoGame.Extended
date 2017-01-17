using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public enum TiledMapObjectDrawOrderContent : byte
    {
        [XmlEnum(Name = "topdown")] TopDown,
        [XmlEnum(Name = "index")] Manual
    }
}