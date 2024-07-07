// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Content.Tiled;

public class TiledMapImageContent
{
    //[XmlIgnore]
    //public Texture2DContent Content { get; set; }

    //[XmlIgnore]
    //public ExternalReference<Texture2DContent> ContentRef { get; set; }

    [XmlAttribute(AttributeName = "source")]
    public string Source { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public int Width { get; set; }

    [XmlAttribute(AttributeName = "height")]
    public int Height { get; set; }

    [XmlAttribute(AttributeName = "format")]
    public string Format { get; set; }

    [XmlAttribute(AttributeName = "trans")]
    public string RawTransparentColor { get; set; } = string.Empty;

    [XmlIgnore]
    public Color TransparentColor
    {
        get => RawTransparentColor == string.Empty ? Color.Transparent : ColorHelper.FromHex(RawTransparentColor);
        set => RawTransparentColor = ColorHelper.ToHex(value);
    }

    [XmlElement(ElementName = "data")]
    public TiledMapTileLayerDataContent Data { get; set; }

    public override string ToString()
    {
        return Source;
    }
}
