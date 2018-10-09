using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
	public class TiledMapTileLayerDataChunkContent
	{
		[XmlAttribute(AttributeName = "x")]
		public int X { get; set; }

		[XmlAttribute(AttributeName = "y")]
		public int Y { get; set; }

		[XmlAttribute(AttributeName = "width")]
		public int Width { get; set; }

		[XmlAttribute(AttributeName = "height")]
		public int Height { get; set; }

		[XmlElement(ElementName = "tile")]
		public List<TiledMapTileContent> Tiles { get; set; }

		[XmlText]
		public string Value { get; set; }
	}
}