using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
	public class TiledMapTilesetGridContent
	{
		[XmlAttribute(AttributeName = "orientation")]
		public TiledMapOrientationContent Orientation { get; set; }

		[XmlAttribute(AttributeName = "width")]
		public int Width { get; set; }

		[XmlAttribute(AttributeName = "height")]
		public int Height { get; set; }
	}
}