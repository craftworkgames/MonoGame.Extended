using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public enum TiledMapOrientationContent : byte
    {
        [XmlEnum(Name = "orthogonal")] Orthogonal,
        [XmlEnum(Name = "isometric")] Isometric,
        [XmlEnum(Name = "staggered")] Staggered
    }
}