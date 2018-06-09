using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
	public enum TiledMapStaggerIndexContent : byte
	{
		[XmlEnum("even")]Even,
		[XmlEnum("odd")]Odd
	}
}