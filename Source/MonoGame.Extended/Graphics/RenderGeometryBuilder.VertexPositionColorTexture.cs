using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes.Explicit;

namespace MonoGame.Extended.Graphics
{
    public static partial class RenderGeometryBuilder
    {
        private static VertexPositionColorTexture _vertexPositionColorTexture;

        // to use delegates without creating unecessary memory garbage, we need to "cache" the delegates
        private static VertexDelegate<VertexPositionColorTexture> _outputVertexPositionColorTexture;
        private static readonly ShapeBuilder.PointDelegate _pointToVertexPositionColorTextureDelegate = PointToVertexPositionColorTexture;
        private static readonly ShapeBuilder.PointDelegate _spritePointToPositionColorTextureDelegate = SpritePointToVertexPositionColorTexture;
        private static readonly ShapeBuilder.PointDelegate _arcPointToPositionColorTextureDelegate = ArcPointToVertexPositionColorTexture;

        // these variables are for used in one of the delegate methods
        // they are here because we want to prevent delegate closures which would create a new object on the heap every invocation!
        private static int _vertexIndexCount;
        private static int _vertexIndexOffset;
        private static Vector2 _spriteTextureCoordinateTopLeft;
        private static Vector2 _spriteTextureCoordinateBottomRight;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PointToVertexPositionColorTexture(ref Vector3 point)
        {
            _vertexPositionColorTexture.Position = point;
            _outputVertexPositionColorTexture(ref _vertexPositionColorTexture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SpritePointToVertexPositionColorTexture(ref Vector3 point)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (_vertexIndexCount)
            {
                case 0:
                    _vertexPositionColorTexture.TextureCoordinate = _spriteTextureCoordinateTopLeft;
                    break;
                case 1:
                    _vertexPositionColorTexture.TextureCoordinate.X = _spriteTextureCoordinateBottomRight.X;
                    _vertexPositionColorTexture.TextureCoordinate.Y = _spriteTextureCoordinateTopLeft.Y;
                    break;
                case 2:
                    _vertexPositionColorTexture.TextureCoordinate.X = _spriteTextureCoordinateTopLeft.X;
                    _vertexPositionColorTexture.TextureCoordinate.Y = _spriteTextureCoordinateBottomRight.Y;
                    break;
                case 3:
                    _vertexPositionColorTexture.TextureCoordinate = _spriteTextureCoordinateBottomRight;
                    break;
            }

            _vertexPositionColorTexture.Position = point;
            _outputVertexPositionColorTexture(ref _vertexPositionColorTexture);
            _vertexIndexCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ArcPointToVertexPositionColorTexture(ref Vector3 point)
        {
            _vertexPositionColorTexture.Position = point;
            _outputVertexPositionColorTexture(ref _vertexPositionColorTexture);
            _vertexIndexCount++;

            // need at least 3 points (the first point is the center of the arc)
            if (!(_vertexIndexCount >= 3))
            {
                return;
            }

            _outputVertexIndex(_vertexIndexOffset + 0); // center of arc
            _outputVertexIndex(_vertexIndexOffset + _vertexIndexCount - 2); // point i-1
            _outputVertexIndex(_vertexIndexOffset + _vertexIndexCount - 1); // point i
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddClockwiseQuadrilateralIndices(VertexIndexDelegate outputIndex, int indexOffset)
        {
            outputIndex(0 + indexOffset);
            outputIndex(1 + indexOffset);
            outputIndex(2 + indexOffset);
            outputIndex(1 + indexOffset);
            outputIndex(3 + indexOffset);
            outputIndex(2 + indexOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureOutputVertexDelegate(VertexDelegate<VertexPositionColorTexture> outputVertex)
        {
            if (outputVertex == null)
            {
                throw new ArgumentNullException(nameof(outputVertex));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureOutputIndexDelegate(VertexIndexDelegate outputIndex)
        {
            if (outputIndex == null)
            {
                throw new ArgumentNullException(nameof(outputIndex));
            }
        }

        public static void CreateLine(VertexDelegate<VertexPositionColorTexture> outputVertex, VertexIndexDelegate outputIndex, int indexOffset, Vector2 firstPoint, Vector2 secondPoint, Color color, float width = 1f, float depth = 0f)
        {
            EnsureOutputVertexDelegate(outputVertex);
            EnsureOutputIndexDelegate(outputIndex);

            _vertexPositionColorTexture.Position.Z = depth;
            _vertexPositionColorTexture.Color = color;
            _vertexPositionColorTexture.TextureCoordinate = Vector2.Zero;

            var vector = secondPoint - firstPoint;
            var perpendicular = vector.PerpendicularClockwise();
            perpendicular.Normalize();

            var halfWidth = width * 0.5f;
            var offset = perpendicular * halfWidth;

            _vertexPositionColorTexture.Position.X = firstPoint.X + offset.X;
            _vertexPositionColorTexture.Position.Y = firstPoint.Y + offset.Y;
            outputVertex(ref _vertexPositionColorTexture);

            _vertexPositionColorTexture.Position.X = firstPoint.X - offset.X;
            _vertexPositionColorTexture.Position.Y = firstPoint.Y - offset.Y;
            outputVertex(ref _vertexPositionColorTexture);

            _vertexPositionColorTexture.Position.X = secondPoint.X + offset.X;
            _vertexPositionColorTexture.Position.Y = secondPoint.Y + offset.Y;
            outputVertex(ref _vertexPositionColorTexture);

            _vertexPositionColorTexture.Position.X = secondPoint.X - offset.X;
            _vertexPositionColorTexture.Position.Y = secondPoint.Y - offset.Y;
            outputVertex(ref _vertexPositionColorTexture);

            AddClockwiseQuadrilateralIndices(outputIndex, indexOffset);
        }

        public static void CreateLine(VertexDelegate<VertexPositionColorTexture> outputVertex, VertexIndexDelegate outputIndex, int indexOffset, Vector3 firstPoint, Vector3 secondPoint, Color color, float width = 1f)
        {
            EnsureOutputVertexDelegate(outputVertex);
            EnsureOutputIndexDelegate(outputIndex);

            _vertexPositionColorTexture.Color = color;
            _vertexPositionColorTexture.TextureCoordinate = Vector2.Zero;

            var firstPoint2D = new Vector2(firstPoint.X, firstPoint.Y);
            var secondPoint2D = new Vector2(secondPoint.X, secondPoint.Y);
            var vector = secondPoint2D - firstPoint2D;
            var perpendicular = vector.PerpendicularClockwise();
            perpendicular.Normalize();

            var halfWidth = width * 0.5f;
            var offset = perpendicular * halfWidth;

            _vertexPositionColorTexture.Position.Z = firstPoint.Z;

            _vertexPositionColorTexture.Position.X = firstPoint.X + offset.X;
            _vertexPositionColorTexture.Position.Y = firstPoint.Y + offset.Y;
            outputVertex(ref _vertexPositionColorTexture);

            _vertexPositionColorTexture.Position.X = firstPoint.X - offset.X;
            _vertexPositionColorTexture.Position.Y = firstPoint.Y - offset.Y;
            outputVertex(ref _vertexPositionColorTexture);

            _vertexPositionColorTexture.Position.Z = secondPoint.Z;

            _vertexPositionColorTexture.Position.X = secondPoint.X + offset.X;
            _vertexPositionColorTexture.Position.Y = secondPoint.Y + offset.Y;
            outputVertex(ref _vertexPositionColorTexture);

            _vertexPositionColorTexture.Position.X = secondPoint.X - offset.X;
            _vertexPositionColorTexture.Position.Y = secondPoint.Y - offset.Y;
            outputVertex(ref _vertexPositionColorTexture);

            AddClockwiseQuadrilateralIndices(outputIndex, indexOffset);
        }

        public static void CreateArc(VertexDelegate<VertexPositionColorTexture> outputVertex, VertexIndexDelegate outputIndex, int indexOffset, Vector2 position, float radius, float startAngle, float endAngle, Color color, float depth = 0f, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount)
        {
            EnsureOutputVertexDelegate(outputVertex);
            EnsureOutputIndexDelegate(outputIndex);

            _outputVertexPositionColorTexture = outputVertex;
            _outputVertexIndex = outputIndex;
            _vertexIndexCount = 0;
            _vertexIndexOffset = indexOffset;
            _vertexPositionColorTexture = new VertexPositionColorTexture(new Vector3(position, depth), color, Vector2.Zero);

            outputVertex(ref _vertexPositionColorTexture);
            _vertexIndexCount++;

            ShapeBuilder.CreateArc(_arcPointToPositionColorTextureDelegate, position, radius, startAngle, endAngle, depth, circleSegmentsCount);
        }
//
//        public static void CreateCircle(VertexDelegate<VertexPositionColorTexture> outputVertex, VertexIndexDelegate outputIndex, Vector2 position, float radius, Color color, float depth = 0f, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount)
//        {
//            _outputVertexPositionColorTexture = outputVertex;
//            _outputVertexIndex = outputIndex;
//            _vertexPositionColorTexture.Color = color;
//            _vertexPositionColorTexture.TextureCoordinate = Vector2.Zero;
//
//            var vertexDelegate = outputIndex == null ? _shapePointToVertexPositionColorTextureDelegate : _shapePointToIndexedVertexPositionColorTextureDelegate;
//
//            ShapeBuilder.CreateCircle(vertexDelegate, position, radius, depth, circleSegmentsCount);
//        }

        public static void CreateRectangleOffCenter(VertexDelegate<VertexPositionColorTexture> outputVertex, VertexIndexDelegate outputIndex, int indexOffset, Vector2 position, SizeF size, Color color, float rotation = 0f, float depth = 0f)
        {
            EnsureOutputVertexDelegate(outputVertex);
            EnsureOutputIndexDelegate(outputIndex);

            _outputVertexPositionColorTexture = outputVertex;
            _outputVertexIndex = outputIndex;
            _vertexPositionColorTexture.Color = color;
            _vertexPositionColorTexture.TextureCoordinate = Vector2.Zero;

            ShapeBuilder.CreateRectangleFromCenter(_pointToVertexPositionColorTextureDelegate, position, size, rotation, depth);

            AddClockwiseQuadrilateralIndices(outputIndex, indexOffset);
        }

        public static void CreateRectangleOutlineOffCenter(VertexDelegate<VertexPositionColorTexture> outputVertex, VertexIndexDelegate outputIndex, int indexOffset, Vector2 position, SizeF size, Color color, float rotation = 0f, float depth = 0f)
        {
            EnsureOutputVertexDelegate(outputVertex);
            EnsureOutputIndexDelegate(outputIndex);

            _outputVertexPositionColorTexture = outputVertex;
            _outputVertexIndex = outputIndex;
            _vertexPositionColorTexture.Color = color;
            _vertexPositionColorTexture.TextureCoordinate = Vector2.Zero;

            ShapeBuilder.CreateRectangleFromCenter(_pointToVertexPositionColorTextureDelegate, position, size, rotation, depth);

            outputIndex(0);
            outputIndex(1);
            outputIndex(0);
            outputIndex(2);
            outputIndex(1);
            outputIndex(3);
            outputIndex(2);
            outputIndex(3);
        }

        public static void CreateRectangleOffTopLeft(VertexDelegate<VertexPositionColorTexture> outputVertex, VertexIndexDelegate outputIndex, int indexOffset, Vector2 position, SizeF size, Color color, float rotation = 0f, Vector2? origin = null, float depth = 0f)
        {
            EnsureOutputVertexDelegate(outputVertex);
            EnsureOutputIndexDelegate(outputIndex);

            _outputVertexPositionColorTexture = outputVertex;
            _outputVertexIndex = outputIndex;
            _vertexPositionColorTexture.Color = color;
            _vertexPositionColorTexture.TextureCoordinate = Vector2.Zero;

            ShapeBuilder.CreateRectangleFromTopLeft(_pointToVertexPositionColorTextureDelegate, position, size, rotation, origin, depth);

            AddClockwiseQuadrilateralIndices(outputIndex, indexOffset);
        }

        public static void CreateRectangleOutlineOffTopLeft(VertexDelegate<VertexPositionColorTexture> outputVertex, VertexIndexDelegate outputIndex, int indexOffset, Vector2 position, SizeF size, Color color, float rotation = 0f, Vector2? origin = null, float depth = 0f)
        {
            EnsureOutputVertexDelegate(outputVertex);
            EnsureOutputIndexDelegate(outputIndex);

            _outputVertexPositionColorTexture = outputVertex;
            _outputVertexIndex = outputIndex;
            _vertexPositionColorTexture.Color = color;
            _vertexPositionColorTexture.TextureCoordinate = Vector2.Zero;

            ShapeBuilder.CreateRectangleFromTopLeft(_pointToVertexPositionColorTextureDelegate, position, size, rotation, origin, depth);

            AddClockwiseQuadrilateralIndices(outputIndex, indexOffset);
        }

        public static void CreateSprite(VertexDelegate<VertexPositionColorTexture> outputVertex, VertexIndexDelegate outputIndex, int indexOffset, Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = 0)
        {
            EnsureOutputVertexDelegate(outputVertex);
            EnsureOutputIndexDelegate(outputIndex);

            if (texture == null)
            {
                return;
            }

            var origin1 = origin ?? Vector2.Zero;
            var scale1 = scale ?? Vector2.One;
            var textureSize = new SizeF(texture.Width, texture.Height);

            SizeF size;

            if (sourceRectangle.HasValue)
            {
                var rectangle = sourceRectangle.Value;
                size = rectangle;
                _spriteTextureCoordinateTopLeft.X = rectangle.X / textureSize.Width;
                _spriteTextureCoordinateTopLeft.Y = rectangle.Y / textureSize.Height;
                _spriteTextureCoordinateBottomRight.X = (rectangle.X + rectangle.Width) / textureSize.Width;
                _spriteTextureCoordinateBottomRight.Y = (rectangle.Y + rectangle.Height) / textureSize.Height;
            }
            else
            {
                _spriteTextureCoordinateTopLeft = Vector2.Zero;
                _spriteTextureCoordinateBottomRight = Vector2.One;
                size = textureSize;
            }

            origin1 = origin1 * scale1;
            size = size * scale1;

            _outputVertexPositionColorTexture = outputVertex;
            _outputVertexIndex = outputIndex;
            _vertexPositionColorTexture.Color = color ?? Color.White;
            _vertexPositionColorTexture.TextureCoordinate = Vector2.Zero;
            _vertexIndexCount = 0;

            ShapeBuilder.CreateRectangleFromTopLeft(_spritePointToPositionColorTextureDelegate, position, size, rotation, origin1, depth);

            AddClockwiseQuadrilateralIndices(outputIndex, indexOffset);
        }
    }
}
