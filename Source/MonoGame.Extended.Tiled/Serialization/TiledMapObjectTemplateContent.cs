using System.Xml.Serialization;

namespace MonoGame.Extended.Tiled.Serialization
{
	[XmlRoot(ElementName = "template")]
    public class TiledMapObjectTemplateContent
    {
		[XmlElement(ElementName = "tileset")]
		public TiledMapTilesetContent Tileset { get; set; }

		//[XmlIgnore]
		//public ExternalReference<TiledMapTilesetContent> TilesetReference { get; set; }

		[XmlElement(ElementName = "object")]
		public TiledMapObjectContent Object { get; set; }
    }
}
