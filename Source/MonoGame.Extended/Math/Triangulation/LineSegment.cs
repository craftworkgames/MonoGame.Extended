using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Extended.Triangulation
{
    /// <summary>
    /// MIT Licensed: https://github.com/nickgravelyn/Triangulator
    /// </summary>
    struct LineSegment
    {
        public Vertex A;
        public Vertex B;

        public LineSegment(Vertex a, Vertex b)
        {
            A = a;
            B = b;
        }

        public float? IntersectsWithRay(Vector2 origin, Vector2 direction)
        {
            float largestDistance = MathHelper.Max(A.Position.X - origin.X, B.Position.X - origin.X) * 2f;
            LineSegment raySegment = new LineSegment(new Vertex(origin, 0), new Vertex(origin + (direction * largestDistance), 0));

            Vector2? intersection = FindIntersection(this, raySegment);
            float? value = null;

            if (intersection != null)
                value = Vector2.Distance(origin, intersection.Value);

            return value;
        }

        public static Vector2? FindIntersection(LineSegment a, LineSegment b)
        {
            float x1 = a.A.Position.X;
            float y1 = a.A.Position.Y;
            float x2 = a.B.Position.X;
            float y2 = a.B.Position.Y;
            float x3 = b.A.Position.X;
            float y3 = b.A.Position.Y;
            float x4 = b.B.Position.X;
            float y4 = b.B.Position.Y;

            float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);

            float uaNum = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3);
            float ubNum = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3);

            float ua = uaNum / denom;
            float ub = ubNum / denom;

            if (MathHelper.Clamp(ua, 0f, 1f) != ua || MathHelper.Clamp(ub, 0f, 1f) != ub)
                return null;

            return a.A.Position + (a.B.Position - a.A.Position) * ua;
        }
    }
}
