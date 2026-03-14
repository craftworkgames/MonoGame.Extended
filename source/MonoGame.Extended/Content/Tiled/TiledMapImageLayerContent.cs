// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Tiled;

[Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
public class TiledMapImageLayerContent : TiledMapLayerContent
{
    [XmlAttribute(AttributeName = "x")]
    public int X { get; set; }

    [XmlAttribute(AttributeName = "y")]
    public int Y { get; set; }

    [XmlElement(ElementName = "image")]
    public TiledMapImageContent Image { get; set; }

    public TiledMapImageLayerContent()
        : base(TiledMapLayerType.ImageLayer)
    {
        Opacity = 1.0f;
        Visible = true;
        Properties = new List<TiledMapPropertyContent>();
    }

    public override string ToString()
    {
        return $"{Name}: {Image}";
    }
}
