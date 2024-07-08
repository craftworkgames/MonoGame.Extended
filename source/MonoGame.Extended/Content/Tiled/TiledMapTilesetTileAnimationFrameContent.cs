// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

public class TiledMapTilesetTileAnimationFrameContent
{
    [XmlAttribute(AttributeName = "tileid")]
    public int TileIdentifier { get; set; }

    [XmlAttribute(AttributeName = "duration")]
    public int Duration { get; set; }

    public override string ToString()
    {
        return $"TileID: {TileIdentifier}, Duration: {Duration}";
    }
}
