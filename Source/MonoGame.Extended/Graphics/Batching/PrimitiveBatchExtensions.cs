using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public static class PrimitiveBatchExtensions
    {
        private static readonly VertexPositionColorTexture[] _spriteItemVertices = new VertexPositionColorTexture[4];
        private static readonly short[] _spriteItemIndices = {
            0,
            1,
            2,
            1,
            3,
            2
        };

        public static void DrawSprite(this PrimitiveBatch<VertexPositionColorTexture> primitiveBatch, IDrawContextTexture2D drawContext, Rectangle? sourceRectangle, Vector3 position, Color? color = null, float rotation = 0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, uint sortKey = 0)
        {
            if (drawContext == null)
            {
                throw new ArgumentNullException(nameof(drawContext));
            }

            var texture = drawContext.Texture;
            if (texture == null)
            {
                throw new ArgumentException("Texture is null.");
            }

            var color1 = color ?? Color.White;
            var origin1 = origin ?? Vector2.Zero;
            var scale1 = scale ?? Vector2.One;

            origin1 *= scale1;

            int width;
            int height;
            Vector2 textureCoordinateTopLeft;
            Vector2 textureCoordinateBottomRight;

            var textureWidth = texture.Width;
            var textureHeight = texture.Height;
            if (sourceRectangle.HasValue)
            {
                var rectangle = sourceRectangle.Value;
                width = rectangle.Width;
                height = rectangle.Height;
                textureCoordinateTopLeft.X = rectangle.X / (float)textureWidth;
                textureCoordinateTopLeft.Y = rectangle.Y / (float)textureHeight;
                textureCoordinateBottomRight.X = (rectangle.X + rectangle.Width) / (float)textureWidth;
                textureCoordinateBottomRight.Y = (rectangle.Y + rectangle.Height) / (float)textureHeight;
            }
            else
            {
                textureCoordinateTopLeft.X = 0;
                textureCoordinateTopLeft.Y = 0;
                textureCoordinateBottomRight.X = 1;
                textureCoordinateBottomRight.Y = 1;
                width = textureWidth;
                height = textureHeight;
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

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (rotation == 0f)
            {
                SetSpriteItemVerticesNoRotation(position.X - origin1.X, position.Y - origin1.Y, width, height, color1, textureCoordinateTopLeft, textureCoordinateBottomRight, position.Z);
            }
            else
            {
                SetSpriteItemVerticesWithRotation(position.X, position.Y, -origin1.X, -origin1.Y, width, height, (float)Math.Sin(rotation), (float)Math.Cos(rotation), color1, textureCoordinateTopLeft, textureCoordinateBottomRight, position.Z);
            }

            primitiveBatch.Draw(drawContext, PrimitiveType.TriangleList, _spriteItemVertices, _spriteItemIndices, sortKey);
        }

        private static void SetSpriteItemVerticesWithRotation(float x, float y, float dx, float dy, float w, float h, float sin, float cos, Color color, Vector2 topLeftTextureCoordinate, Vector2 bottomRightTextureCoordinate, float depth)
        {
            var topLeftPosition = new Vector3(x + dx * cos - dy * sin, y + dx * sin + dy * cos, depth);
            var topLeftVertex = new VertexPositionColorTexture(topLeftPosition, color, topLeftTextureCoordinate);
            _spriteItemVertices[0] = topLeftVertex;

            var topRightPosition = new Vector3(x + (dx + w) * cos - dy * sin, y + (dx + w) * sin + dy * cos, depth);
            var topRightTextureCoordinate = new Vector2(bottomRightTextureCoordinate.X, topLeftTextureCoordinate.Y);
            var topRightVertex = new VertexPositionColorTexture(topRightPosition, color, topRightTextureCoordinate);
            _spriteItemVertices[1] = topRightVertex;

            var bottomLeftPosition = new Vector3(x + dx * cos - (dy + h) * sin, y + dx * sin + (dy + h) * cos, depth);
            var bottomLeftTextureCoordinate = new Vector2(topLeftTextureCoordinate.X, bottomRightTextureCoordinate.Y);
            var bottomLeftVertex = new VertexPositionColorTexture(bottomLeftPosition, color, bottomLeftTextureCoordinate);
            _spriteItemVertices[2] = bottomLeftVertex;

            var bottomRightPosition = new Vector3(x + (dx + w) * cos - (dy + h) * sin, y + (dx + w) * sin + (dy + h) * cos, depth);
            var bottomRightVertex = new VertexPositionColorTexture(bottomRightPosition, color, bottomRightTextureCoordinate);
            _spriteItemVertices[3] = bottomRightVertex;
        }

        private static void SetSpriteItemVerticesNoRotation(float x, float y, float w, float h, Color color, Vector2 topLeftTextureCoordinate, Vector2 bottomRightTextureCoordinate, float depth)
        {
            var topLeftPosition = new Vector3(x, y, depth);
            var topLeftVertex = new VertexPositionColorTexture(topLeftPosition, color, topLeftTextureCoordinate);
            _spriteItemVertices[0] = topLeftVertex;

            var topRightPosition = new Vector3(x + w, y, depth);
            var topRightTextureCoordinate = new Vector2(bottomRightTextureCoordinate.X, topLeftTextureCoordinate.Y);
            var topRightVertex = new VertexPositionColorTexture(topRightPosition, color, topRightTextureCoordinate);
            _spriteItemVertices[1] = topRightVertex;

            var bottomLeftPosition = new Vector3(x, y + h, depth);
            var bottomLeftTextureCoordinate = new Vector2(topLeftTextureCoordinate.X, bottomRightTextureCoordinate.Y);
            var bottomLeftVertex = new VertexPositionColorTexture(bottomLeftPosition, color, bottomLeftTextureCoordinate);
            _spriteItemVertices[2] = bottomLeftVertex;

            var bottomRightPosition = new Vector3(x + w, y + h, depth);
            var bottomRightVertex = new VertexPositionColorTexture(bottomRightPosition, color, bottomRightTextureCoordinate);
            _spriteItemVertices[3] = bottomRightVertex;
        }
    }
}
