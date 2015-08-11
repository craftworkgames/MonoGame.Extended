using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TmxObject
    {
        [XmlAttribute(DataType = "int", AttributeName = "gid")]
        public int Gid { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(DataType = "int", AttributeName = "x")]
        public int X { get; set; }

        [XmlAttribute(DataType = "int", AttributeName = "y")]
        public int Y { get; set; }

        [XmlAttribute(DataType = "int", AttributeName = "width")]
        public int Width { get; set; }

        [XmlAttribute(DataType = "int", AttributeName = "height")]
        public int Height { get; set; }

        [XmlAttribute(DataType = "int", AttributeName = "rotation")]
        public int Rotation { get; set; }
        
        [XmlAttribute(DataType = "boolean", AttributeName = "visible")]
        public bool Visible { get; set; }

        [XmlElement(ElementName = "image")]
        public TmxImage Image { get; set; }

        //[XmlElement(ElementName = "ellipse")]
        //public TmxEllipse Ellipse { get; set; }

        //[XmlElement(ElementName = "polygon")]
        //public TmxPolygon Polygon { get; set; }

        //[XmlElement(ElementName = "polyline")]
        //public TmxPolyline Polyline { get; set; }
    }
}