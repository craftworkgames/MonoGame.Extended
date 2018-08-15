using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
	// This content class is going to be a lot more complex than the others we use.
	// Objects can reference a template file which has starting values for the
	// object. The value in the object file overrides any value specified in the
	// template. All values have to be able to store a null value so we know if the
	// XML parser actually found a value for the property and not just a default
	// value. Default values are used when the object and any templates don't 
	// specify a value.
	public class TiledMapObjectContent
	{
		private uint? _globalIdentifier;
		private int? _identifier;
		private float? _height;
		private float? _rotation;
		private bool? _visible;
		private float? _width;
		private float? _x;
		private float? _y;

		[XmlAttribute(DataType = "int", AttributeName = "id")]
		public int Identifier { get => _identifier ?? 0; set => _identifier = value; }

		[XmlAttribute(DataType = "string", AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(DataType = "string", AttributeName = "type")]
		public string Type { get; set; }

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
		public uint GlobalIdentifier { get => _globalIdentifier??0; set => _globalIdentifier = value; }

		[XmlElement(ElementName = "ellipse")]
		public TiledMapEllipseContent Ellipse { get; set; }

		[XmlElement(ElementName = "polygon")]
		public TiledMapPolygonContent Polygon { get; set; }

		[XmlElement(ElementName = "polyline")]
		public TiledMapPolylineContent Polyline { get; set; }

		[XmlAttribute(DataType = "string", AttributeName = "template")]
		public string TemplateSource { get; set; }

		internal static void Process(TiledMapObjectContent obj, ContentProcessorContext context)
		{
			if (!string.IsNullOrWhiteSpace(obj.TemplateSource))
			{
				var template = context.BuildAndLoadAsset<TiledMapObjectLayerContent, TiledMapObjectTemplateContent>(new ExternalReference<TiledMapObjectLayerContent>(obj.TemplateSource), "");

				// Nothing says a template can't reference another template.
				// Yay recusion!
				Process(template.Object, context);

				if (!obj._globalIdentifier.HasValue && template.Object._globalIdentifier.HasValue)
					obj.GlobalIdentifier = template.Object.GlobalIdentifier;
				if (!obj._height.HasValue && template.Object._height.HasValue)
					obj.Height = template.Object.Height;
				if (!obj._identifier.HasValue && template.Object._identifier.HasValue)
					obj.Identifier = template.Object.Identifier;
				if (!obj._rotation.HasValue && template.Object._rotation.HasValue)
					obj.Rotation = template.Object.Rotation;
				if (!obj._visible.HasValue && template.Object._visible.HasValue)
					obj.Visible = template.Object.Visible;
				if (!obj._width.HasValue && template.Object._width.HasValue)
					obj.Width = template.Object.Width;
				if (!obj._x.HasValue && template.Object._x.HasValue)
					obj.X = template.Object.X;
				if (!obj._y.HasValue && template.Object._y.HasValue)
					obj.Y = template.Object.Y;
				if (obj.Ellipse == null && template.Object.Ellipse != null)
					obj.Ellipse = template.Object.Ellipse;
				if (String.IsNullOrWhiteSpace(obj.Name) && !String.IsNullOrWhiteSpace(template.Object.Name))
					obj.Name = template.Object.Name;
				if (obj.Polygon == null && template.Object.Polygon != null)
					obj.Polygon = template.Object.Polygon;
				if (obj.Polyline == null && template.Object.Polyline != null)
					obj.Polyline = template.Object.Polyline;
				foreach (var tProperty in template.Object.Properties)
					if (!obj.Properties.Exists(p => p.Name == tProperty.Name))
						obj.Properties.Add(tProperty);
				if (String.IsNullOrWhiteSpace(obj.Type) && !String.IsNullOrWhiteSpace(template.Object.Type))
					obj.Type = template.Object.Type;
			}
		}
	}
}