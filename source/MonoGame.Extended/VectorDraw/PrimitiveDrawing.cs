using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Triangulation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Extended.VectorDraw
{
    public class PrimitiveDrawing
    {
        //Drawing
        private PrimitiveBatch _primitiveBatch;

        private SpriteBatch _batch;
        private SpriteFont _font;
        private GraphicsDevice _device;
        private readonly Vector2[] _tempVertices = new Vector2[1000]; //TODO: something else...

        //private Matrix _localProjection;
        //private Matrix _localView;

        //TODO: do we need to split this based on platform?
#if XBOX || WINDOWS_PHONE
        public const int CircleSegments = 16;
#else
        public const int CircleSegments = 32;
#endif

        public PrimitiveDrawing(PrimitiveBatch primitiveBatch)
        {
            _primitiveBatch = primitiveBatch;
        }

        public void DrawPoint(Vector2 center, Color color)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            //Add two points or the PrimitiveBatch acts up
            _primitiveBatch.AddVertex(center, color, PrimitiveType.LineList);
            _primitiveBatch.AddVertex(center, color, PrimitiveType.LineList);
        }

        public void DrawRectangle(Vector2 location, float width, float height, Color color)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            Vector2[] rectVerts = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(width, 0),
                new Vector2(width, height),
                new Vector2(0, height)
            };

            //Location is offset here
            DrawPolygon(location, rectVerts, color);
        }

        public void DrawSolidRectangle(Vector2 location, float width, float height, Color color)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            Vector2[] rectVerts = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(width, 0),
                new Vector2(width, height),
                new Vector2(0, height)
            };

            DrawSolidPolygon(location, rectVerts, color);
        }

            public void DrawCircle(Vector2 center, float radius, Color color)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            const double increment = Math.PI * 2.0 / CircleSegments;
            double theta = 0.0;

            for (int i = 0; i < CircleSegments; i++)
            {
                Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = center + radius * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                _primitiveBatch.AddVertex(v1, color, PrimitiveType.LineList);
                _primitiveBatch.AddVertex(v2, color, PrimitiveType.LineList);

                theta += increment;
            }
        }

        public void DrawSolidCircle(Vector2 center, float radius, Color color)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            const double increment = Math.PI * 2.0 / CircleSegments;
            double theta = 0.0;

            Color colorFill = color * 0.5f;

            Vector2 v0 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
            theta += increment;

            for (int i = 1; i < CircleSegments - 1; i++)
            {
                Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = center + radius * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                _primitiveBatch.AddVertex(v0, colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(v1, colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(v2, colorFill, PrimitiveType.TriangleList);

                theta += increment;
            }

            DrawCircle(center, radius, color);            
        }
        public void DrawSolidCircle(Vector2 center, float radius, Color color, Color fillcolor)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            const double increment = Math.PI * 2.0 / CircleSegments;
            double theta = 0.0;

            Vector2 v0 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
            theta += increment;

            for (int i = 1; i < CircleSegments - 1; i++)
            {
                Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = center + radius * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                _primitiveBatch.AddVertex(v0, fillcolor, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(v1, fillcolor, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(v2, fillcolor, PrimitiveType.TriangleList);

                theta += increment;
            }

            DrawCircle(center, radius, color);
        }

        public void DrawSegment(Vector2 start, Vector2 end, Color color)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            _primitiveBatch.AddVertex(start, color, PrimitiveType.LineList);
            _primitiveBatch.AddVertex(end, color, PrimitiveType.LineList);
        }

        public void DrawPolygon(Vector2 position, Vector2[] vertices, Color color, bool closed = true)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            int count = vertices.Length;

            for (int i = 0; i < count - 1; i++)
            {
                //translate the vertices according to the position passed
                _primitiveBatch.AddVertex(new Vector2(vertices[i].X + position.X, vertices[i].Y + position.Y), color, PrimitiveType.LineList);
                _primitiveBatch.AddVertex(new Vector2(vertices[i + 1].X + position.X, vertices[i + 1].Y + position.Y), color, PrimitiveType.LineList);
            }
            if (closed)
            {
                //TODO: verify closed is working as expected
                _primitiveBatch.AddVertex(new Vector2(vertices[count - 1].X + position.X, vertices[count - 1].Y + position.Y), color, PrimitiveType.LineList);
                _primitiveBatch.AddVertex(new Vector2(vertices[0].X + position.X, vertices[0].Y + position.Y), color, PrimitiveType.LineList);
            }
        }

        public void DrawSolidPolygon(Vector2 position, Vector2[] vertices, Color color, bool outline = true)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            int count = vertices.Length;

            if (count == 2)
            {
                DrawPolygon(position, vertices, color);
                return;
            }

            Color colorFill = color * (outline ? 0.5f : 1.0f);

            Vector2[] outVertices;
            int[] outIndices;
            Triangulator.Triangulate(vertices, WindingOrder.CounterClockwise, out outVertices, out outIndices);

            for(int i = 0; i < outIndices.Length - 2; i += 3)
            {
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i]].X + position.X, outVertices[outIndices[i]].Y + position.Y), colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i + 1]].X + position.X, outVertices[outIndices[i + 1]].Y + position.Y), colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i + 2]].X + position.X, outVertices[outIndices[i + 2]].Y + position.Y), colorFill, PrimitiveType.TriangleList);
            }

            if (outline)
                DrawPolygon(position, vertices, color);
        }

        public void DrawEllipse(Vector2 center, Vector2 radius, int sides, Color color)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            DrawPolygon(center, CreateEllipse(radius.X, radius.Y, sides), color);
        }

        public void DrawSolidEllipse(Vector2 center, Vector2 radius, int sides, Color color, bool outline = true)
        {
            if (!_primitiveBatch.IsReady())
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");

            Color colorFill = color * (outline ? 0.5f : 1.0f);

            Vector2[] vertices = CreateEllipse(radius.X, radius.Y, sides);

            Vector2[] outVertices;
            int[] outIndices;
            Triangulator.Triangulate(vertices, WindingOrder.CounterClockwise, out outVertices, out outIndices);

            for (int i = 0; i < outIndices.Length - 2; i += 3)
            {
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i]].X + center.X, outVertices[outIndices[i]].Y + center.Y), colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i + 1]].X + center.X, outVertices[outIndices[i + 1]].Y + center.Y), colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i + 2]].X + center.X, outVertices[outIndices[i + 2]].Y + center.Y), colorFill, PrimitiveType.TriangleList);
            }
        }

        private static Vector2[] CreateEllipse(float rx, float ry, int sides)
        {
            var vertices = new Vector2[sides];

            var t = 0.0;
            var dt = 2.0 * Math.PI / sides;
            for (var i = 0; i < sides; i++, t += dt)
            {
                var x = (float)(rx * Math.Cos(t));
                var y = (float)(ry * Math.Sin(t));
                vertices[i] = new Vector2(x, y);
            }
            return vertices;
        }
    }
}
