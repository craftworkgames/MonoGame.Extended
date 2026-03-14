// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

[Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
public class TiledMapTilesetTileContent
{
    public TiledMapTilesetTileContent()
    {
        Properties = new List<TiledMapPropertyContent>();
        Type = string.Empty;
    }

    [XmlAttribute(AttributeName = "id")]
    public int LocalIdentifier { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }

    [XmlElement(ElementName = "image")]
    public TiledMapImageContent Image { get; set; }

    [XmlArray("objectgroup")]
    [XmlArrayItem("object")]
    public List<TiledMapObjectContent> Objects { get; set; }

    [XmlArray("animation")]
    [XmlArrayItem("frame")]
    public List<TiledMapTilesetTileAnimationFrameContent> Frames { get; set; }

    [XmlArray("properties")]
    [XmlArrayItem("property")]
    public List<TiledMapPropertyContent> Properties { get; set; }
    
    public override string ToString()
    {
        return LocalIdentifier.ToString();
    }
}
