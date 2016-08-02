using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Shapes.Explicit;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Graphics
{
    public static class GeometryBuilderExtensionsVertexPositionColorTexture
    {
        //TODO: Support QuadStrip AND TriangleList primitive type

        private static VertexPositionColorTexture _vertex;

        // to use delegates without creating unecessary memory garbage, we need to "cache" the delegates
        private static readonly ExplicitShapeBuilder.OutputPointDelegate _pointToVertex = PointToGeneralVertex;
        private static readonly ExplicitShapeBuilder.OutputPointDelegate _spritePointToVertex = SpritePointToGeneralVertex;
        private static readonly ExplicitShapeBuilder.OutputPointDelegate _arcPointToTriangleVertex = ArcPointToTriangleVertex;
        private static readonly ExplicitShapeBuilder.OutputPointDelegate _arcPointToLineVertex = ArcPointToLineVertex;

        // these variables are for used in one of the delegate methods
        // they are here because we want to prevent delegate closures which would create a new object on the heap every invocation!
        private static int _vertexIndexCount;
        private static int _vertexIndexOffset;
        private static Vector2 _spriteTextureCoordinateTopLeft;
        private static Vector2 _spriteTextureCoordinateBottomRight;

        private static GeometryBuilder<VertexPositionColorTexture>.OutputVertexDelegate _outputVertex;
        private static GeometryBuilder<VertexPositionColorTexture>.OutputVertexIndexDelegate _outputIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PointToGeneralVertex(ref Vector3 point)
        {
            _vertex.Position = point;
            _outputVertex(ref _vertex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SpritePointToGeneralVertex(ref Vector3 point)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (_vertexIndexCount)
            {
                case 0:
                _vertex.TextureCoordinate = _spriteTextureCoordinateTopLeft;
                break;
                case 1:
                _vertex.TextureCoordinate.X = _spriteTextureCoordinateBottomRight.X;
                _vertex.TextureCoordinate.Y = _spriteTextureCoordinateTopLeft.Y;
                break;
                case 2:
                _vertex.TextureCoordinate.X = _spriteTextureCoordinateTopLeft.X;
                _vertex.TextureCoordinate.Y = _spriteTextureCoordinateBottomRight.Y;
                break;
                case 3:
                _vertex.TextureCoordinate = _spriteTextureCoordinateBottomRight;
                break;
            }

            _vertex.Position = point;
            _outputVertex(ref _vertex);
            _vertexIndexCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ArcPointToTriangleVertex(ref Vector3 point)
        {
            _vertex.Position = point;
            _outputVertex(ref _vertex);
            _vertexIndexCount++;

            // need at least 3 points (the first point is the center of the arc)
            if (!(_vertexIndexCount >= 3))
            {
                return;
            }

            _outputIndex(_vertexIndexOffset + 0); // center of arc/circle
            _outputIndex(_vertexIndexOffset + _vertexIndexCount - 2); // point i-1
            _outputIndex(_vertexIndexOffset + _vertexIndexCount - 1); // point i
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ArcPointToLineVertex(ref Vector3 point)
        {
            _vertex.Position = point;
            _outputVertex(ref _vertex);
            _vertexIndexCount++;

            // need at least 2 points
            if (!(_vertexIndexCount >= 2))
            {
                return;
            }

            _outputIndex(_vertexIndexOffset + _vertexIndexCount - 2); // point i-1
            _outputIndex(_vertexIndexOffset + _vertexIndexCount - 1); // point i
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddClockwiseQuadrilateralIndices(int indexOffset)
        {
            _outputIndex(0 + indexOffset);
            _outputIndex(1 + indexOffset);
            _outputIndex(2 + indexOffset);
            _outputIndex(1 + indexOffset);
            _outputIndex(3 + indexOffset);
            _outputIndex(2 + indexOffset);
        }

        public static void BuildLine(this GeometryBuilder<VertexPositionColorTexture> geometryBuilder, out PrimitiveType primitiveType, ref Line2F line, Color color, float depth = 0f, int indexOffset = 0)
        {
            primitiveType = PrimitiveType.LineList;
            _vertex.Position.Z = depth;
            _vertex.Color = color;
            _vertex.TextureCoordinate = Vector2.Zero;

            _outputVertex = geometryBuilder.OutputVertex;
            _outputIndex = geometryBuilder.OutputIndex;

            _vertex.Position.X = line.FirstPoint.X;
            _vertex.Position.Y = line.FirstPoint.Y;
            _outputVertex(ref _vertex);

            _vertex.Position.X = line.SecondPoint.X;
            _vertex.Position.Y = line.SecondPoint.Y;
            _outputVertex(ref _vertex);

            _outputIndex(0 + indexOffset);
            _outputIndex(1 + indexOffset);
        }

        public static void BuildLine(this GeometryBuilder<VertexPositionColorTexture> geometryBuilder, out PrimitiveType primitiveType, ref Line3F line, Color color, int indexOffset = 0)
        {
            primitiveType = PrimitiveType.LineList;
            _vertex.Color = color;
            _vertex.TextureCoordinate = Vector2.Zero;

            _outputVertex = geometryBuilder.OutputVertex;
            _outputIndex = geometryBuilder.OutputIndex;

            _vertex.Position = line.FirstPoint;
            _outputVertex(ref _vertex);

            _vertex.Position = line.SecondPoint;
            _outputVertex(ref _vertex);

            _outputIndex(0 + indexOffset);
            _outputIndex(1 + indexOffset);
        }

        public static void BuildArc(this GeometryBuilder<VertexPositionColorTexture> geometryBuilder, out PrimitiveType primitiveType, ref ArcF arc, Color color, float depth = 0f, int circleSegmentsCount = ExplicitShapeBuilder.DefaultCircleSegmentsCount, int indexOffset = 0)
        {
            primitiveType = PrimitiveType.TriangleList;
            _vertexIndexCount = 0;
            _vertexIndexOffset = indexOffset;
            _vertex = new VertexPositionColorTexture(new Vector3(arc.Centre, depth), color, Vector2.Zero);

            _outputVertex = geometryBuilder.OutputVertex;
            _outputIndex = geometryBuilder.OutputIndex;

            _outputVertex(ref _vertex);
            _vertexIndexCount++;

            ExplicitShapeBuilder.BuildArc(_arcPointToTriangleVertex, ref arc, depth, circleSegmentsCount);
        }

        public static void BuildArcOutline(this GeometryBuilder<VertexPositionColorTexture> geometryBuilder, out PrimitiveType primitiveType, ref ArcF arc, Color color, float depth = 0f, int circleSegmentsCount = ExplicitShapeBuilder.DefaultCircleSegmentsCount, int indexOffset = 0)
        {
            primitiveType = PrimitiveType.LineList;
            _vertexIndexCount = 0;
            _vertexIndexOffset = indexOffset;
            _vertex = new VertexPositionColorTexture(new Vector3(arc.Centre, depth), color, Vector2.Zero);

            _outputVertex = geometryBuilder.OutputVertex;
            _outputIndex = geometryBuilder.OutputIndex;

            ExplicitShapeBuilder.BuildArc(_arcPointToLineVertex, ref arc, depth, circleSegmentsCount);
        }

        public static void BuildCircle(this GeometryBuilder<VertexPositionColorTexture> geometryBuilder, out PrimitiveType primitiveType, ref CircleF circle, Color color, float depth = 0f, int circleSegmentsCount = ExplicitShapeBuilder.DefaultCircleSegmentsCount, int indexOffset = 0)
        {
            primitiveType = PrimitiveType.TriangleList;
            _vertexIndexCount = 0;
            _vertexIndexOffset = indexOffset;
            _vertex = new VertexPositionColorTexture(new Vector3(circle.Centre, depth), color, Vector2.Zero);

            _outputVertex = geometryBuilder.OutputVertex;
            _outputIndex = geometryBuilder.OutputIndex;

            ExplicitShapeBuilder.BuildCircle(_arcPointToTriangleVertex, ref circle, depth, circleSegmentsCount);
        }

        public static void BuildCircleOutline(this GeometryBuilder<VertexPositionColorTexture> geometryBuilder, out PrimitiveType primitiveType, ref CircleF circle, Color color, float depth = 0f, int circleSegmentsCount = ExplicitShapeBuilder.DefaultCircleSegmentsCount, int indexOffset = 0)
        {
            primitiveType = PrimitiveType.LineList;
            _vertexIndexCount = 0;
            _vertexIndexOffset = indexOffset;
            _vertex = new VertexPositionColorTexture(new Vector3(circle.Centre, depth), color, Vector2.Zero);

            _outputVertex = geometryBuilder.OutputVertex;
            _outputIndex = geometryBuilder.OutputIndex;

            ExplicitShapeBuilder.BuildCircle(_arcPointToLineVertex, ref circle, depth, circleSegmentsCount);

            // close the circle: N points on the circle boundary logically implies N + 1 lines between the N points
            _outputIndex(_vertexIndexCount - 1 + indexOffset);
            _outputIndex(indexOffset);
        }

        public static void BuildRectangle(this GeometryBuilder<VertexPositionColorTexture> geometryBuilder, out PrimitiveType primitiveType, ref RectangleF rectangle, Color color, float depth = 0f, int indexOffset = 0)
        {
            primitiveType = PrimitiveType.TriangleList;
            _vertex.Color = color;
            _vertex.TextureCoordinate = Vector2.Zero;

            var centre = rectangle.Center;

            _outputVertex = geometryBuilder.OutputVertex;
            _outputIndex = geometryBuilder.OutputIndex;

            ExplicitShapeBuilder.BuildRectangle(_pointToVertex, ref rectangle, depth);

            AddClockwiseQuadrilateralIndices(indexOffset);
        }

        public static void BuildRectangleOutline(this GeometryBuilder<VertexPositionColorTexture> geometryBuilder, out PrimitiveType primitiveType, ref RectangleF rectangle, Color color, float depth = 0f, int indexOffset = 0)
        {
            primitiveType = PrimitiveType.LineList;
            _vertex.Color = color;
            _vertex.TextureCoordinate = Vector2.Zero;

            var centre = rectangle.Center;

            _outputVertex = geometryBuilder.OutputVertex;
            _outputIndex = geometryBuilder.OutputIndex;

            ExplicitShapeBuilder.BuildRectangle(_pointToVertex, ref rectangle, depth);

            // bottom: left to right
            _outputIndex(2 + indexOffset);
            _outputIndex(3 + indexOffset);

            // left: top to bottom
            _outputIndex(0 + indexOffset);
            _outputIndex(2 + indexOffset);

            // right: top to bottom
            _outputIndex(1 + indexOffset);
            _outputIndex(3 + indexOffset);

            // top: left to right
            _outputIndex(0 + indexOffset);
            _outputIndex(1 + indexOffset);
        }

        public static void BuildSprite(this GeometryBuilder<VertexPositionColorTexture> geometryBuilder, out PrimitiveType primitiveType, Sprite sprite, float depth = 0, int indexOffset = 0)
        {
            primitiveType = PrimitiveType.TriangleList;

            var textureRegion = sprite.TextureRegion;
            var regionBounds = textureRegion.Bounds;
            SizeF size = regionBounds;

            textureRegion.GetTextureCoordinates(out _spriteTextureCoordinateTopLeft);
            textureRegion.GetTextureCoordinates(new Point(regionBounds.Width, regionBounds.Height), out _spriteTextureCoordinateBottomRight);

            var spriteEffect = sprite.Effect;
            if ((spriteEffect & SpriteEffects.FlipVertically) != 0)
            {
                var temp = _spriteTextureCoordinateBottomRight.Y;
                _spriteTextureCoordinateBottomRight.Y = _spriteTextureCoordinateTopLeft.Y;
                _spriteTextureCoordinateTopLeft.Y = temp;
            }

            if ((spriteEffect & SpriteEffects.FlipHorizontally) != 0)
            {
                var temp = _spriteTextureCoordinateBottomRight.X;
                _spriteTextureCoordinateBottomRight.X = _spriteTextureCoordinateTopLeft.X;
                _spriteTextureCoordinateTopLeft.X = temp;
            }

            _vertex.Color = sprite.Color;
            _vertex.TextureCoordinate = Vector2.Zero;
            _vertexIndexCount = 0;

            _outputVertex = geometryBuilder.OutputVertex;
            _outputIndex = geometryBuilder.OutputIndex;

            var rectangle = new RectangleF(sprite.Position, size);
            ExplicitShapeBuilder.BuildRectangle(_spritePointToVertex, ref rectangle, sprite.Rotation, sprite.Origin, depth);

            AddClockwiseQuadrilateralIndices(indexOffset);
        }
    }
}
