using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
	public enum TiledMapStaggerAxisContent : byte
	{
		[XmlEnum("x")]X,
		[XmlEnum("y")]Y
	}
}