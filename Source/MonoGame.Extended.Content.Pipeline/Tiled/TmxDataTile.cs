using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxDataTile
    {
        [XmlAttribute(AttributeName = "gid")]
        public int Gid { get; set; }

        public TmxDataTile()
        {
        }

        public TmxDataTile(int gid)
        {
            Gid = gid;
        }

        public override string ToString()
        {
            return Gid.ToString();
        }
    }
}