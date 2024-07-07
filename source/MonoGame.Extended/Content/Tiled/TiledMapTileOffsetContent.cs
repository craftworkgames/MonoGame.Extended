// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

[XmlRoot(ElementName = "tileoffset")]
public class TiledMapTileOffsetContent
{
    [XmlAttribute(AttributeName = "x")] public int X;

    [XmlAttribute(AttributeName = "y")] public int Y;

    public override string ToString()
    {
        return $"{X}, {Y}";
    }
}
