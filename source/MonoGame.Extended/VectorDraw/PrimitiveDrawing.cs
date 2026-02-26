// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

// Adapted from Velcro Physics (formerly known as Farseer Physics).
// Used with permission: https://github.com/craftworkgames/MonoGame.Extended/issues/574

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.Triangulation;

namespace MonoGame.Extended.VectorDraw
{
    /// <summary>
    /// Provides methods for drawing primitive shapes using a <see cref="PrimitiveBatch"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class renders filled shapes and outlines directly via the GPU, producing correct alpha-blended
    /// results even with semi-transparent colors. For quick outline-only drawing via <see cref="SpriteBatch"/>
    /// during prototyping or debugging, see <c>ShapeExtensions</c>.
    /// </para>
    /// <para>
    /// Call <see cref="PrimitiveBatch.Begin"/> on the associated <see cref="PrimitiveBatch"/> before issuing any
    /// draw calls, and <see cref="PrimitiveBatch.End"/> when finished to flush geometry to the GPU.
    /// </para>
    /// </remarks>
    public class PrimitiveDrawing
    {
        /// <summary>
        /// The default number of segments used when approximating circles and ellipses.
        /// </summary>
#if XBOX || WINDOWS_PHONE
        public const int CircleSegments = 16;
#else
        public const int CircleSegments = 32;
#endif

        private readonly PrimitiveBatch _primitiveBatch;

        /// <summary>
        /// Initializes a new instance of <see cref="PrimitiveDrawing"/>.
        /// </summary>
        /// <param name="primitiveBatch">The <see cref="PrimitiveBatch"/> used to issue draw calls.</param>
        /// <exception cref="ArgumentNullException"><paramref name="primitiveBatch"/> is <see langword="null"/>.</exception>
        public PrimitiveDrawing(PrimitiveBatch primitiveBatch)
        {
            ArgumentNullException.ThrowIfNull(primitiveBatch);
            _primitiveBatch = primitiveBatch;
        }

        /// <summary>
        /// Draws a single point at the specified position.
        /// </summary>
        /// <param name="center">The position of the point.</param>
        /// <param name="color">The color of the point.</param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawPoint(Vector2 center, Color color)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            _primitiveBatch.AddVertex(center, color, PrimitiveType.LineList);
            _primitiveBatch.AddVertex(center, color, PrimitiveType.LineList);
        }

