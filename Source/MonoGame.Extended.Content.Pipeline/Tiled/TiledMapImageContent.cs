using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapImageContent
    {
		[XmlIgnore]
		public ExternalReference<Texture2DContent> Content;

        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }

        [XmlAttribute(AttributeName = "format")]
        public string Format { get; set; }

        [XmlAttribute(AttributeName = "trans")]
        public string TransparentColor { get; set; }

        [XmlElement(ElementName = "data")]
        public TiledMapTileLayerDataContent Data { get; set; }

        public override string ToString()
        {
            return Source;
        }
    }
}