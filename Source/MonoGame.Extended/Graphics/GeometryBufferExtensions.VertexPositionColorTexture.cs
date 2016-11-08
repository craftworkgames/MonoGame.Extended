using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public static class GeometryBuilderExtensionsVertexPositionColorTexture
    {
        /// <summary>
        ///     Adds a sprite to the back of specified <see cref="GeometryBuffer{VertexPositionColorTexture}" />.
        /// </summary>
        /// <param name="geometryBuffer">The <see cref="GeometryBuffer{VertexPositionColorTexture}" />.</param>
        /// <param name="indexOffset">The vertex index offset applied to generated indices.</param>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix2D" />.</param>
        /// <param name="sourceRectangle">
        ///     The texture region <see cref="Rectangle" /> of the <paramref name="texture" />. Use
        ///     <code>null</code> to use the entire <see cref="Texture2D" />.
        /// </param>
        /// <param name="size">
        ///     The <see cref="Size" /> of the sprite. Use <code>null</code> to use the default size which is either
        ///     the size of the <paramref name="sourceRectangle" /> if it is not <code>null</code>, or the texture size if
        ///     <paramref name="sourceRectangle" /> is <code>null</code>.
        /// </param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="origin">
        ///     The origin <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.Zero" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth. The default value is <code>0</code>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        /// <exception cref="GeometryBufferOverflowException{TVertexType}">
        ///     The underlying
        ///     <see cref="GeometryBuffer{TVertexType}" /> is full.
        /// </exception>
        public static unsafe void EnqueueSprite(this GeometryBuffer<VertexPositionColorTexture> geometryBuffer,
            ushort indexOffset, Texture2D texture, ref Matrix2D transformMatrix, Rectangle? sourceRectangle = null, Size? size = null,
            Color? color = null, Vector2? origin = null, SpriteEffects effects = SpriteEffects.None, float depth = 0)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            geometryBuffer.ThrowIfWouldOverflow(4, 6);

            var origin1 = origin ?? Vector2.Zero;
            Vector2 positionTopLeft, positionBottomRight, textureCoordinateTopLeft, textureCoordinateBottomRight;
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

            positionTopLeft.X = -origin1.X;
            positionTopLeft.Y = -origin1.Y;
            positionBottomRight.X = -origin1.X + size1.Width;
            positionBottomRight.Y = -origin1.Y + size1.Height;

            var spriteEffect = effects;
            if ((spriteEffect & SpriteEffects.FlipVertically) != 0)
            {
                var temp = textureCoordinateBottomRight.Y;
                textureCoordinateBottomRight.Y = textureCoordinateTopLeft.Y;
                textureCoordinateTopLeft.Y = temp;
            }

            if ((spriteEffect & SpriteEffects.FlipHorizontally) != 0)
            {
                var temp = textureCoordinateBottomRight.X;
                textureCoordinateBottomRight.X = textureCoordinateTopLeft.X;
                textureCoordinateTopLeft.X = temp;
            }

            var vertex = new VertexPositionColorTexture(new Vector3(Vector2.Zero, depth), color ?? Color.White,
                Vector2.Zero);

            fixed (VertexPositionColorTexture* fixedPointer = geometryBuffer._vertices)
            {
                var pointer = fixedPointer + geometryBuffer._vertexCount;

                // top-left
                var position = positionTopLeft;
                transformMatrix.Transform(position, out position);
                vertex.Position.X = position.X;
                vertex.Position.Y = position.Y;
                vertex.TextureCoordinate = textureCoordinateTopLeft;
                *(pointer + 0) = vertex;

                // top-right
                position.X = positionBottomRight.X;
                position.Y = positionTopLeft.Y;
                transformMatrix.Transform(position, out position);
                vertex.Position.X = position.X;
                vertex.Position.Y = position.Y;
                vertex.TextureCoordinate.X = textureCoordinateBottomRight.X;
                vertex.TextureCoordinate.Y = textureCoordinateTopLeft.Y;
                *(pointer + 1) = vertex;

                // bottom-left
                position.X = positionTopLeft.X;
                position.Y = positionBottomRight.Y;
                transformMatrix.Transform(position, out position);
                vertex.Position.X = position.X;
                vertex.Position.Y = position.Y;
                vertex.TextureCoordinate.X = textureCoordinateTopLeft.X;
                vertex.TextureCoordinate.Y = textureCoordinateBottomRight.Y;
                *(pointer + 2) = vertex;

                // bottom-right
                position = positionBottomRight;
                transformMatrix.Transform(position, out position);
                vertex.Position.X = position.X;
                vertex.Position.Y = position.Y;
                vertex.TextureCoordinate = textureCoordinateBottomRight;
                *(pointer + 3) = vertex;
            }

            geometryBuffer._vertexCount += 4;

            fixed (ushort* fixedPointer = geometryBuffer._indices)
            {
                var pointer = fixedPointer + geometryBuffer._indexCount;
                *(pointer + 0) = (ushort)(0 + indexOffset);
                *(pointer + 1) = (ushort)(1 + indexOffset);
                *(pointer + 2) = (ushort)(2 + indexOffset);
                *(pointer + 3) = (ushort)(1 + indexOffset);
                *(pointer + 4) = (ushort)(3 + indexOffset);
                *(pointer + 5) = (ushort)(2 + indexOffset);
            }

            geometryBuffer._indexCount += 6;
        }
    }
}
