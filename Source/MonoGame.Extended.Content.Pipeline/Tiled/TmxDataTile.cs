using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxDataTile
    {
        public TmxDataTile()
        {
        }

        public override string ToString()
        {
            return Gid.ToString();
        }

        [XmlAttribute(AttributeName = "gid")]
        public int Gid { get; set; }
    }
}