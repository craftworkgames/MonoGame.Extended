// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

[XmlRoot(ElementName = "template")]
public class TiledMapObjectTemplateContent
{
    [XmlElement(ElementName = "tileset")]
    public TiledMapTilesetContent Tileset { get; set; }

    //[XmlIgnore]
    //public ExternalReference<TiledMapTilesetContent> TilesetReference { get; set; }

    [XmlElement(ElementName = "object")]
    public TiledMapObjectContent Object { get; set; }
}
