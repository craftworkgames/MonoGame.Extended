using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Graphics.Batching
{
    public static class PrimitiveBatchExtensions
    {
        private static VertexPositionColorTexture[] _spriteVertices;

        internal static VertexPositionColorTexture[] SpriteVertices
        {
            get { return _spriteVertices ?? (_spriteVertices = new VertexPositionColorTexture[4]); }
        }

        private static VertexPositionColor[] _quadVertices;
        private static short[] _quadIndices;

        internal static VertexPositionColor[] QuadVertices
        {
            get { return _quadVertices ?? (_quadVertices = new VertexPositionColor[4]); }
        }

        public static short[] QuadIndices
        {
            get
            {
                return _quadIndices ?? (_quadIndices = new short[]
                {
                    0,
                    2,
                    3,
                    0,
                    3,
                    1
                });
            }
        }

        public static void DrawRectangle(this PrimitiveBatch<VertexPositionColor> primitiveBatch, Effect effect, Vector2 position, SizeF size, Color? color = null, float rotation = 0f, Vector2? origin = null, Vector2? scale = null, float depth = 0, uint sortKey = 0)
        {
            var color1 = color ?? Color.White;
            var origin1 = origin ?? Vector2.Zero;
            var scale1 = scale ?? Vector2.One;

            origin1 *= scale1;

            var vertices = QuadVertices;

            SetQuadVertexPositionsFromTopLeft(ref position, depth, ref size, rotation, ref origin1, out vertices[0].Position, out vertices[1].Position, out vertices[2].Position, out vertices[3].Position);
            // top-left
            vertices[0].Color = color1;
            // top-right
            vertices[1].Color = color1;
            // bottom-left
            vertices[2].Color = color1;
            // bottom-right
            vertices[3].Color = color1;

            primitiveBatch.Draw(effect, PrimitiveType.TriangleList, vertices, QuadIndices);
        }

        public static void DrawRectangle(this PrimitiveBatch<VertexPositionColor> primitiveBatch, Effect effect, ref RectangleF rectangle, Color? color = null, float depth = 0, uint sortKey = 0)
        {
            var position = new Vector2(rectangle.X, rectangle.Y);
            var size = rectangle.Size;
            DrawRectangle(primitiveBatch, effect, position, size, color, 0, null, null, depth, sortKey);
        }

        public static void DrawSprite(this PrimitiveBatch<VertexPositionColorTexture> primitiveBatch, Effect effect, Size textureSize, Rectangle? sourceRectangle, Vector2 position, Color? color = null, float rotation = 0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = 0, uint sortKey = 0)
        {
            if (effect == null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            var color1 = color ?? Color.White;
            var origin1 = origin ?? Vector2.Zero;
            var scale1 = scale ?? Vector2.One;
            var textureSize1 = new SizeF(textureSize.Width, textureSize.Height);

            origin1 *= scale1;

            SizeF size;
            Vector2 textureCoordinateTopLeft;
            Vector2 textureCoordinateBottomRight;

            if (sourceRectangle.HasValue)
            {
                var rectangle = sourceRectangle.Value;
                size = rectangle;
                textureCoordinateTopLeft.X = rectangle.X / textureSize1.Width;
                textureCoordinateTopLeft.Y = rectangle.Y / textureSize1.Height;
                textureCoordinateBottomRight.X = (rectangle.X + rectangle.Width) / textureSize1.Width;
                textureCoordinateBottomRight.Y = (rectangle.Y + rectangle.Height) / textureSize1.Height;
            }
            else
            {
                textureCoordinateTopLeft.X = 0;
                textureCoordinateTopLeft.Y = 0;
                textureCoordinateBottomRight.X = 1;
                textureCoordinateBottomRight.Y = 1;
                size = textureSize1;
            }

            if ((spriteEffects & SpriteEffects.FlipVertically) != 0)
            {
                var temp = textureCoordinateBottomRight.Y;
                textureCoordinateBottomRight.Y = textureCoordinateTopLeft.Y;
                textureCoordinateTopLeft.Y = temp;
            }
            if ((spriteEffects & SpriteEffects.FlipHorizontally) != 0)
            {
                var temp = textureCoordinateBottomRight.X;
                textureCoordinateBottomRight.X = textureCoordinateTopLeft.X;
                textureCoordinateTopLeft.X = temp;
            }

            var vertices = SpriteVertices;
            SetQuadVertexPositionsFromTopLeft(ref position, depth, ref size, rotation, ref origin1, out vertices[0].Position, out vertices[1].Position, out vertices[2].Position, out vertices[3].Position);
            // top-left
            vertices[0].Color = color1;
            vertices[0].TextureCoordinate = textureCoordinateTopLeft;
            // top-right
            vertices[1].Color = color1;
            vertices[1].TextureCoordinate = new Vector2(textureCoordinateBottomRight.X, textureCoordinateTopLeft.Y);
            // bottom-left
            vertices[2].Color = color1;
            vertices[2].TextureCoordinate = new Vector2(textureCoordinateTopLeft.X, textureCoordinateBottomRight.Y);
            // bottom-right
            vertices[3].Color = color1;
            vertices[3].TextureCoordinate = textureCoordinateBottomRight;

            primitiveBatch.Draw(effect, PrimitiveType.TriangleList, vertices, QuadIndices, sortKey);
        }

        private static void SetQuadVertexPositionsFromTopLeft(ref Vector2 topLeftPosition, float depth, ref SizeF size, float rotation, ref Vector2 origin, out Vector3 topLeft, out Vector3 topRight, out Vector3 bottomLeft, out Vector3 bottomRight)
        {
            var x = topLeftPosition.X;
            var y = topLeftPosition.Y;
            var w = size.Width;
            var h = size.Height;
            var dx = -origin.X;
            var dy = -origin.Y;
            var sin = (float)Math.Sin(rotation);
            var cos = (float)Math.Cos(rotation);

            topLeft.X = x + dx * cos - dy * sin;
            topLeft.Y = y + dx * sin + dy * cos;
            topLeft.Z = depth;

            topRight.X = x + (dx + w) * cos - dy * sin;
            topRight.Y = y + (dx + w) * sin + dy * cos;
            topRight.Z = depth;

            bottomLeft.X = x + dx * cos - (dy + h) * sin;
            bottomLeft.Y = y + dx * sin + (dy + h) * cos;
            bottomLeft.Z = depth;

            bottomRight.X = x + (dx + w) * cos - (dy + h) * sin;
            bottomRight.Y = y + (dx + w) * sin + (dy + h) * cos;
            bottomRight.Z = depth;
        }
    }
}
