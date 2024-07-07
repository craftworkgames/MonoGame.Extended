// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Xml.Serialization;
using MonoGame.Extended.Content.Tiled;

namespace MonoGame.Extended.Content.Tiled;

// This content class is going to be a lot more complex than the others we use.
// Objects can reference a template file which has starting values for the
// object. The value in the object file overrides any value specified in the
// template. All values have to be able to store a null value so we know if the
// XML parser actually found a value for the property and not just a default
// value. Default values are used when the object and any templates don't 
// specify a value.
public class TiledMapObjectContent
{
    // TODO: HACK These shouldn't be public
    public uint? _globalIdentifier;
    public int? _identifier;
    public float? _height;
    public float? _rotation;
    public bool? _visible;
    public float? _width;
    public float? _x;
    public float? _y;

    [XmlAttribute(DataType = "int", AttributeName = "id")]
    public int Identifier { get => _identifier ?? 0; set => _identifier = value; }

    [XmlAttribute(DataType = "string", AttributeName = "name")]
    public string Name { get; set; }

    // Deprecated as of Tiled 1.9.0 (replaced by "class" attribute)
    [XmlAttribute(DataType = "string", AttributeName = "type")]
    public string Type { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "class")]
    public string Class { get; set; }

    [XmlAttribute(DataType = "float", AttributeName = "x")]
    public float X { get => _x ?? 0; set => _x = value; }

    [XmlAttribute(DataType = "float", AttributeName = "y")]
    public float Y { get => _y ?? 0; set => _y = value; }

    [XmlAttribute(DataType = "float", AttributeName = "width")]
    public float Width { get => _width ?? 0; set => _width = value; }

    [XmlAttribute(DataType = "float", AttributeName = "height")]
    public float Height { get => _height ?? 0; set => _height = value; }

    [XmlAttribute(DataType = "float", AttributeName = "rotation")]
    public float Rotation { get => _rotation ?? 0; set => _rotation = value; }

    [XmlAttribute(DataType = "boolean", AttributeName = "visible")]
    public bool Visible { get => _visible ?? true; set => _visible = value; }

    [XmlArray("properties")]
    [XmlArrayItem("property")]
    public List<TiledMapPropertyContent> Properties { get; set; }

    [XmlAttribute(DataType = "unsignedInt", AttributeName = "gid")]
    public uint GlobalIdentifier { get => _globalIdentifier ?? 0; set => _globalIdentifier = value; }

    [XmlElement(ElementName = "ellipse")]
    public TiledMapEllipseContent Ellipse { get; set; }

    [XmlElement(ElementName = "polygon")]
    public TiledMapPolygonContent Polygon { get; set; }

    [XmlElement(ElementName = "polyline")]
    public TiledMapPolylineContent Polyline { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "template")]
    public string TemplateSource { get; set; }


}
