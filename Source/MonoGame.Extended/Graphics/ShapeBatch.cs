using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Shapes.Explicit;

namespace MonoGame.Extended.Graphics
{
    public class ShapeBatch : IDisposable
    {
        //TODO: Create AffineMatrix2D

        private readonly PrimitiveBatch<VertexPositionColorTexture, BatchItemData> _primitiveBatch;
        private BatchItemData _emptyBatchItemData;
        private Matrix _defaultWorld;
        private Matrix _defaultView;
        private Matrix _defaultProjection;
        private readonly ShapeBuilder _shapeBuilder;
        private readonly ShapeEffect _effect;
        private readonly Texture2D _pixelTexture;

        private VertexPositionColorTexture _firstVertex = new VertexPositionColorTexture(Vector3.Zero, Color.White, Vector2.Zero);
        private VertexPositionColorTexture _secondVertex = new VertexPositionColorTexture(Vector3.Zero, Color.White, Vector2.Zero);
        private VertexPositionColorTexture _thirdVertex = new VertexPositionColorTexture(Vector3.Zero, Color.White, Vector2.Zero);
        private VertexPositionColorTexture _fourthVertex = new VertexPositionColorTexture(Vector3.Zero, Color.White, Vector2.Zero);

        public ShapeBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }

            _primitiveBatch = new PrimitiveBatch<VertexPositionColorTexture, BatchItemData>(graphicsDevice);
            _effect = new ShapeEffect(graphicsDevice);
            _shapeBuilder = new ShapeBuilder();

            var viewport = graphicsDevice.Viewport;

