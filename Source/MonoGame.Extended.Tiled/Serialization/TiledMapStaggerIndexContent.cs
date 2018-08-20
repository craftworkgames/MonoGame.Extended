using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
	public enum TiledMapStaggerIndexContent : byte
	{
		[XmlEnum("even")]Even,
		[XmlEnum("odd")]Odd
	}
}