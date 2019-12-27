using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        private Matrix _localProjection;
        private Matrix _localView;

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
    }
}
