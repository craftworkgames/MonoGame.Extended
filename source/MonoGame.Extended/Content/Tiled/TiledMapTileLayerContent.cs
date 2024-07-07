// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Xml.Serialization;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Tiled;

public class TiledMapTileLayerContent : TiledMapLayerContent
{
    public TiledMapTileLayerContent()
        : base(TiledMapLayerType.TileLayer)
    {
    }

    [XmlAttribute(AttributeName = "x")]
    public int X { get; set; }

    [XmlAttribute(AttributeName = "y")]
    public int Y { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public int Width { get; set; }

    [XmlAttribute(AttributeName = "height")]
    public int Height { get; set; }

    [XmlElement(ElementName = "data")]
    public TiledMapTileLayerDataContent Data { get; set; }

    [XmlIgnore]
    public TiledMapTile[] Tiles { get; set; }
}
