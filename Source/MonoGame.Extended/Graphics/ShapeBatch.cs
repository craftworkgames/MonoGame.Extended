using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Shapes.Explicit;

namespace MonoGame.Extended.Graphics
{
    public class ShapeBatch
    {
        private readonly PrimitiveBatch<VertexPositionColorTexture, BatchItemData, Effect> _primitiveBatch;
        private BatchItemData _emptyBatchItemData;
        private Matrix _defaultWorld;
        private Matrix _defaultView;
        private Matrix _defaultProjection;
        private readonly ShapeBuilder _shapeBuilder;
        private readonly ShapeBatchEffect _effect;

        public ShapeBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }

            _primitiveBatch = new PrimitiveBatch<VertexPositionColorTexture, BatchItemData, Effect>(graphicsDevice);
            _effect = new ShapeBatchEffect(graphicsDevice);
            _shapeBuilder = new ShapeBuilder();

            var viewport = graphicsDevice.Viewport;

            _defaultWorld = Matrix.Identity;
            _defaultView = Matrix.Identity;
            _defaultProjection = Matrix.CreateTranslation(xPosition: -0.5f, yPosition: -0.5f, zPosition: 0) * Matrix.CreateOrthographicOffCenter(left: 0, right: viewport.Width, bottom: viewport.Height, top: 0, zNearPlane: 0, zFarPlane: -1);
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
            _primitiveBatch.Begin(effect, batchMode);
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

        public void DrawLine(Vector2 firstPoint, Vector2 secondPoint, Color? color = null, float depth = 0f, uint sortKey = 0)
        {
            var color1 = color ?? Color.White;
            var firstVertex = new VertexPositionColorTexture(new Vector3(x: 0, y: 0, z: depth), color1, Vector2.Zero);
            var secondVertex = new VertexPositionColorTexture(new Vector3(x: 0, y: 0, z: depth), color1, Vector2.Zero);
            _primitiveBatch.DrawLine(ref firstVertex, ref secondVertex, ref _emptyBatchItemData, sortKey);
        }

        public void DrawPolygonOutline(IReadOnlyList<Vector2> points, Color? color = null, float depth = 0f, uint sortKey = 0)
        {
            if (points.Count == 0)
            {
                return;
            }

            var color1 = color ?? Color.White;

            Vector2 firstPoint;
            Vector2 secondPoint;
            var firstVertex = new VertexPositionColorTexture(new Vector3(x: 0, y: 0, z: depth), color1, Vector2.Zero);
            var secondVertex = new VertexPositionColorTexture(new Vector3(x: 0, y: 0, z: depth), color1, Vector2.Zero);

            for (var i = 0; i < points.Count - 1; i++)
            {
                firstPoint = points[i];
                secondPoint = points[i + 1];

                firstVertex.Position.X = firstPoint.X;
                firstVertex.Position.Y = firstPoint.Y;
                secondVertex.Position.X = secondPoint.X;
                secondVertex.Position.Y = secondPoint.Y;

                _primitiveBatch.DrawLine(ref firstVertex, ref secondVertex, ref _emptyBatchItemData, sortKey);
            }

            firstPoint = points[points.Count - 1];
            secondPoint = points[0];

            firstVertex.Position.X = firstPoint.X;
            firstVertex.Position.Y = firstPoint.Y;
            secondVertex.Position.X = secondPoint.X;
            secondVertex.Position.Y = secondPoint.Y;

            //_primitiveBatch.DrawLine(ref firstVertex, ref secondVertex, ref _emptyBatchItemData, sortKey);
        }