        /// <summary>
        /// Draws a rectangle outline.
        /// </summary>
        /// <param name="location">The top-left position of the rectangle in world space.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="color">The color of the outline.</param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawRectangle(Vector2 location, float width, float height, Color color)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            Vector2[] rectVerts = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(width, 0),
                new Vector2(width, height),
                new Vector2(0, height)
            };

            DrawPolygon(location, rectVerts, color);
        }

        /// <summary>
        /// Draws a solid (filled) rectangle with an optional outline.
        /// </summary>
        /// <param name="location">The top-left position of the rectangle in world space.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="color">
        /// The fill color. When <paramref name="outline"/> is <see langword="true"/>, also used for the outline.
        /// </param>
        /// <param name="outline">
        /// When <see langword="true"/>, an outline is drawn over the filled rectangle. Defaults to <see langword="true"/>.
        /// </param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawSolidRectangle(Vector2 location, float width, float height, Color color, bool outline = true)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            Vector2[] rectVerts = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(width, 0),
                new Vector2(width, height),
                new Vector2(0, height)
            };

            DrawSolidPolygon(location, rectVerts, color, outline);
        }

        /// <summary>
        /// Draws a circle outline using <see cref="CircleSegments"/> segments.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the outline.</param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawCircle(Vector2 center, float radius, Color color)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            double increment = Math.PI * 2.0 / CircleSegments;
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

        /// <summary>
        /// Draws a solid (filled) circle with an optional outline using <see cref="CircleSegments"/> segments.
        /// The fill and outline use the same color.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color used for both the fill and the outline.</param>
        /// <param name="outline">
        /// When <see langword="true"/>, an outline is drawn over the filled circle. Defaults to <see langword="true"/>.
        /// </param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawSolidCircle(Vector2 center, float radius, Color color, bool outline = true)
        {
            DrawSolidCircle(center, radius, color, color, outline);
        }

        /// <summary>
        /// Draws a solid (filled) circle with an optional outline using <see cref="CircleSegments"/> segments.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the outline.</param>
        /// <param name="fillColor">The color of the fill.</param>
        /// <param name="outline">
        /// When <see langword="true"/>, an outline is drawn over the filled circle. Defaults to <see langword="true"/>.
        /// </param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawSolidCircle(Vector2 center, float radius, Color color, Color fillColor, bool outline = true)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            double increment = Math.PI * 2.0 / CircleSegments;
            double theta = 0.0;

            Vector2 v0 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
            theta += increment;

            for (int i = 1; i < CircleSegments - 1; i++)
            {
                Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = center + radius * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                _primitiveBatch.AddVertex(v0, fillColor, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(v1, fillColor, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(v2, fillColor, PrimitiveType.TriangleList);

                theta += increment;
            }

            if (outline)
            {
                DrawCircle(center, radius, color);
            }
        }

        /// <summary>
        /// Draws an arc outline.
        /// </summary>
        /// <param name="center">The center point of the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle in radians.</param>
        /// <param name="sweepAngle">
        /// The sweep angle in radians. Positive values sweep counter-clockwise.
        /// Use <c>MathHelper.TwoPi</c> to draw a full circle.
        /// </param>
        /// <param name="sides">The number of line segments used to approximate the arc.</param>
        /// <param name="color">The color of the arc.</param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawArc(Vector2 center, float radius, float startAngle, float sweepAngle, int sides, Color color)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            float step = sweepAngle / sides;
            float theta = startAngle;

            for (int i = 0; i < sides; i++)
            {
                Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = center + radius * new Vector2((float)Math.Cos(theta + step), (float)Math.Sin(theta + step));

                _primitiveBatch.AddVertex(v1, color, PrimitiveType.LineList);
                _primitiveBatch.AddVertex(v2, color, PrimitiveType.LineList);

                theta += step;
            }
        }

        /// <summary>
        /// Draws a solid (filled) arc (pie slice) with an outline. The fill and outline use the same color.
        /// </summary>
        /// <param name="center">The center point of the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle in radians.</param>
        /// <param name="sweepAngle">
        /// The sweep angle in radians. Positive values sweep counter-clockwise.
        /// Use <c>MathHelper.TwoPi</c> for a full circle.
        /// </param>
        /// <param name="sides">The number of triangle segments used to fill the arc.</param>
        /// <param name="color">The color used for both the fill and the outline.</param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawSolidArc(Vector2 center, float radius, float startAngle, float sweepAngle, int sides, Color color)
        {
            DrawSolidArc(center, radius, startAngle, sweepAngle, sides, color, color);
        }

        /// <summary>
        /// Draws a solid (filled) arc (pie slice) with an outline.
        /// </summary>
        /// <param name="center">The center point of the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="startAngle">The starting angle in radians.</param>
        /// <param name="sweepAngle">
        /// The sweep angle in radians. Positive values sweep counter-clockwise.
        /// Use <c>MathHelper.TwoPi</c> for a full circle.
        /// </param>
        /// <param name="sides">The number of triangle segments used to fill the arc.</param>
        /// <param name="color">The color of the outline.</param>
        /// <param name="fillColor">The color of the fill.</param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawSolidArc(Vector2 center, float radius, float startAngle, float sweepAngle, int sides, Color color, Color fillColor)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            float step = sweepAngle / sides;
            float theta = startAngle;

            for (int i = 0; i < sides; i++)
            {
                Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = center + radius * new Vector2((float)Math.Cos(theta + step), (float)Math.Sin(theta + step));

                _primitiveBatch.AddVertex(center, fillColor, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(v1, fillColor, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(v2, fillColor, PrimitiveType.TriangleList);

                theta += step;
            }

            DrawArc(center, radius, startAngle, sweepAngle, sides, color);
        }

        /// <summary>
        /// Draws a line segment between two points.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="color">The color of the line.</param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawSegment(Vector2 start, Vector2 end, Color color)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            _primitiveBatch.AddVertex(start, color, PrimitiveType.LineList);
            _primitiveBatch.AddVertex(end, color, PrimitiveType.LineList);
        }

        /// <summary>
        /// Draws a polygon outline.
        /// </summary>
        /// <param name="position">The world offset applied to all vertices.</param>
        /// <param name="vertices">The polygon vertices in local space.</param>
        /// <param name="color">The color of the outline.</param>
        /// <param name="closed">
        /// When <see langword="true"/>, an edge is drawn between the last and first vertex to close the polygon.
        /// Defaults to <see langword="true"/>.
        /// </param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawPolygon(Vector2 position, Vector2[] vertices, Color color, bool closed = true)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            int count = vertices.Length;

            for (int i = 0; i < count - 1; i++)
            {
                _primitiveBatch.AddVertex(new Vector2(vertices[i].X + position.X, vertices[i].Y + position.Y), color, PrimitiveType.LineList);
                _primitiveBatch.AddVertex(new Vector2(vertices[i + 1].X + position.X, vertices[i + 1].Y + position.Y), color, PrimitiveType.LineList);
            }

            if (closed)
            {
                _primitiveBatch.AddVertex(new Vector2(vertices[count - 1].X + position.X, vertices[count - 1].Y + position.Y), color, PrimitiveType.LineList);
                _primitiveBatch.AddVertex(new Vector2(vertices[0].X + position.X, vertices[0].Y + position.Y), color, PrimitiveType.LineList);
            }
        }

        /// <summary>
        /// Draws a solid (filled) polygon with an optional outline.
        /// </summary>
        /// <param name="position">The world offset applied to all vertices.</param>
        /// <param name="vertices">The polygon vertices in local space.</param>
        /// <param name="color">
        /// The fill color. When <paramref name="outline"/> is <see langword="true"/>, also used for the outline.
        /// </param>
        /// <param name="outline">
        /// When <see langword="true"/>, an outline is drawn over the filled polygon. Defaults to <see langword="true"/>.
        /// </param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawSolidPolygon(Vector2 position, Vector2[] vertices, Color color, bool outline = true)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            int count = vertices.Length;

            if (count == 2)
            {
                DrawPolygon(position, vertices, color);
                return;
            }

            Vector2[] outVertices;
            int[] outIndices;
            Triangulator.Triangulate(vertices, WindingOrder.CounterClockwise, out outVertices, out outIndices);

            for (int i = 0; i < outIndices.Length - 2; i += 3)
            {
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i]].X + position.X, outVertices[outIndices[i]].Y + position.Y), color, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i + 1]].X + position.X, outVertices[outIndices[i + 1]].Y + position.Y), color, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i + 2]].X + position.X, outVertices[outIndices[i + 2]].Y + position.Y), color, PrimitiveType.TriangleList);
            }

            if (outline)
            {
                DrawPolygon(position, vertices, color);
            }
        }

        /// <summary>
        /// Draws an ellipse outline.
        /// </summary>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="radius">The horizontal (X) and vertical (Y) radii of the ellipse.</param>
        /// <param name="sides">The number of line segments used to approximate the ellipse.</param>
        /// <param name="color">The color of the outline.</param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawEllipse(Vector2 center, Vector2 radius, int sides, Color color)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            DrawPolygon(center, CreateEllipse(radius.X, radius.Y, sides), color);
        }

        /// <summary>
        /// Draws a solid (filled) ellipse with an optional outline.
        /// </summary>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="radius">The horizontal (X) and vertical (Y) radii of the ellipse.</param>
        /// <param name="sides">The number of segments used to approximate the ellipse.</param>
        /// <param name="color">
        /// The fill color. When <paramref name="outline"/> is <see langword="true"/>, also used for the outline.
        /// </param>
        /// <param name="outline">
        /// When <see langword="true"/>, an outline is drawn over the filled ellipse. Defaults to <see langword="true"/>.
        /// </param>
        /// <exception cref="InvalidOperationException"><see cref="PrimitiveBatch.Begin"/> must be called first.</exception>
        public void DrawSolidEllipse(Vector2 center, Vector2 radius, int sides, Color color, bool outline = true)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }

            Vector2[] vertices = CreateEllipse(radius.X, radius.Y, sides);

            Vector2[] outVertices;
            int[] outIndices;
            Triangulator.Triangulate(vertices, WindingOrder.CounterClockwise, out outVertices, out outIndices);

            for (int i = 0; i < outIndices.Length - 2; i += 3)
            {
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i]].X + center.X, outVertices[outIndices[i]].Y + center.Y), color, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i + 1]].X + center.X, outVertices[outIndices[i + 1]].Y + center.Y), color, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(new Vector2(outVertices[outIndices[i + 2]].X + center.X, outVertices[outIndices[i + 2]].Y + center.Y), color, PrimitiveType.TriangleList);
            }

            if (outline)
            {
                DrawPolygon(center, vertices, color);
            }
        }

        private static Vector2[] CreateEllipse(float rx, float ry, int sides)
        {
            Vector2[] vertices = new Vector2[sides];
            double t = 0.0;
            double dt = 2.0 * Math.PI / sides;

            for (int i = 0; i < sides; i++, t += dt)
            {
                vertices[i] = new Vector2((float)(rx * Math.Cos(t)), (float)(ry * Math.Sin(t)));
            }

            return vertices;
        }
    }
}
