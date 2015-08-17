using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public enum TmxOrientation
    {
        [XmlEnum(Name = "orthogonal")] Orthogonal = 1,
        [XmlEnum(Name = "isometric")]  Isometric = 2,
        [XmlEnum(Name = "staggered")]  Staggered = 3
    }
}