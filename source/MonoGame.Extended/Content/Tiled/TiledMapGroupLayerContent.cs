// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Tiled;

[Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
public class TiledMapGroupLayerContent : TiledMapLayerContent
{
    protected TiledMapGroupLayerContent()
        : base(TiledMapLayerType.GroupLayer)
    {
    }

    [XmlElement(ElementName = "layer", Type = typeof(TiledMapTileLayerContent))]
    [XmlElement(ElementName = "imagelayer", Type = typeof(TiledMapImageLayerContent))]
    [XmlElement(ElementName = "objectgroup", Type = typeof(TiledMapObjectLayerContent))]
    [XmlElement(ElementName = "group", Type = typeof(TiledMapGroupLayerContent))]
    public List<TiledMapLayerContent> Layers { get; set; }

}
