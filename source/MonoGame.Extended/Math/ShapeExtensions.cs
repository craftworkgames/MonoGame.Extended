using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended
{
    /// <summary>
    ///     Sprite batch extensions for drawing primitive shapes
    /// </summary>
    public static class ShapeExtensions
    {
        private static Texture2D _whitePixelTexture;

        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (_whitePixelTexture == null)
            {
                _whitePixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _whitePixelTexture.SetData(new[] { Color.White });
                spriteBatch.Disposing += (sender, args) =>
                {
                    _whitePixelTexture?.Dispose();
                    _whitePixelTexture = null;
                };
            }

            return _whitePixelTexture;
        }

        /// <summary>
        ///     Draws a closed polygon from a <see cref="Polygon" /> shape
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// ///
        /// <param name="position">Where to position the polygon</param>
        /// <param name="polygon">The polygon to draw</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the lines</param>
        /// /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2 position, Polygon polygon, Color color, float thickness = 1f, float layerDepth = 0)
        {
            DrawPolygon(spriteBatch, position, polygon.Vertices, color, thickness, layerDepth);
        }

        /// <summary>
        ///     Draws a closed polygon from an array of points
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// ///
        /// <param name="offset">Where to offset the points</param>
        /// <param name="points">The points to connect with lines</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the lines</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2 offset, IReadOnlyList<Vector2> points, Color color, float thickness = 1f, float layerDepth = 0)
        {
            if (points.Count == 0)
                return;

            if (points.Count == 1)
            {
                DrawPoint(spriteBatch, points[0], color, (int)thickness);
                return;
            }

            var texture = GetTexture(spriteBatch);

            for (var i = 0; i < points.Count - 1; i++)
                DrawPolygonEdge(spriteBatch, texture, points[i] + offset, points[i + 1] + offset, color, thickness, layerDepth);

            DrawPolygonEdge(spriteBatch, texture, points[points.Count - 1] + offset, points[0] + offset, color, thickness, layerDepth);
        }

        private static void DrawPolygonEdge(SpriteBatch spriteBatch, Texture2D texture, Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth)
        {
            var length = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(texture, point1, null, color, angle, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rectangle">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, RectangleF rectangle, Color color, float layerDepth = 0)
        {
            FillRectangle(spriteBatch, rectangle.Position, rectangle.Size, color, layerDepth);
        }

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, Vector2 location, SizeF size, Color color, float layerDepth = 0)
        {
            spriteBatch.Draw(GetTexture(spriteBatch), location, null, color, 0, Vector2.Zero, size, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The X coord of the left side</param>
        /// <param name="y">The Y coord of the upper side</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color, float layerDepth = 0)
        {
            FillRectangle(spriteBatch, new Vector2(x, y), new SizeF(width, height), color, layerDepth);
        }

        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rectangle">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the lines</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, RectangleF rectangle, Color color, float thickness = 1f, float layerDepth = 0)
        {
            var texture = GetTexture(spriteBatch);
            var topLeft = new Vector2(rectangle.X, rectangle.Y);
            var topRight = new Vector2(rectangle.Right - thickness, rectangle.Y);
            var bottomLeft = new Vector2(rectangle.X, rectangle.Bottom - thickness);
            var horizontalScale = new Vector2(rectangle.Width, thickness);
            var verticalScale = new Vector2(thickness, rectangle.Height);

            spriteBatch.Draw(texture, topLeft, null, color, 0f, Vector2.Zero, horizontalScale, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(texture, topLeft, null, color, 0f, Vector2.Zero, verticalScale, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(texture, topRight, null, color, 0f, Vector2.Zero, verticalScale, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(texture, bottomLeft, null, color, 0f, Vector2.Zero, horizontalScale, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 location, SizeF size, Color color, float thickness = 1f, float layerDepth = 0)
        {
            DrawRectangle(spriteBatch, new RectangleF(location.X, location.Y, size.Width, size.Height), color, thickness, layerDepth);
        }


        /// <summary>
        /// Draws a rectangle outline.
        /// </summary>
        public static void DrawRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color, float thickness = 1f, float layerDepth = 0)
        {
            DrawRectangle(spriteBatch, new RectangleF(x, y, width, height), color, thickness, layerDepth);
        }

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x1">The X coord of the first point</param>
        /// <param name="y1">The Y coord of the first point</param>
        /// <param name="x2">The X coord of the second point</param>
        /// <param name="y2">The Y coord of the second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float thickness = 1f, float layerDepth = 0)
        {
            DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, thickness, layerDepth);
        }

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f, float layerDepth = 0)
        {
            // calculate the distance between the two vectors
            var distance = Vector2.Distance(point1, point2);

            // calculate the angle between the two vectors
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(spriteBatch, point1, distance, angle, color, thickness, layerDepth);
        }

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point">The starting point</param>
        /// <param name="length">The length of the line</param>
        /// <param name="angle">The angle of this line from the starting point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f, float layerDepth = 0)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Draws a point at the specified x, y position. The center of the point will be at the position.
        /// </summary>
        public static void DrawPoint(this SpriteBatch spriteBatch, float x, float y, Color color, float size = 1f, float layerDepth = 0)
        {
            DrawPoint(spriteBatch, new Vector2(x, y), color, size, layerDepth);
        }

        /// <summary>
        /// Draws a point at the specified position. The center of the point will be at the position.
        /// </summary>
        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color color, float size = 1f, float layerDepth = 0)
        {
            var scale = Vector2.One * size;
            var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
            spriteBatch.Draw(GetTexture(spriteBatch), position + offset, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Draw a circle from a <see cref="CircleF" /> shape
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="circle">The circle shape to draw</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the lines used</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, CircleF circle, int sides, Color color, float thickness = 1f, float layerDepth = 0)
        {
            DrawCircle(spriteBatch, circle.Center, circle.Radius, sides, color, thickness, layerDepth);
        }

        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the lines used</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness = 1f, float layerDepth = 0)
        {
            DrawPolygon(spriteBatch, center, CreateCircle(radius, sides), color, thickness, layerDepth);
        }

        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The center X of the circle</param>
        /// <param name="y">The center Y of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, float radius, int sides, Color color, float thickness = 1f, float layerDepth = 0)
        {
            DrawPolygon(spriteBatch, new Vector2(x, y), CreateCircle(radius, sides), color, thickness, layerDepth);
        }

        /// <summary>
        /// Draw an ellipse.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">Center of the ellipse</param>
        /// <param name="radius">Radius of the ellipse</param>
        /// <param name="sides">The number of sides to generate.</param>
        /// <param name="color">The color of the ellipse.</param>
        /// <param name="thickness">The thickness of the line around the ellipse.</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawEllipse(this SpriteBatch spriteBatch, Vector2 center, Vector2 radius, int sides, Color color, float thickness = 1f, float layerDepth = 0)
        {
            DrawPolygon(spriteBatch, center, CreateEllipse(radius.X, radius.Y, sides), color, thickness, layerDepth);
        }

        private static Vector2[] CreateCircle(double radius, int sides)
        {
            const double max = 2.0 * Math.PI;
            var points = new Vector2[sides];
            var step = max / sides;
            var theta = 0.0;

            for (var i = 0; i < sides; i++)
            {
                points[i] = new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)));
                theta += step;
            }

            return points;
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
