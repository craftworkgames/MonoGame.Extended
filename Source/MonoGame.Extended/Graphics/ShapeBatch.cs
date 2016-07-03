using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;

namespace MonoGame.Extended.Graphics
{
    public class ShapeBatch : PrimitiveBatch<VertexPositionColorTexture, ShapeBatch.BatchItemData>
    {
        private BatchItemData _shapeItemData;
        private Matrix _defaultWorld;
        private Matrix _defaultView;
        private Matrix _defaultProjection;
        private readonly ShapeEffect _effect;
        private readonly Texture2D _pixelTexture;

        public ShapeBatch(GraphicsDevice graphicsDevice, int maximumVerticesCount = DefaultMaximumVerticesCount, int maximumIndicesCount = DefaultMaximumIndicesCount)
            : base(graphicsDevice, maximumVerticesCount, maximumIndicesCount)
        {
            _effect = new ShapeEffect(graphicsDevice);

            var viewport = graphicsDevice.Viewport;

            _defaultWorld = Matrix.Identity;
            _defaultView = Matrix.Identity;
            _defaultProjection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1);

            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[]
            {
                Color.White
            });

            _shapeItemData = new BatchItemData
            {
                Texture = _pixelTexture
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _shapeItemData.Texture = null;
            _pixelTexture?.Dispose();
        }

        public void Begin(BatchMode batchMode)
        {
            _effect.SetWorld(ref _defaultWorld);
            _effect.SetView(ref _defaultView);
            _effect.SetProjection(ref _defaultProjection);
            Begin(batchMode, _effect);
        }

        public void Begin(BatchMode batchMode, ref Matrix view)
        {
            _effect.SetWorld(ref _defaultWorld);
            _effect.SetView(ref view);
            _effect.SetProjection(ref _defaultProjection);
            Begin(batchMode, _effect);
        }

        public void DrawLine(Vector2 firstPoint, Vector2 secondPoint, Color color, float width = 1f, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            var startVertex = geometryBuffer.VerticesCount;
            var startIndex = geometryBuffer.IndicesCount;

            RenderGeometryBuilder.CreateLine(geometryBuffer.EnqueueVertexDelegate, geometryBuffer.EnqueueVertexIndexDelegate, startVertex, firstPoint, secondPoint, color, width, depth);

            EnqueueDraw(PrimitiveType.TriangleList, 4, startIndex, 6, ref _shapeItemData, sortKey);
        }

