// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

[XmlRoot(ElementName = "tileset")]
public class TiledMapTilesetContent
{
    public TiledMapTilesetContent()
    {
        TileOffset = new TiledMapTileOffsetContent();
        Tiles = new List<TiledMapTilesetTileContent>();
        Properties = new List<TiledMapPropertyContent>();
    }

    [XmlAttribute(AttributeName = "firstgid")]
    public int FirstGlobalIdentifier { get; set; }

    [XmlAttribute(AttributeName = "source")]
    public string Source { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    // Deprecated as of Tiled 1.9.0 (replaced by "class" attribute)
    [XmlAttribute(DataType = "string", AttributeName = "type")]
    public string Type { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "class")]
    public string Class { get; set; }

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

    [XmlElement(ElementName = "grid")]
    public TiledMapTilesetGridContent Grid { get; set; }

    [XmlElement(ElementName = "tile")]
    public List<TiledMapTilesetTileContent> Tiles { get; set; }

    [XmlArray("properties")]
    [XmlArrayItem("property")]
    public List<TiledMapPropertyContent> Properties { get; set; }

    [XmlElement(ElementName = "image")]
    public TiledMapImageContent Image { get; set; }

    public bool ContainsGlobalIdentifier(int globalIdentifier)
    {
        return globalIdentifier >= FirstGlobalIdentifier && globalIdentifier < FirstGlobalIdentifier + TileCount;
    }

    public override string ToString()
    {
        return $"{Name}: {Image}";
    }
}
