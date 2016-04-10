using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Shapes
{
    public static class PolygonCreator
    {
        public class PolygonSegment
        {
            internal PolygonSegment Previous { get; set; }
            public Vector2 StartPoint => Previous?.EndPoint ?? EndPoint;
            public Vector2 EndPoint { get; set; }

            public PolygonSegment(Vector2 endpoint) {
                EndPoint = endpoint;
            }
        }
        public class CubicBezierSegment : PolygonSegment
        {
            public Vector2 PointA;
            public Vector2 PointB;
            public CubicBezierSegment(Vector2 pointa, Vector2 pointb, Vector2 endpoint) : base(endpoint) {
                PointA = pointa;
                PointB = pointb;
                
            }
        }

        public class FluentPolygon
        {
            internal List<PolygonSegment> Segments = new List<PolygonSegment>();
            internal PolygonSegment Last => Segments[Segments.Count - 1];
            internal void Add(PolygonSegment segment) {
                segment.Previous = Last;
                Segments.Add(segment);
            }
        }
        //PolygonCreator.Create(new Vector2(0, 0))
        //            .AddSegment(new CubicBezierSegment(Vector2.One, Vector2.One, Vector2.UnitX))
        //            .AddLine(Vector2.UnitY)
        //            .Close();

        public static FluentPolygon Create(Vector2 startposition) {
            var result = new FluentPolygon();
            result.Segments.Add(new PolygonSegment(startposition));
            return result;
        }

        public static FluentPolygon AddLine(this FluentPolygon polygon, Vector2 point) {
            return AddSegment(polygon, new PolygonSegment(point));
        }
        public static FluentPolygon AddSegment(this FluentPolygon polygon, PolygonSegment segment) {
            polygon.Add(segment);
            return polygon;
        }
        public static PolygonF Close(this FluentPolygon polygon) {
            polygon.Segments[0].Previous = polygon.Last;
            return new PolygonF(polygon.Segments.Select(t=>t.EndPoint));
        }
        public static PolygonF Close(this FluentPolygon polygon, PolygonSegment endSegment) {
            polygon.Segments[0] = endSegment;
            endSegment.Previous = polygon.Last;
            return new PolygonF(polygon.Segments.Select(t => t.EndPoint));
        }


    }
}