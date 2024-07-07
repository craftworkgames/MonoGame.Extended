// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

public class TiledMapTilesetGridContent
{
    [XmlAttribute(AttributeName = "orientation")]
    public TiledMapOrientationContent Orientation { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public int Width { get; set; }

    [XmlAttribute(AttributeName = "height")]
    public int Height { get; set; }
}
