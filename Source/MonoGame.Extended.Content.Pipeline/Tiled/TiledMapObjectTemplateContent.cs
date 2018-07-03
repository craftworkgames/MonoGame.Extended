using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
	[XmlRoot(ElementName = "template")]
    public class TiledMapObjectTemplateContent
    {
		[XmlElement(ElementName = "tileset")]
		public TiledMapTilesetContent Tileset { get; set; }

		[XmlIgnore]
		public ExternalReference<TiledMapTilesetContent> TilesetReference { get; set; }

		[XmlElement(ElementName = "object")]
		public TiledMapObjectContent Object { get; set; }
    }
}
