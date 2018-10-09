using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
    public enum TiledMapOrientationContent : byte
    {
        [XmlEnum(Name = "orthogonal")] Orthogonal,
        [XmlEnum(Name = "isometric")] Isometric,
        [XmlEnum(Name = "staggered")] Staggered,
		[XmlEnum(Name = "hexagonal")] Hexagonal
    }
}