        public void DrawLine(Vector3 firstPoint, Vector3 secondPoint, Color color, float width = 1f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            var startVertex = geometryBuffer.VerticesCount;
            var startIndex = geometryBuffer.IndicesCount;

            RenderGeometryBuilder.CreateLine(geometryBuffer.EnqueueVertexDelegate, geometryBuffer.EnqueueVertexIndexDelegate, startVertex, firstPoint, secondPoint, color, width);

            EnqueueDraw(PrimitiveType.TriangleList, 4, startIndex, 6, ref _shapeItemData, sortKey);
        }

//        public void DrawArcOutline(Vector2 position, float radius, float startAngle, float endAngle, Color color, float depth = 0f, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount, uint sortKey = 0)
//        {
//            var startVertex = GeometryBuffer.VerticesCount;
//            var startIndex = GeometryBuffer.IndicesCount;
//
//            RenderGeometryBuilder.CreateArc(GeometryBuffer.EnqueueVertexDelegate, GeometryBuffer.EnqueueVertexIndexDelegate, position, radius, startAngle, endAngle, color, depth, circleSegmentsCount);
//
//            var vertexCount = GeometryBuffer.VerticesCount;
//            var indexCount = GeometryBuffer.IndicesCount;
//
//            EnqueueDraw(PrimitiveType.TriangleList, startVertex, startIndex, vertexCount, indexCount, ref _shapeItemData);
//
//            _structBuffer.Clear();
//            _structBuffer.AppendArc(position, radius, startAngle, endAngle, depth, circleSegmentsCount);
//
//            var points = _structBuffer.Items;
//            var pointsCount = _structBuffer.Length;
//
//            for (var i = 0; i < pointsCount - 1; i++)
//            {
//                DrawLine3D(points[i], points[i + 1], color, sortKey);
//            }
//        }
//
//        public void DrawArc(Vector2 position, float radius, float startAngle, float endAngle, Color color, float depth = 0f, int circleSegmentsCount = StructBuffer<>.DefaultCircleSegmentsCount, uint sortKey = 0)
//        {
//            _structBuffer.Clear();
//            _structBuffer.AppendArc(position, radius, startAngle, endAngle, depth, circleSegmentsCount);
//
//            var points = _structBuffer.Items;
//            var pointsCount = _structBuffer.Length;
//            var position3D = new Vector3(position, depth);
//
//            for (var i = 0; i < pointsCount - 1; i++)
//            {
//                DrawTriangle3D(position3D, points[i], points[i + 1], color, sortKey);
//            } 
//        }
//
//        public void DrawCircleOutline(Vector2 position, float radius, Color color, Vector2? axis = null, float depth = 0f, int circleSegmentsCount = StructBuffer<>.DefaultCircleSegmentsCount, uint sortKey = 0)
//        {
//            _structBuffer.Clear();
//            _structBuffer.AppendCircle(position, radius, depth, circleSegmentsCount);
//
//            var points = _structBuffer.Items;
//            var pointsCount = _structBuffer.Length;
//            var firstPoint = points[0];
//
//            for (var i = 0; i < pointsCount - 1; i++)
//            {
//                DrawLine3D(points[i], points[i + 1], color, sortKey);
//            }
//
//            DrawLine3D(points[pointsCount - 1], firstPoint, color, sortKey);
//
//            if (!axis.HasValue)
//            {
//                return;
//            }
//
//            var axisCirclePosition = new Vector2(position.X + axis.Value.X * radius, position.Y + axis.Value.Y * radius);
//            DrawLine2D(position, axisCirclePosition, color, depth, sortKey);
//        }
//
//        public void DrawCircle(Vector2 position, float radius, Color color, float depth = 0f, int circleSegmentsCount = StructBuffer<>.DefaultCircleSegmentsCount, uint sortKey = 0)
//        {
//            _structBuffer.Clear();
//            _structBuffer.AppendCircle(position, radius, depth, circleSegmentsCount);
//
//            var points = _structBuffer.Items;
//            var pointsCount = _structBuffer.Length;
//            var position3D = new Vector3(position, depth);
//            var firstPoint = points[0];
//
//            for (var i = 0; i < pointsCount - 1; i++)
//            {
//                DrawTriangle3D(position3D, points[i], points[i + 1], color, sortKey);
//            }
//
//            DrawTriangle3D(position3D, points[pointsCount - 1], firstPoint, color, sortKey);
//        }

        public void DrawRectangleOffTopLeft(Vector2 position, SizeF size, Color color, float rotation = 0f, Vector2? origin = null, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            var startVertex = geometryBuffer.VerticesCount;
            var startIndex = geometryBuffer.IndicesCount;

            RenderGeometryBuilder.CreateRectangleOffTopLeft(geometryBuffer.EnqueueVertexDelegate, geometryBuffer.EnqueueVertexIndexDelegate, startVertex, position, size, color, rotation, origin, depth);

            EnqueueDraw(PrimitiveType.TriangleList, 4, startIndex, 6, ref _shapeItemData, sortKey);
        }

        public void DrawRectangleOffCenter(Vector2 position, SizeF size, Color color, float rotation = 0f, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            var startVertex = geometryBuffer.VerticesCount;
            var startIndex = geometryBuffer.IndicesCount;

            RenderGeometryBuilder.CreateRectangleOffCenter(geometryBuffer.EnqueueVertexDelegate, geometryBuffer.EnqueueVertexIndexDelegate, startVertex, position, size, color, rotation, depth);

            EnqueueDraw(PrimitiveType.TriangleList, 4, startIndex, 6, ref _shapeItemData, sortKey);
        }

        public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = 0, uint sortKey = 0)
        {
            if (texture == null)
            {
                return;
            }

            var geometryBuffer = GeometryBuffer;

            var startVertex = geometryBuffer.VerticesCount;
            var startIndex = geometryBuffer.IndicesCount;

            RenderGeometryBuilder.CreateSprite(geometryBuffer.EnqueueVertexDelegate, geometryBuffer.EnqueueVertexIndexDelegate, startVertex, texture, position, sourceRectangle, color, rotation, origin, scale, spriteEffects, depth);

            var itemData = new BatchItemData(texture);
            EnqueueDraw(PrimitiveType.TriangleList, 4, startIndex, 6, ref itemData, sortKey);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BatchItemData : IBatchItemData<BatchItemData>
        {
            public Texture2D Texture;

            internal BatchItemData(Texture2D texture)
            {
                Texture = texture;
            }

            public bool Equals(ref BatchItemData other)
            {
                return other.Texture == Texture;
            }

            public void ApplyTo(Effect effect)
            {
                var textureEffect = effect as ITextureEffect2D;
                if (textureEffect != null)
                {
                    textureEffect.Texture = Texture;
                }
            }
        }
    }
}