        public void DrawPolygon(IReadOnlyList<Vector2> points, Color? color = null, float depth = 0f, uint sortKey = 0)
        {
            if (points.Count == 0)
            {
                return;
            }

            var color1 = color ?? Color.White;

            var firstVertex = new VertexPositionColorTexture(new Vector3(points[0], depth), color1, Vector2.Zero);
            var secondVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);
            var thirdVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);

            for (var i = 1; i < points.Count - 1; i++)
            {
                var secondPoint = points[i];
                var thirdPoint = points[i + 1];

                secondVertex.Position.X = secondPoint.X;
                secondVertex.Position.Y = secondPoint.Y;
                thirdVertex.Position.X = thirdPoint.X;
                thirdVertex.Position.Y = thirdPoint.Y;

                _primitiveBatch.DrawTriangle(ref firstVertex, ref secondVertex, ref thirdVertex, ref _emptyBatchItemData, sortKey);
            }
        }

        public void DrawArcOutline(Vector2 position, float radius, float startAngle, float endAngle, Color? color = null, float depth = 0, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount, uint sortKey = 0)
        {
            var color1 = color ?? Color.White;

            var firstVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);
            var secondVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);

            _shapeBuilder.Clear();
            _shapeBuilder.AppendArc(position, radius, startAngle, endAngle, circleSegmentsCount);

            var firstPoint = _shapeBuilder.Buffer[0];
            firstVertex.Position.X = firstPoint.X;
            firstVertex.Position.Y = firstPoint.Y;

            for (var i = 1; i < _shapeBuilder.Count; i++)
            {
                var point = _shapeBuilder.Buffer[i];

                secondVertex.Position.X = point.X;
                secondVertex.Position.Y = point.Y;

                _primitiveBatch.DrawLine(ref firstVertex, ref secondVertex, ref _emptyBatchItemData, sortKey);

                firstVertex.Position.X = secondVertex.Position.X;
                firstVertex.Position.Y = secondVertex.Position.Y;
            }
        }

        public void DrawArc(Vector2 position, float radius, float startAngle, float endAngle, Color? color = null, float depth = 0, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount, uint sortKey = 0)
        {
            var color1 = color ?? Color.White;

            var firstVertex = new VertexPositionColorTexture(new Vector3(position.X, position.Y, depth), color1, Vector2.Zero);
            var secondVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);
            var thirdVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);

            _shapeBuilder.Clear();
            _shapeBuilder.AppendArc(position, radius, startAngle, endAngle, circleSegmentsCount);

            var firstPoint = _shapeBuilder.Buffer[0];

            secondVertex.Position.X = firstPoint.X;
            secondVertex.Position.Y = firstPoint.Y;

            for (var i = 1; i < _shapeBuilder.Count; i++)
            {
                var point = _shapeBuilder.Buffer[i];

                thirdVertex.Position.X = point.X;
                thirdVertex.Position.Y = point.Y;

                _primitiveBatch.DrawTriangle(ref firstVertex, ref secondVertex, ref thirdVertex, ref _emptyBatchItemData, sortKey);

                secondVertex.Position.X = thirdVertex.Position.X;
                secondVertex.Position.Y = thirdVertex.Position.Y;
            }
        }

        public void DrawCircleOutline(Vector2 position, float radius, Vector2? axis = null, Color? color = null, float depth = 0, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount, uint sortKey = 0)
        {
            var color1 = color ?? Color.White;

            var firstVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);
            var secondVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);

            _shapeBuilder.Clear();
            _shapeBuilder.AppendCircle(position, radius, circleSegmentsCount);

            var firstPoint = _shapeBuilder.Buffer[0];
            firstVertex.Position.X = firstPoint.X;
            firstVertex.Position.Y = firstPoint.Y;

            for (var i = 1; i < _shapeBuilder.Count; i++)
            {
                var point = _shapeBuilder.Buffer[i];

                secondVertex.Position.X = point.X;
                secondVertex.Position.Y = point.Y;

                _primitiveBatch.DrawLine(ref firstVertex, ref secondVertex, ref _emptyBatchItemData, sortKey);

                firstVertex.Position.X = secondVertex.Position.X;
                firstVertex.Position.Y = secondVertex.Position.Y;
            }

            secondVertex.Position.X = firstPoint.X;
            secondVertex.Position.Y = firstPoint.Y;

            _primitiveBatch.DrawLine(ref firstVertex, ref secondVertex, ref _emptyBatchItemData, sortKey);

            if (!axis.HasValue)
            {
                return;
            }

            firstVertex.Position.X = position.X;
            firstVertex.Position.Y = position.Y;
            secondVertex.Position.X = position.X + axis.Value.X * radius;
            secondVertex.Position.Y = position.Y + axis.Value.Y * radius;

            _primitiveBatch.DrawLine(ref firstVertex, ref secondVertex, ref _emptyBatchItemData, sortKey);
        }

        public void DrawCircle(Vector2 position, float radius, Color? color = null, float depth = 0, int circleSegmentsCount = ShapeBuilder.DefaultCircleSegmentsCount, uint sortKey = 0)
        {
            var color1 = color ?? Color.White;

            var firstVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);
            var secondVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);
            var thirdVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);

            _shapeBuilder.Clear();
            _shapeBuilder.AppendCircle(position, radius, circleSegmentsCount);

            var firstPoint = _shapeBuilder.Buffer[0];
            var secondPoint = _shapeBuilder.Buffer[1];

            firstVertex.Position.X = firstPoint.X;
            firstVertex.Position.Y = firstPoint.Y;
            secondVertex.Position.X = secondPoint.X;
            secondVertex.Position.Y = secondPoint.Y;

            for (var i = 2; i < _shapeBuilder.Count; i++)
            {
                var point = _shapeBuilder.Buffer[i];

                thirdVertex.Position.X = point.X;
                thirdVertex.Position.Y = point.Y;

                _primitiveBatch.DrawTriangle(ref firstVertex, ref secondVertex, ref thirdVertex, ref _emptyBatchItemData, sortKey);

                secondVertex.Position.X = thirdVertex.Position.X;
                secondVertex.Position.Y = thirdVertex.Position.Y;
            }
        }

        internal struct BatchItemData : IBatchItemData<BatchItemData, Effect>
        {
            internal Texture2D Texture;

            public bool Equals(ref BatchItemData other)
            {
                return true;
            }

            public void ApplyTo(Effect effect)
            {
            }
        }
    }
}
