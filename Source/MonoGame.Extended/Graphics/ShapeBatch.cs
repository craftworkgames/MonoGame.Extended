using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Shapes.Explicit;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Graphics
{
    public class ShapeBatch : PrimitiveBatch<VertexPositionColorTexture, ShapeBatch.BatchItemData>
    {
        public const ushort DefaultMaximumVerticesCount = 8192;
        public const ushort DefaultMaximumIndicesCount = 12288;

        private readonly GeometryBuilder<VertexPositionColorTexture> _geometryBuilder;
        private BatchItemData _shapeItemData;
        private Matrix _defaultWorld;
        private Matrix _defaultView;
        private Matrix _defaultProjection;
        private readonly ShapeEffect _effect;
        private readonly Texture2D _pixelTexture;

        public int CircleSegmentsCount { get; set; } = ExplicitShapeBuilder.DefaultCircleSegmentsCount;

        public ShapeBatch(GraphicsDevice graphicsDevice, int maximumVerticesCount = DefaultMaximumVerticesCount, int maximumIndicesCount = DefaultMaximumIndicesCount)
            : base(new GeometryBuffer<VertexPositionColorTexture>(graphicsDevice, GeometryBufferType.Dynamic, maximumVerticesCount, maximumIndicesCount))
        {
            _geometryBuilder = new GeometryBuilder<VertexPositionColorTexture>(GeometryBuffer.Enqueue, GeometryBuffer.Enqueue);

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

        public void Draw(ref Line2F line, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildLine(out primitiveType, ref line, color, depth, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItemData, sortKey);
        }

        public void Draw(ref Line3F line, Color color, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildLine(out primitiveType, ref line, color, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItemData, sortKey);
        }

        public void Draw(ref ArcF arc, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildArc(out primitiveType, ref arc, color, depth, CircleSegmentsCount, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItemData, sortKey);
        }

        public void DrawOutline(ref ArcF arc, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildArcOutline(out primitiveType, ref arc, color, depth, CircleSegmentsCount, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItemData, sortKey);
        }

        public void Draw(ref CircleF circle, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildCircle(out primitiveType, ref circle, color, depth, CircleSegmentsCount, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItemData, sortKey);
        }

        public void DrawOutline(ref CircleF circle, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildCircleOutline(out primitiveType, ref circle, color, depth, CircleSegmentsCount, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItemData, sortKey);
        }

        //TODO: Idea: Apply transformation when filling vertex buffer

        public void Draw(ref RectangleF rectangle, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildRectangle(out primitiveType, ref rectangle, color, depth, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItemData, sortKey);
        }

        public void DrawOutline(ref RectangleF rectangle, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildRectangleOutline(out primitiveType, ref rectangle, color, depth, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItemData, sortKey);
        }

        public void Draw(Sprite sprite, float depth = 0, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildSprite(out primitiveType, sprite, depth, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            var itemData = new BatchItemData(sprite.TextureRegion.Texture);

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref itemData, sortKey);
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
