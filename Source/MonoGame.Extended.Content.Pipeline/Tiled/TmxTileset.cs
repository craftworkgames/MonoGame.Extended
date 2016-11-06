using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [XmlRoot(ElementName = "tileset")]
    public class TmxTileset
    {
        public TmxTileset()
        {
            TileOffset = new TmxTileOffset();
            Tiles = new List<TmxTile>();
            Properties = new List<TmxProperty>();
            TerrainTypes = new List<TmxTerrain>();

        }

        public override string ToString()
        {
            return $"{Name}: {Image}";
        }

        [XmlAttribute(AttributeName = "firstgid")]
        public int FirstGid { get; set; }

        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "tilewidth")]
        public int TileWidth { get; set; }

        [XmlAttribute(AttributeName = "tileheight")]
        public int TileHeight { get; set; }

        [XmlAttribute(AttributeName = "spacing")]
        public int Spacing { get; set; }

        [XmlAttribute(AttributeName = "margin")]
        public int Margin { get; set; }

        [XmlAttribute(AttributeName = "tilecount")]
        public int TileCount { get; set; }

        [XmlElement(ElementName = "tileoffset")]
        public TmxTileOffset TileOffset { get; set; }

        [XmlElement(ElementName = "tile")]
        public List<TmxTile> Tiles { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TmxProperty> Properties { get; set; }

        [XmlElement(ElementName = "image")]
        public TmxImage Image { get; set; }

        [XmlArray("terraintypes")]
        [XmlArrayItem("terrain")]
        public List<TmxTerrain> TerrainTypes { get; set; }
    }
}