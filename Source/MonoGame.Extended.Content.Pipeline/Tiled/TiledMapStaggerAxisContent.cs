using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
	public enum TiledMapStaggerAxisContent : byte
	{
		[XmlEnum("x")]X,
		[XmlEnum("y")]Y
	}
}