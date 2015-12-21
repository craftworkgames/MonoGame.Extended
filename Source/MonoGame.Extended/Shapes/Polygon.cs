using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public class Polygon
    {
        public Polygon(Vector2[] vertices)
        {
            Vertices = vertices;
        }

        public Vector2[] Vertices { get; private set; }

        public RectangleF BoundingRectangle
        {
            get
            {
                var minX = Vertices[0].X;
                var maxX = Vertices[0].X;
                var minY = Vertices[0].Y;
                var maxY = Vertices[0].Y;

                for (var i = 1; i < Vertices.Length; i++)
                {
                    var vertex = Vertices[i];
                    minX = Math.Min(vertex.X, minX);
                    maxX = Math.Max(vertex.X, maxX);
                    minY = Math.Min(vertex.Y, minY);
                    maxY = Math.Max(vertex.Y, maxY);
                }

                return new RectangleF(minX, minY, maxX - minX, maxY - minY);
            }
        }

        public bool Contains(Vector2 point)
        {
            if (!BoundingRectangle.Contains(point))
                return false;

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            var inside = false;

            for (int i = 0, j = Vertices.Length - 1; i < Vertices.Length; j = i++)
            {
                if ((Vertices[i].Y > point.Y) != (Vertices[j].Y > point.Y) &&
                     point.X < (Vertices[j].X - Vertices[i].X) * (point.Y - Vertices[i].Y) / (Vertices[j].Y - Vertices[i].Y) + Vertices[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}