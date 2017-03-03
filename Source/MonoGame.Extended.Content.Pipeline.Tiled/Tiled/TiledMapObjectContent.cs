using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapObjectContent
    {
        [XmlAttribute(DataType = "int", AttributeName = "id")]
        public int Identifier { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(DataType = "float", AttributeName = "x")]
        public float X { get; set; }

        [XmlAttribute(DataType = "float", AttributeName = "y")]
        public float Y { get; set; }

        [XmlAttribute(DataType = "float", AttributeName = "width")]
        public float Width { get; set; }

        [XmlAttribute(DataType = "float", AttributeName = "height")]
        public float Height { get; set; }

        [XmlAttribute(DataType = "float", AttributeName = "rotation")]
        public float Rotation { get; set; }

        [XmlAttribute(DataType = "boolean", AttributeName = "visible")]
        public bool Visible { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<TiledMapPropertyContent> Properties { get; set; }

        [XmlAttribute(DataType = "unsignedInt", AttributeName = "gid")]
        public uint GlobalIdentifier { get; set; }

        [XmlElement(ElementName = "ellipse")]
        public TiledMapEllipseContent Ellipse { get; set; }

        [XmlElement(ElementName = "polygon")]
        public TiledMapPolygonContent Polygon { get; set; }

        [XmlElement(ElementName = "polyline")]
        public TiledMapPolylineContent Polyline { get; set; }

        public TiledMapObjectContent()
        {
            GlobalIdentifier = 0;
            Visible = true;
        }
    }
}