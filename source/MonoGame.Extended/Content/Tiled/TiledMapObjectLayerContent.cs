// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Xml.Serialization;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Tiled;

public class TiledMapObjectLayerContent : TiledMapLayerContent
{
    public TiledMapObjectLayerContent()
        : base(TiledMapLayerType.ObjectLayer)
    {
        Objects = new List<TiledMapObjectContent>();
    }

    [XmlAttribute(AttributeName = "color")]
    public string Color { get; set; }

    [XmlElement(ElementName = "object")]
    public List<TiledMapObjectContent> Objects { get; set; }

    [XmlAttribute(AttributeName = "draworder")]
    public TiledMapObjectDrawOrderContent DrawOrder { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
