using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Shapes.Explicit;
using MonoGame.Extended.Shapes.Triangulation;

namespace MonoGame.Extended.Graphics
{
    public class ShapeBatch : PrimitiveBatch<VertexPositionColorTexture, ShapeBatch.BatchItemData>
    {
        private readonly MeshBuilder<VertexPositionColorTexture> _meshBuilder;
        private BatchItemData _shapeItemData;
        private Matrix _defaultWorld;
        private Matrix _defaultView;
        private Matrix _defaultProjection;
        private readonly ShapeEffect _effect;
        private readonly Texture2D _pixelTexture;

        private IPolygonTriangulator _polygonTriangulator;

        public int CircleSegmentsCount { get; set; } = ShapeBuilder.DefaultSegmentsCount;

        public ShapeBatch(GraphicsDevice graphicsDevice, int maximumVerticesCount = DefaultMaximumVerticesCount, int maximumIndicesCount = DefaultMaximumIndicesCount, IPolygonTriangulator polygonTriangulator = null)
            : base(graphicsDevice, maximumVerticesCount, maximumIndicesCount)
        {
            _meshBuilder = new MeshBuilder<VertexPositionColorTexture>(MeshBuffer.EnqueueVertexDelegate, MeshBuffer.EnqueueIndexDelegate);

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

            _polygonTriangulator = polygonTriangulator ?? new EarClipPolygonTriangulator();
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

        public void DrawLine(ref Line2F line, float width, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = MeshBuffer;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _meshBuilder.Line(ref line, width, color, depth, startVertex);

            EnqueueDraw(PrimitiveType.TriangleList, 4, startIndex, 6, ref _shapeItemData, sortKey);
        }

        public void DrawLine(ref Line3F line, float width, Color color, uint sortKey = 0)
        {
            var geometryBuffer = MeshBuffer;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _meshBuilder.Line(ref line, width, color, startVertex);

            EnqueueDraw(PrimitiveType.TriangleList, 4, startIndex, 6, ref _shapeItemData, sortKey);
        }

        public void DrawArc(ref ArcF arc, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = MeshBuffer;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _meshBuilder.Arc(ref arc, color, depth, CircleSegmentsCount, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(PrimitiveType.TriangleList, vertexCount, startIndex, indexCount, ref _shapeItemData, sortKey);
        }

        //
        //        public void DrawCircleOutline(Vector2 position, float radius, Color color, Vector2? axis = null, float depth = 0f, int circleSegmentsCount = StructBuffer<>.DefaultChordCount, uint sortKey = 0)
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
        //        public void DrawCircle(Vector2 position, float radius, Color color, float depth = 0f, int circleSegmentsCount = StructBuffer<>.DefaultChordCount, uint sortKey = 0)
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

            //TODO: Idea: Apply transformation when filling vertex buffer

        public void DrawRectangle(ref RectangleF rectangle, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = MeshBuffer;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _meshBuilder.Rectangle(ref rectangle, color, depth, startVertex);

            EnqueueDraw(PrimitiveType.TriangleList, 4, startIndex, 6, ref _shapeItemData, sortKey);
        }

        public void DrawSprite(ref Sprite sprite, Texture2D texture, SpriteEffects spriteEffects = SpriteEffects.None, float depth = 0, uint sortKey = 0)
        {
            if (texture == null)
            {
                return;
            }

            var geometryBuffer = MeshBuffer;

            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _meshBuilder.Sprite(ref sprite, texture, spriteEffects, depth, startVertex);

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
