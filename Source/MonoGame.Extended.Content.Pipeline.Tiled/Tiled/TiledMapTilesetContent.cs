using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [XmlRoot(ElementName = "tileset")]
    public class TiledMapTilesetContent
    {
        [XmlAttribute(AttributeName = "firstgid")]
        public int FirstGlobalIdentifier { get; set; }

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

        [XmlAttribute(AttributeName = "columns")]
        public int Columns { get; set; }

        [XmlAttribute(AttributeName = "tilecount")]
        public int TileCount { get; set; }

        [XmlElement(ElementName = "tileoffset")]
        public TiledMapTileOffsetContent TileOffset { get; set; }

        [XmlElement(ElementName = "tile")]
        public List<TiledMapTilesetTileContent> Tiles { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TiledMapPropertyContent> Properties { get; set; }

        [XmlElement(ElementName = "image")]
        public TiledMapImageContent Image { get; set; }

        [XmlArray("terraintypes")]
        [XmlArrayItem("terrain")]
        public List<TiledMapTerrainContent> TerrainTypes { get; set; }

        public TiledMapTilesetContent()
        {
            TileOffset = new TiledMapTileOffsetContent();
            Tiles = new List<TiledMapTilesetTileContent>();
            Properties = new List<TiledMapPropertyContent>();
            TerrainTypes = new List<TiledMapTerrainContent>();
        }

        public bool ContainsGlobalIdentifier(int globalIdentifier)
        {
            return (globalIdentifier >= FirstGlobalIdentifier) && (globalIdentifier < FirstGlobalIdentifier + TileCount);
        }

        public override string ToString()
        {
            return $"{Name}: {Image}";
        }
    }
}