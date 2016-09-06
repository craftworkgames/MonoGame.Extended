using System;
using System.Collections.Generic;
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

            public unsafe GeometryBuffer.GeometryItem EnqueueSprite(
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

                var vertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color ?? Color.White,
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

                return new GeometryBuffer.GeometryItem(startIndex, 2);
            }

            public unsafe GeometryBuffer.GeometryItem EnqueueRectangle(ref Matrix2D transformMatrix, SizeF size, Color? color = null, float depth = 0)
            {
                ThrowIfWouldOverflow(4, 6);

                Vector2 positionTopLeft, positionBottomRight;

                var halfSize = size * 0.5f;

                positionTopLeft.X = -halfSize.Width;
                positionTopLeft.Y = -halfSize.Height;
                positionBottomRight.X = halfSize.Width;
                positionBottomRight.Y = halfSize.Height;

                var vertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color ?? Color.White,
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

                return new GeometryBuffer.GeometryItem(startIndex, 2);
            }

            internal unsafe GeometryBuffer.GeometryItem EnqueueConvexPolygon(IReadOnlyList<Vector2> points, ref Matrix2D transformMatrix, Color? color, float depth)
            {
                if (points.Count <= 2)
                    return new GeometryBuffer.GeometryItem(0, 0);

                // https://www.siggraph.org/education/materials/HyperGraph/scanline/outprims/polygon1.htm

                var startIndex = IndexCount;
                var startVertex = VertexCount;
                var trianglesCount = points.Count - 2;

                var point = points[0];
                transformMatrix.Transform(point, out point);
                var firstVertex = new VertexPositionColorTexture(new Vector3(point.X, point.Y, depth), color ?? Color.White,
Vector2.Zero);

                point = points[1];
                transformMatrix.Transform(point, out point);
                var secondVertex = new VertexPositionColorTexture(new Vector3(point.X, point.Y, depth), color ?? Color.White,
                Vector2.Zero);

                var thirdVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color ?? Color.White,
                Vector2.Zero);

                fixed (VertexPositionColorTexture* fixedVertexPointer = Vertices)
                {
                    var vertexPointer = fixedVertexPointer + VertexCount;

                    *(vertexPointer++) = firstVertex;
                    *(vertexPointer++) = secondVertex;

                    fixed (ushort* fixedIndexPointer = Indices)
                    {
                        var indexPointer = fixedIndexPointer + IndexCount;

                        for (var index = 2; index < points.Count; ++index)
                        {
                            point = points[index];

                            transformMatrix.Transform(point, out point);
                            thirdVertex.Position.X = point.X;
                            thirdVertex.Position.Y = point.Y;
                            *(vertexPointer++) = thirdVertex;
                            *(indexPointer++) = (ushort)startVertex;
                            *(indexPointer++) = (ushort)(index - 1 + startVertex);
                            *(indexPointer++) = (ushort)(index - 0 + startVertex);
                            
                            secondVertex.Position.X = thirdVertex.Position.X;
                            secondVertex.Position.Y = thirdVertex.Position.Y;
                        }
                    }
                }

                VertexCount += points.Count;
                IndexCount += 3 * trianglesCount;

                return new GeometryBuffer.GeometryItem(startIndex, trianglesCount);
            }
        }
    }
}
