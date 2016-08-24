using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Graphics
{
    public static class GeometryBuilderExtensionsVertexPositionColorTexture
    {
        /// <summary>
        ///     Enqueues a sprite into the specified <see cref="GeometryBuffer{VertexPositionColorTexture}" />.
        /// </summary>
        /// <param name="geometryBuffer">The <see cref="GeometryBuffer{VertexPositionColorTexture}" />.</param>
        /// <param name="vertexIndexOffset">The vertex index offset applied to the generated indices.</param>
        /// <param name="textureRegion">The <see cref="TextureRegion2D" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix2D" />.</param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="origin">
        ///     The origin <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.Zero" />.
        /// </param>
        /// <param name="options">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth. The default value is <code>0</code>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void EnqueueSprite(this GeometryBuffer<VertexPositionColorTexture> geometryBuffer, int vertexIndexOffset, TextureRegion2D textureRegion, ref Matrix2D transformMatrix, Color? color = null, Vector2? origin = null, SpriteEffects options = SpriteEffects.None, float depth = 0)
        {
            if (textureRegion == null)
                throw new ArgumentNullException(paramName: nameof(textureRegion));

            var textureRegionBounds = textureRegion.Bounds;
            var textureSize = new Size(textureRegion.Texture.Width, textureRegion.Texture.Height);

            Vector2 textureCoordinateTopLeft, textureCoordinateBottomRight;

            textureCoordinateTopLeft.X = (textureRegionBounds.X + 0.5f) / textureSize.Width;
            textureCoordinateTopLeft.Y = (textureRegionBounds.Y + 0.5f) / textureSize.Height;

            textureCoordinateBottomRight.X = (textureRegion.X + textureRegionBounds.Width + 0.5f) / textureSize.Width;
            textureCoordinateBottomRight.Y = (textureRegion.Y + textureRegionBounds.Height + 0.5f) / textureSize.Height;

            var spriteEffect = options;
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

            var vertex = new VertexPositionColorTexture(position: new Vector3(Vector2.Zero, depth), color: color ?? Color.White, textureCoordinate: Vector2.Zero);
            var origin1 = origin ?? Vector2.Zero;
            Vector2 position;

            // top-left
            position = new Vector2(-origin1.X, -origin1.Y);
            transformMatrix.Transform(position, out position);
            vertex.Position.X = position.X;
            vertex.Position.Y = position.Y;
            vertex.TextureCoordinate = textureCoordinateTopLeft;
            geometryBuffer.Enqueue(ref vertex);

            // top-right
            position = new Vector2(x: -origin1.X + textureRegionBounds.Width, y: -origin1.Y);
            transformMatrix.Transform(position, out position);
            vertex.Position.X = position.X;
            vertex.Position.Y = position.Y;
            vertex.TextureCoordinate.X = textureCoordinateBottomRight.X;
            vertex.TextureCoordinate.Y = textureCoordinateTopLeft.Y;
            geometryBuffer.Enqueue(ref vertex);

            // bottom-left
            position = new Vector2(-origin1.X, y: -origin1.Y + textureRegionBounds.Height);
            transformMatrix.Transform(position, out position);
            vertex.Position.X = position.X;
            vertex.Position.Y = position.Y;
            vertex.TextureCoordinate.X = textureCoordinateTopLeft.X;
            vertex.TextureCoordinate.Y = textureCoordinateBottomRight.Y;
            geometryBuffer.Enqueue(ref vertex);

            // bottom-right
            position = new Vector2(x: -origin1.X + textureRegionBounds.Width, y: -origin1.Y + textureRegionBounds.Height);
            transformMatrix.Transform(position, out position);
            vertex.Position.X = position.X;
            vertex.Position.Y = position.Y;
            vertex.TextureCoordinate = textureCoordinateBottomRight;
            geometryBuffer.Enqueue(ref vertex);

            geometryBuffer.EnqueueClockwiseQuadrilateralIndices(vertexIndexOffset);
        }
    }
}
