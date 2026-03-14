// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

[Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
public enum TiledMapTileDrawOrderContent : byte
{
    [XmlEnum(Name = "right-down")] RightDown,
    [XmlEnum(Name = "right-up")] RightUp,
    [XmlEnum(Name = "left-down")] LeftDown,
    [XmlEnum(Name = "left-up")] LeftUp
}
