using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxFrame
    {
        public TmxFrame() { }

        public override string ToString()
        {
            return $"TileID: {TileId} Duration: {Duration}";
        }

        [XmlAttribute(AttributeName = "tileid")]
        public int TileId { get; set; }

        [XmlAttribute(AttributeName = "duration")]
        public int Duration { get; set; }
    }
}
