using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Geometry
{
    public class SpriteBuilderPositionColorTextureU16 : GraphicsGeometryBuilder<VertexPositionColorTexture, ushort>
    {
        public SpriteBuilderPositionColorTextureU16()
            : base(PrimitiveType.TriangleList, 4, 6)
        {
            Indices[0] = 0;
            Indices[1] = 1;
            Indices[2] = 2;
            Indices[3] = 1;
            Indices[4] = 3;
            Indices[5] = 2;
        }

        public unsafe void BuildSprite(ref Matrix2D transformMatrix, Texture2D texture, Rectangle? sourceRectangle = null,
            Color? color = null, Size? size = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = 0)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            Vector2 textureCoordinateTopLeft, textureCoordinateBottomRight;
            Size size1;

            if (sourceRectangle == null)
            {
                size1 = size ?? new Size(texture.Width, texture.Height);
                textureCoordinateTopLeft.X = 0;
                textureCoordinateTopLeft.Y = 0;
                textureCoordinateBottomRight.X = 1;
                textureCoordinateBottomRight.Y = 1;
            }
            else
            {
                var textureRegion = sourceRectangle.Value;
                size1 = size ?? new Size(textureRegion.Width, textureRegion.Height);
                textureCoordinateTopLeft.X = textureRegion.X / (float)texture.Width;
                textureCoordinateTopLeft.Y = textureRegion.Y / (float)texture.Height;
                textureCoordinateBottomRight.X = (textureRegion.X + textureRegion.Width) / (float)texture.Width;
                textureCoordinateBottomRight.Y = (textureRegion.Y + textureRegion.Height) / (float)texture.Height;
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

            // Use XNA legacy model space origin of top-left instead of center of sprite
            Vector2 positionTopLeft, positionTopRight, positionBottomLeft, positionBottomRight;
            positionTopLeft.X = 0;
            positionTopLeft.Y = 0;

            positionTopRight.X = size1.Width;
            positionTopRight.Y = 0;

            positionBottomLeft.X = 0;
            positionBottomLeft.Y = size1.Height;

            positionBottomRight.X = size1.Width;
            positionBottomRight.Y = size1.Height;

            var vertex = default(VertexPositionColorTexture);
            vertex.Position.Z = depth;
            vertex.Color = color ?? Color.White;

            fixed (VertexPositionColorTexture* fixedPointer = Vertices)
            {
                var pointer = fixedPointer;

                Vector2 point;
                transformMatrix.Transform(positionTopLeft, out point);
                vertex.Position.X = point.X;
                vertex.Position.Y = point.Y;
                vertex.TextureCoordinate = textureCoordinateTopLeft;
                *pointer++ = vertex;

                transformMatrix.Transform(positionTopRight, out point);
                vertex.Position.X = point.X;
                vertex.Position.Y = point.Y;
                vertex.TextureCoordinate.X = textureCoordinateBottomRight.X;
                vertex.TextureCoordinate.Y = textureCoordinateTopLeft.Y;
                *pointer++ = vertex;

                transformMatrix.Transform(positionBottomLeft, out point);
                vertex.Position.X = point.X;
                vertex.Position.Y = point.Y;
                vertex.TextureCoordinate.X = textureCoordinateTopLeft.X;
                vertex.TextureCoordinate.Y = textureCoordinateBottomRight.Y;
                *pointer++ = vertex;

                transformMatrix.Transform(positionBottomRight, out point);
                vertex.Position.X = point.X;
                vertex.Position.Y = point.Y;
                vertex.TextureCoordinate = textureCoordinateBottomRight;
                *pointer = vertex;
            }
        }
    }
}
