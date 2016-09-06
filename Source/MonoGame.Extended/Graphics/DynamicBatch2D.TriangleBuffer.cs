using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public partial class DynamicBatch2D
    {
        internal class TriangleBuffer : GeometryBuffer<VertexPositionColorTexture, ushort>
        {
            public TriangleBuffer(GraphicsDevice graphicsDevice, int maximumVerticesCount, int maximumIndicesCount)
                : base(graphicsDevice, GeometryBufferType.Dynamic, maximumVerticesCount, maximumIndicesCount)
            {
            }

            private unsafe void EnqueueClockwiseQuadrilateralIndices(int indexOffset)
            {
                fixed (ushort* fixedPointer = Indices)
                {
                    var pointer = fixedPointer + IndexCount;
                    *(pointer + 0) = (ushort)(0 + indexOffset);
                    *(pointer + 1) = (ushort)(1 + indexOffset);
                    *(pointer + 2) = (ushort)(2 + indexOffset);
                    *(pointer + 3) = (ushort)(1 + indexOffset);
                    *(pointer + 4) = (ushort)(3 + indexOffset);
                    *(pointer + 5) = (ushort)(2 + indexOffset);
                }
                IndexCount += 6;
            }

            public unsafe int EnqueueSprite(
                Texture2D texture, ref Matrix2D transformMatrix, Rectangle? sourceRectangle = null,
                Size? size = null,
                Color? color = null, Vector2? origin = null, SpriteEffects effects = SpriteEffects.None, float depth = 0)
            {
                if (texture == null)
                    throw new ArgumentNullException(nameof(texture));

                ThrowIfWouldOverflow(4, 6);

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

                fixed (VertexPositionColorTexture* fixedPointer = Vertices)
                {
                    var pointer = fixedPointer + VertexCount;

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

                var startVertex = VertexCount;
                VertexCount += 4;

                var startIndex = IndexCount;
                EnqueueClockwiseQuadrilateralIndices(startVertex);

                return startIndex;
            }

            public unsafe int EnqueueRectangle(ref Matrix2D transformMatrix, SizeF size, Color? color = null, float depth = 0)
            {
                ThrowIfWouldOverflow(4, 6);

                Vector2 positionTopLeft, positionBottomRight;

                var halfSize = size * 0.5f;

                positionTopLeft.X = -halfSize.Width;
                positionTopLeft.Y = -halfSize.Height;
                positionBottomRight.X = halfSize.Width;
                positionBottomRight.Y = halfSize.Height;

                var vertex = new VertexPositionColorTexture(new Vector3(Vector2.Zero, depth), color ?? Color.White,
                    Vector2.Zero);

                fixed (VertexPositionColorTexture* fixedPointer = Vertices)
                {
                    var pointer = fixedPointer + VertexCount;

                    // top-left
                    var position = positionTopLeft;
                    transformMatrix.Transform(position, out position);
                    vertex.Position.X = position.X;
                    vertex.Position.Y = position.Y;
                    *(pointer + 0) = vertex;

                    // top-right
                    position.X = positionBottomRight.X;
                    position.Y = positionTopLeft.Y;
                    transformMatrix.Transform(position, out position);
                    vertex.Position.X = position.X;
                    vertex.Position.Y = position.Y;
                    *(pointer + 1) = vertex;

                    // bottom-left
                    position.X = positionTopLeft.X;
                    position.Y = positionBottomRight.Y;
                    transformMatrix.Transform(position, out position);
                    vertex.Position.X = position.X;
                    vertex.Position.Y = position.Y;
                    *(pointer + 2) = vertex;

                    // bottom-right
                    position = positionBottomRight;
                    transformMatrix.Transform(position, out position);
                    vertex.Position.X = position.X;
                    vertex.Position.Y = position.Y;
                    *(pointer + 3) = vertex;
                }

                var startVertex = VertexCount;
                VertexCount += 4;

                var startIndex = IndexCount;
                EnqueueClockwiseQuadrilateralIndices(startVertex);

                return startIndex;
            }
        }
    }
}