            _defaultWorld = Matrix.Identity;
            _defaultView = Matrix.Identity;
            _defaultProjection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1);

            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[]
            {
                Color.White
            });
            _emptyBatchItemData = new BatchItemData
            {
                Texture = _pixelTexture
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _primitiveBatch.Dispose();
            _emptyBatchItemData.Texture = null;
            _pixelTexture?.Dispose();
            _shapeBuilder?.Dispose();
        }

        public void Begin()
        {
            Begin(BatchMode.Deferred);
        }

        public void Begin(BatchMode batchMode)
        {
            _effect.SetWorld(ref _defaultWorld);
            _effect.SetView(ref _defaultView);
            _effect.SetProjection(ref _defaultProjection);
            _primitiveBatch.Begin(_effect, batchMode);
        }

        public void Begin(BatchMode batchMode, Effect effect)
        {
            _primitiveBatch.Begin((Effects.ShapeEffect)effect, batchMode);
        }

        public void Begin(BatchMode batchMode, ref Matrix view)
        {
            _effect.SetWorld(ref _defaultWorld);
            _effect.SetView(ref view);
            _effect.SetProjection(ref _defaultProjection);
            _primitiveBatch.Begin(_effect, batchMode);
        }

        public void End()
        {
            _primitiveBatch.End();
        }

        public void DrawLine2D(Vector2 firstPoint, Vector2 secondPoint, Color color, float depth = 0f, uint sortKey = 0)
        {
            _firstVertex.Position = new Vector3(firstPoint, depth);
            _secondVertex.Position = new Vector3(secondPoint, depth);
            _firstVertex.Color = _secondVertex.Color = color;
            _firstVertex.TextureCoordinate = _secondVertex.TextureCoordinate = Vector2.Zero;
            _primitiveBatch.DrawLine(ref _firstVertex, ref _secondVertex, ref _emptyBatchItemData, sortKey);
        }

        public void DrawLine3D(Vector3 firstPoint, Vector3 secondPoint, Color? color = null, uint sortKey = 0)
        {
            _firstVertex.Position = firstPoint;
            _secondVertex.Position = secondPoint;
            _firstVertex.Color = _secondVertex.Color = color ?? Color.White;
            _firstVertex.TextureCoordinate = _secondVertex.TextureCoordinate = Vector2.Zero;
            _primitiveBatch.DrawLine(ref _firstVertex, ref _secondVertex, ref _emptyBatchItemData, sortKey);
        }

        public void DrawTriangle2D(Vector2 firstPoint, Vector2 secondPoint, Vector2 thirdPoint, Color color, float depth = 0f, uint sortKey = 0)
        {
            _firstVertex.Position = new Vector3(firstPoint, depth);
            _secondVertex.Position = new Vector3(secondPoint, depth);
            _thirdVertex.Position = new Vector3(thirdPoint, depth);
            _firstVertex.Color = _secondVertex.Color = _thirdVertex.Color = color;
            _firstVertex.TextureCoordinate = _secondVertex.TextureCoordinate = _thirdVertex.TextureCoordinate = Vector2.Zero;
            _primitiveBatch.DrawTriangle(ref _firstVertex, ref _secondVertex, ref _thirdVertex, ref _emptyBatchItemData, sortKey);
        }

        public void DrawTriangle3D(Vector3 firstPoint, Vector3 secondPoint, Vector3 thirdPoint, Color color, uint sortKey = 0)
        {
            _firstVertex.Position = firstPoint;
            _secondVertex.Position = secondPoint;
            _thirdVertex.Position = thirdPoint;
            _firstVertex.Color = _secondVertex.Color = _thirdVertex.Color = color;
            _firstVertex.TextureCoordinate = _secondVertex.TextureCoordinate = _thirdVertex.TextureCoordinate = Vector2.Zero;
            _primitiveBatch.DrawTriangle(ref _firstVertex, ref _secondVertex, ref _thirdVertex, ref _emptyBatchItemData, sortKey);
        }

        public void DrawQuadrilateral2D(Vector2 firstPoint, Vector2 secondPoint, Vector2 thirdPoint, Vector2 fourthPoint, Color color, float depth = 0f, uint sortKey = 0)
        {
            _firstVertex.Position = new Vector3(firstPoint, depth);
            _secondVertex.Position = new Vector3(secondPoint, depth);
            _thirdVertex.Position = new Vector3(thirdPoint, depth);
            _fourthVertex.Position = new Vector3(fourthPoint, depth);
            _firstVertex.Color = _secondVertex.Color = _thirdVertex.Color = _fourthVertex.Color = color;
            _firstVertex.TextureCoordinate = _secondVertex.TextureCoordinate = _thirdVertex.TextureCoordinate = _fourthVertex.TextureCoordinate = Vector2.Zero;
            _primitiveBatch.DrawQuadrilateral(ref _firstVertex, ref _secondVertex, ref _thirdVertex, ref _fourthVertex, ref _emptyBatchItemData, sortKey);
        }

        public void DrawQuadrilateral3D(Vector3 firstPoint, Vector3 secondPoint, Vector3 thirdPoint, Vector3 fourthPoint, Color color, uint sortKey = 0)
        {
            _firstVertex.Position = firstPoint;
            _secondVertex.Position = secondPoint;
            _thirdVertex.Position = thirdPoint;
            _fourthVertex.Position = fourthPoint;
            _firstVertex.Color = _secondVertex.Color = _thirdVertex.Color = color;
            _firstVertex.TextureCoordinate = _secondVertex.TextureCoordinate = _thirdVertex.TextureCoordinate = _fourthVertex.TextureCoordinate = Vector2.Zero;
            _primitiveBatch.DrawQuadrilateral(ref _firstVertex, ref _secondVertex, ref _thirdVertex, ref _fourthVertex, ref _emptyBatchItemData, sortKey);
        }

        public void DrawPolygonLine(IReadOnlyList<Vector2> points, Color color, float depth = 0f, uint sortKey = 0)
        {
            if (points.Count == 0)
            {
                return;
            }

            for (var i = 0; i < points.Count - 1; i++)
            {
                DrawLine2D(points[i], points[i + 1], color, depth, sortKey);
            }
        }

        public void DrawPolygonOutline(IReadOnlyList<Vector2> points, Color color, float depth = 0f, uint sortKey = 0)
        {
            if (points.Count == 0)
            {
                return;
            }

            for (var i = 0; i < points.Count - 1; i++)
            {
                DrawLine2D(points[i], points[i + 1], color, depth, sortKey);
            }

            DrawLine2D(points[points.Count - 1], points[0], color, depth, sortKey);
        }

        public void DrawPolygon(IReadOnlyList<Vector2> points, Color color, float depth = 0f, uint sortKey = 0)
        {
            if (points.Count == 0)
            {
                return;
            }

            var firstPoint = points[0];

            for (var i = 1; i < points.Count - 1; i++)
            {
                DrawTriangle2D(firstPoint, points[i], points[i + 1], color, depth, sortKey);
            }
        }

        public void DrawArcOutline(Vector2 position, float radius, float startAngle, float endAngle, Color color, float depth = 0, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount, uint sortKey = 0)
        {
            _shapeBuilder.Clear();
            _shapeBuilder.AppendArc(position, radius, startAngle, endAngle, depth, circleSegmentsCount);

            var points = _shapeBuilder.Buffer;
            var pointsCount = _shapeBuilder.Count;

            for (var i = 0; i < pointsCount - 1; i++)
            {
                DrawLine3D(points[i], points[i + 1], color, sortKey);
            }
        }

        public void DrawArc(Vector2 position, float radius, float startAngle, float endAngle, Color color, float depth = 0, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount, uint sortKey = 0)
        {
            _shapeBuilder.Clear();
            _shapeBuilder.AppendArc(position, radius, startAngle, endAngle, depth, circleSegmentsCount);

            var points = _shapeBuilder.Buffer;
            var pointsCount = _shapeBuilder.Count;
            var position3D = new Vector3(position, depth);

            for (var i = 0; i < pointsCount - 1; i++)
            {
                DrawTriangle3D(position3D, points[i], points[i + 1], color, sortKey);
            } 
        }

        public void DrawCircleOutline(Vector2 position, float radius, Color color, Vector2? axis = null, float depth = 0, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount, uint sortKey = 0)
        {
            _shapeBuilder.Clear();
            _shapeBuilder.AppendCircle(position, radius, depth, circleSegmentsCount);

            var points = _shapeBuilder.Buffer;
            var pointsCount = _shapeBuilder.Count;
            var firstPoint = points[0];

            for (var i = 0; i < pointsCount - 1; i++)
            {
                DrawLine3D(points[i], points[i + 1], color, sortKey);
            }

            DrawLine3D(points[pointsCount - 1], firstPoint, color, sortKey);

            if (!axis.HasValue)
            {
                return;
            }

            var axisCirclePosition = new Vector2(position.X + axis.Value.X * radius, position.Y + axis.Value.Y * radius);
            DrawLine2D(position, axisCirclePosition, color, depth, sortKey);
        }

        public void DrawCircle(Vector2 position, float radius, Color color, float depth = 0, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount, uint sortKey = 0)
        {
            _shapeBuilder.Clear();
            _shapeBuilder.AppendCircle(position, radius, depth, circleSegmentsCount);

            var points = _shapeBuilder.Buffer;
            var pointsCount = _shapeBuilder.Count;
            var position3D = new Vector3(position, depth);
            var firstPoint = points[0];

            for (var i = 0; i < pointsCount - 1; i++)
            {
                DrawTriangle3D(position3D, points[i], points[i + 1], color, sortKey);
            }

            DrawTriangle3D(position3D, points[pointsCount - 1], firstPoint, color, sortKey);
        }

        internal struct BatchItemData : IBatchItemData<BatchItemData>
        {
            internal Texture2D Texture;

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
