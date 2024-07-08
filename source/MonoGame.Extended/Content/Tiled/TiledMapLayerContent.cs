// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Xml.Serialization;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Tiled;

[XmlInclude(typeof(TiledMapTileLayerContent))]
[XmlInclude(typeof(TiledMapImageLayerContent))]
[XmlInclude(typeof(TiledMapObjectLayerContent))]
public abstract class TiledMapLayerContent
{
    protected TiledMapLayerContent(TiledMapLayerType layerType)
    {
        LayerType = layerType;
        Opacity = 1.0f;
        ParallaxX = 1.0f;
        ParallaxY = 1.0f;
        Visible = true;
        Properties = new List<TiledMapPropertyContent>();
    }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    // Deprecated as of Tiled 1.9.0 (replaced by "class" attribute)
    [XmlAttribute(DataType = "string", AttributeName = "type")]
    public string Type { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "class")]
    public string Class { get; set; }

    [XmlAttribute(AttributeName = "opacity")]
    public float Opacity { get; set; }

    [XmlAttribute(AttributeName = "visible")]
    public bool Visible { get; set; }

    [XmlAttribute(AttributeName = "offsetx")]
    public float OffsetX { get; set; }

    [XmlAttribute(AttributeName = "offsety")]
    public float OffsetY { get; set; }

    [XmlAttribute(AttributeName = "parallaxx")]
    public float ParallaxX { get; set; }

    [XmlAttribute(AttributeName = "parallaxy")]
    public float ParallaxY { get; set; }

    [XmlArray("properties")]
    [XmlArrayItem("property")]
    public List<TiledMapPropertyContent> Properties { get; set; }

    [XmlIgnore]
    public TiledMapLayerType LayerType { get; }

    public override string ToString()
    {
        return Name;
    }
}
