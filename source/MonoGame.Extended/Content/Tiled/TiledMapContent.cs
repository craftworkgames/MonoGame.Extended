// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

[XmlRoot(ElementName = "map")]
public class TiledMapContent
{
    public TiledMapContent()
    {
        Properties = new List<TiledMapPropertyContent>();
        Tilesets = new List<TiledMapTilesetContent>();
        Layers = new List<TiledMapLayerContent>();
    }

    [XmlIgnore]
    public string Name { get; set; }

    [XmlIgnore]
    public string FilePath { get; set; }

    // Deprecated as of Tiled 1.9.0 (replaced by "class" attribute)
    [XmlAttribute(DataType = "string", AttributeName = "type")]
    public string Type { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "class")]
    public string Class { get; set; }

    [XmlAttribute(AttributeName = "version")]
    public string Version { get; set; }

    [XmlAttribute(AttributeName = "orientation")]
    public TiledMapOrientationContent Orientation { get; set; }

    [XmlAttribute(AttributeName = "renderorder")]
    public TiledMapTileDrawOrderContent RenderOrder { get; set; }

    [XmlAttribute(AttributeName = "backgroundcolor")]
    public string BackgroundColor { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public int Width { get; set; }

    [XmlAttribute(AttributeName = "height")]
    public int Height { get; set; }

    [XmlAttribute(AttributeName = "tilewidth")]
    public int TileWidth { get; set; }

    [XmlAttribute(AttributeName = "tileheight")]
    public int TileHeight { get; set; }

    [XmlAttribute(AttributeName = "hexsidelength")]
    public int HexSideLength { get; set; }

    [XmlAttribute(AttributeName = "staggeraxis")]
    public TiledMapStaggerAxisContent StaggerAxis { get; set; }

    [XmlAttribute(AttributeName = "staggerindex")]
    public TiledMapStaggerIndexContent StaggerIndex { get; set; }

    [XmlElement(ElementName = "tileset")]
    public List<TiledMapTilesetContent> Tilesets { get; set; }

    [XmlElement(ElementName = "layer", Type = typeof(TiledMapTileLayerContent))]
    [XmlElement(ElementName = "imagelayer", Type = typeof(TiledMapImageLayerContent))]
    [XmlElement(ElementName = "objectgroup", Type = typeof(TiledMapObjectLayerContent))]
    [XmlElement(ElementName = "group", Type = typeof(TiledMapGroupLayerContent))]
    public List<TiledMapLayerContent> Layers { get; set; }

    [XmlArray("properties")]
    [XmlArrayItem("property")]
    public List<TiledMapPropertyContent> Properties { get; set; }
}
