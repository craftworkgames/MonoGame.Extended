// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

[Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
public class TiledMapPropertyContent
{
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "value")]
    public string ValueAttribute { get; set; }

    [XmlText]
    public string ValueBody { get; set; }

    [XmlArray("properties")]
    [XmlArrayItem("property")]
    public List<TiledMapPropertyContent> Properties { get; set; }

    public string Value => ValueAttribute ?? ValueBody;

    public override string ToString()
    {
        return $"{Name}: {Value}";
    }
}
