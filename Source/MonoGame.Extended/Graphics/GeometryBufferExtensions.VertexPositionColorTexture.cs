using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Graphics
{
    public static class GeometryBuilderExtensionsVertexPositionColorTexture
    {
        public static void EnqueueSprite(this GeometryBuffer<VertexPositionColorTexture> geometryBuffer, int vertexIndexOffset, TextureRegion2D textureRegion, ref Matrix2D transformMatrix, Color? color = null, Vector2? origin = null, SpriteOptions options = SpriteOptions.None, float depth = 0)
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
            if ((spriteEffect & SpriteOptions.FlipVertically) != 0)
            {
                var temp = textureCoordinateBottomRight.Y;
                textureCoordinateBottomRight.Y = textureCoordinateTopLeft.Y;
                textureCoordinateTopLeft.Y = temp;
            }

            if ((spriteEffect & SpriteOptions.FlipHorizontally) != 0)
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
            position = new Vector2(x: -origin1.X, y: -origin1.Y + textureRegionBounds.Height);
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
