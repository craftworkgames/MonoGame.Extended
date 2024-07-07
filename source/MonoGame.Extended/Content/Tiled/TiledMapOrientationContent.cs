// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

public enum TiledMapOrientationContent : byte
{
    [XmlEnum(Name = "orthogonal")] Orthogonal,
    [XmlEnum(Name = "isometric")] Isometric,
    [XmlEnum(Name = "staggered")] Staggered,
    [XmlEnum(Name = "hexagonal")] Hexagonal
}
