// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

public class TiledMapTileLayerDataContent
{
    public TiledMapTileLayerDataContent()
    {
        Tiles = new List<TiledMapTileContent>();
    }

    [XmlAttribute(AttributeName = "encoding")]
    public string Encoding { get; set; }

    [XmlAttribute(AttributeName = "compression")]
    public string Compression { get; set; }

    [XmlElement(ElementName = "tile")]
    public List<TiledMapTileContent> Tiles { get; set; }

    [XmlElement(ElementName = "chunk")]
    public List<TiledMapTileLayerDataChunkContent> Chunks { get; set; }

    [XmlText]
    public string Value { get; set; }

    public override string ToString()
    {
        return $"{Encoding} {Compression}";
    }
}
