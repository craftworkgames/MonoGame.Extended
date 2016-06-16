using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    //TODO: DrawQuad (TriangleList), DrawTriangle (TriangleList), DrawLine (LineList)

    public static partial class PrimitiveBatchExtensions
    {
        public static void DrawSprite(this PrimitiveBatch<VertexPositionColorTexture> primitiveBatch, Effect effect, Size textureSize, Vector2 position, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = 0, uint sortKey = 0)
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

            var topLeft = new VertexPositionColorTexture();
            var topRight = new VertexPositionColorTexture();
            var bottomLeft = new VertexPositionColorTexture();
            var bottomRight = new VertexPositionColorTexture();

            PrimitiveBatchHelper.SetQuadrilateralRectangleVertexPositionsFromTopLeft(ref position, ref size, rotation, depth, ref origin1, out topLeft.Position, out topRight.Position, out bottomLeft.Position, out bottomRight.Position);

            topLeft.Color = topRight.Color = bottomLeft.Color = bottomRight.Color = color1;
            topLeft.TextureCoordinate = textureCoordinateTopLeft;
            topRight.TextureCoordinate = new Vector2(textureCoordinateBottomRight.X, textureCoordinateTopLeft.Y);
            bottomLeft.TextureCoordinate = new Vector2(textureCoordinateTopLeft.X, textureCoordinateBottomRight.Y);
            bottomRight.TextureCoordinate = textureCoordinateBottomRight;

            primitiveBatch.DrawQuadrilateral(effect, ref topLeft, ref topRight, ref bottomLeft, ref bottomRight, sortKey);
        }
    }
}
