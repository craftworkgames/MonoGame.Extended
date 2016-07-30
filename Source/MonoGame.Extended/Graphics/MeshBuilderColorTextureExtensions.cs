using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Shapes.Explicit;

using TVertexType = Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture;

namespace MonoGame.Extended.Graphics
{
    public static class MeshBuilderColorTextureExtensions
    {
        private static TVertexType _vertex;

        // to use delegates without creating unecessary memory garbage, we need to "cache" the delegates
        private static OutputVertexDelegate<TVertexType> _outputVertex;
        private static OutputVertexIndexDelegate _outputIndex;
        private static readonly ShapeBuilder.PointDelegate _pointToVertex = PointToVertexPositionColorTexture;
        private static readonly ShapeBuilder.PointDelegate _spritePointToVertex = SpritePointToVertexPositionColorTexture;
        private static readonly ShapeBuilder.PointDelegate _arcPointToVertex = ArcPointToVertexPositionColorTexture;

        // these variables are for used in one of the delegate methods
        // they are here because we want to prevent delegate closures which would create a new object on the heap every invocation!
        private static int _vertexIndexCount;
        private static int _vertexIndexOffset;
        private static Vector2 _spriteTextureCoordinateTopLeft;
        private static Vector2 _spriteTextureCoordinateBottomRight;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PointToVertexPositionColorTexture(ref Vector3 point)
        {
            _vertex.Position = point;
            _outputVertex(ref _vertex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SpritePointToVertexPositionColorTexture(ref Vector3 point)
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
        private static void ArcPointToVertexPositionColorTexture(ref Vector3 point)
        {
            _vertex.Position = point;
            _outputVertex(ref _vertex);
            _vertexIndexCount++;

            // need at least 3 points (the first point is the center of the arc)
            if (!(_vertexIndexCount >= 3))
            {
                return;
            }

            _outputIndex(_vertexIndexOffset + 0); // center of arc
            _outputIndex(_vertexIndexOffset + _vertexIndexCount - 2); // point i-1
            _outputIndex(_vertexIndexOffset + _vertexIndexCount - 1); // point i
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddClockwiseQuadrilateralIndices(OutputVertexIndexDelegate outputIndex, int indexOffset)
        {
            outputIndex(0 + indexOffset);
            outputIndex(1 + indexOffset);
            outputIndex(2 + indexOffset);
            outputIndex(1 + indexOffset);
            outputIndex(3 + indexOffset);
            outputIndex(2 + indexOffset);
        }

        public static void Line(this MeshBuilder<TVertexType> meshBuilder, ref Line2F line, float width, Color color, float depth = 0f, int indexOffset = 0)
        {
            _vertex.Position.Z = depth;
            _vertex.Color = color;
            _vertex.TextureCoordinate = Vector2.Zero;

            var vector = line.Vector;
            var perpendicular = vector.PerpendicularClockwise();
            perpendicular.Normalize();

            var halfWidth = width * 0.5f;
            var offset = perpendicular * halfWidth;

            _vertex.Position.X = line.FirstPoint.X + offset.X;
            _vertex.Position.Y = line.FirstPoint.Y + offset.Y;
            meshBuilder.OutputVertex(ref _vertex);

            _vertex.Position.X = line.FirstPoint.X - offset.X;
            _vertex.Position.Y = line.FirstPoint.Y - offset.Y;
            meshBuilder.OutputVertex(ref _vertex);

            _vertex.Position.X = line.SecondPoint.X + offset.X;
            _vertex.Position.Y = line.SecondPoint.Y + offset.Y;
            meshBuilder.OutputVertex(ref _vertex);

            _vertex.Position.X = line.SecondPoint.X - offset.X;
            _vertex.Position.Y = line.SecondPoint.Y - offset.Y;
            meshBuilder.OutputVertex(ref _vertex);

            AddClockwiseQuadrilateralIndices(meshBuilder.OutputIndex, indexOffset);
        }

        public static void Line(this MeshBuilder<TVertexType> meshBuilder, ref Line3F line, float width, Color color, int indexOffset = 0)
        {
            _vertex.Color = color;
            _vertex.TextureCoordinate = Vector2.Zero;

            var firstPoint = new Vector2(line.FirstPoint.X, line.FirstPoint.Y);
            var secondPoint = new Vector2(line.SecondPoint.X, line.SecondPoint.Y);
            var vector = secondPoint - firstPoint;
            var perpendicular = vector.PerpendicularClockwise();
            perpendicular.Normalize();

            var halfWidth = width * 0.5f;
            var offset = perpendicular * halfWidth;

            _vertex.Position.Z = line.FirstPoint.Z;

            _vertex.Position.X = firstPoint.X + offset.X;
            _vertex.Position.Y = firstPoint.Y + offset.Y;
            meshBuilder.OutputVertex(ref _vertex);

            _vertex.Position.X = firstPoint.X - offset.X;
            _vertex.Position.Y = firstPoint.Y - offset.Y;
            meshBuilder.OutputVertex(ref _vertex);

            _vertex.Position.Z = line.SecondPoint.Z;

            _vertex.Position.X = secondPoint.X + offset.X;
            _vertex.Position.Y = secondPoint.Y + offset.Y;
            meshBuilder.OutputVertex(ref _vertex);

            _vertex.Position.X = secondPoint.X - offset.X;
            _vertex.Position.Y = secondPoint.Y - offset.Y;
            meshBuilder.OutputVertex(ref _vertex);

            AddClockwiseQuadrilateralIndices(meshBuilder.OutputIndex, indexOffset);
        }

        public static void Arc(this MeshBuilder<TVertexType> meshBuilder, ref ArcF arc, Color color, float depth = 0f, int circleSegmentsCount = ShapeBuilder.DefaultSegmentsCount, int indexOffset = 0, PrimitiveType primitiveType = PrimitiveType.TriangleStrip)
        {
            _outputVertex = meshBuilder.OutputVertex;
            _outputIndex = meshBuilder.OutputIndex;
            _vertexIndexCount = 0;
            _vertexIndexOffset = indexOffset;
            _vertex = new TVertexType(new Vector3(arc.Centre.X, arc.Centre.Y, depth), color, Vector2.Zero);

            _outputVertex(ref _vertex);
            _vertexIndexCount++;

            ShapeBuilder.Arc(_arcPointToVertex, ref arc, depth, circleSegmentsCount);
        }

//        public static void CreateCircle(VertexDelegate<VertexPositionColorTexture> outputVertex, IndexDelegate outputIndex, Vector2 position, float radius, Color color, float depth = 0f, int circleSegmentsCount = ShapeBuilder.DefaultChordCount)
//        {
//            _outputVertexPositionColorTexture = outputVertex;
//            _outputVertexIndex = outputIndex;
//            _vertexPositionColorTexture.Color = color;
//            _vertexPositionColorTexture.TextureCoordinate = Vector2.Zero;
//
//            ShapeBuilder.CreateCircle(_circlePointToPositionColorTextureDelegate, position, radius, depth, circleSegmentsCount);
//        }

        public static void Rectangle(this MeshBuilder<TVertexType> meshBuilder, ref RectangleF rectangle, Color color, float depth = 0f, int indexOffset = 0)
        {
            _outputVertex = meshBuilder.OutputVertex;
            _outputIndex = meshBuilder.OutputIndex;
            _vertex.Color = color;
            _vertex.TextureCoordinate = Vector2.Zero;

            var centre = rectangle.Center;

            ShapeBuilder.Rectangle(_pointToVertex, new Point2F(centre.X, centre.Y), rectangle.Size, depth);

            AddClockwiseQuadrilateralIndices(meshBuilder.OutputIndex, indexOffset);
        }

        public static void Sprite(this MeshBuilder<TVertexType> meshBuilder, ref Sprite sprite, Texture2D texture, SpriteEffects spriteEffect = SpriteEffects.None, float depth = 0, int indexOffset = 0)
        {
            if (texture == null)
            {
                return;
            }

            var textureSize = new SizeF(texture.Width, texture.Height);

            SizeF size;

            if (sprite.SourceRectangle.HasValue)
            {
                var rectangle = sprite.SourceRectangle.Value;
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

            _outputVertex = meshBuilder.OutputVertex;
            _outputIndex = meshBuilder.OutputIndex;
            _vertex.Color = sprite.Color ?? Color.White;
            _vertex.TextureCoordinate = Vector2.Zero;
            _vertexIndexCount = 0;

            ShapeBuilder.Rectangle(_spritePointToVertex, sprite.Centre, size, depth);

            AddClockwiseQuadrilateralIndices(meshBuilder.OutputIndex, indexOffset);
        }
    }
}
