using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Shapes.Explicit;

namespace MonoGame.Extended.Graphics
{
    public class ShapeBatch
    {
        private readonly PrimitiveBatch<VertexPositionColorTexture, BatchItemData, BatchEffect> _primitiveBatch;
        private BatchItemData _emptyBatchItemData;
        private Matrix _defaultWorld;
        private Matrix _defaultView;
        private Matrix _defaultProjection;

        private readonly ShapeBuilder _shapeBuilder;

        public BatchEffect Effect { get; }

        public ShapeBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }

            _primitiveBatch = new PrimitiveBatch<VertexPositionColorTexture, BatchItemData, BatchEffect>(graphicsDevice);
            Effect = new BatchEffect(graphicsDevice);

            _shapeBuilder = new ShapeBuilder();

            var viewport = graphicsDevice.Viewport;

            _defaultWorld = Matrix.Identity;
            _defaultView = Matrix.Identity;
            Matrix.CreateOrthographicOffCenter(left: 0, right: viewport.Width, bottom: viewport.Height, top: 0, zNearPlane: 0, zFarPlane: 1, result: out _defaultProjection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNumberOfCircleSegments(float radius)
        {
            //TODO multiply by world scale
            return (int)Math.Ceiling(10 * Math.Sqrt(radius));
        }

        public void Begin(BatchMode batchMode)
        {
            Effect.SetWorld(ref _defaultWorld);
            Effect.SetView(ref _defaultView);
            Effect.SetProjection(ref _defaultProjection);
            _primitiveBatch.Begin(Effect, batchMode);
        }

        public void Begin(BatchMode batchMode, ref Matrix view)
        {
            Effect.SetWorld(ref _defaultWorld);
            Effect.SetView(ref view);
            Effect.SetProjection(ref _defaultProjection);
            _primitiveBatch.Begin(Effect, batchMode);
        }

        public void Begin(BatchMode batchMode, ref Matrix world, ref Matrix view)
        {
            Effect.SetWorld(ref world);
            Effect.SetView(ref view);
            Effect.SetProjection(ref _defaultProjection);
            _primitiveBatch.Begin(Effect, batchMode);
        }

        public void Begin(BatchMode batchMode, ref Matrix world, ref Matrix view, ref Matrix projection)
        {
            Effect.SetWorld(ref world);
            Effect.SetView(ref view);
            Effect.SetProjection(ref projection);
            _primitiveBatch.Begin(Effect, batchMode);
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
            var firstVertex = new VertexPositionColor(new Vector3(x: 0, y: 0, z: depth), color1);
            var secondVertex = new VertexPositionColor(new Vector3(x: 0, y: 0, z: depth), color1);

            for (var i = 0; i < points.Count - 1; i++)
            {
                firstPoint = points[i];
                secondPoint = points[i + 1];

                firstVertex.Position.X = firstPoint.X;
                firstVertex.Position.Y = firstPoint.Y;
                secondVertex.Position.X = secondPoint.X;
                secondVertex.Position.Y = secondPoint.Y;

                //_primitiveBatch.DrawLine(ref firstVertex, ref secondVertex, ref _emptyBatchItemData, sortKey);
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

        public void DrawArcOutline(Vector2 centerPoint, float radius, float startAngle, float endAngle, Color? color = null, float depth = 0, int? circleSegments = null, uint sortKey = 0)
        {
            //http://slabode.exofire.net/circle_draw.shtml

            var segments1 = circleSegments ?? GetNumberOfCircleSegments(radius);
            var theta = endAngle / (segments1 - 1);
            var cos = (float)Math.Cos(theta);
            var sin = (float)Math.Sin(theta);
            var color1 = color ?? Color.White;
            var firstVertex = new VertexPositionColor(new Vector3(centerPoint.X + radius * (float)Math.Cos(startAngle), centerPoint.Y + radius * (float)Math.Sin(startAngle), depth), color1);
            var secondVertex = new VertexPositionColor(new Vector3(0, 0, depth), color1);

            for (var i = 0; i < segments1; i++)
            {
                secondVertex.Position.X = cos * firstVertex.Position.X - sin * firstVertex.Position.Y;
                secondVertex.Position.Y = sin * firstVertex.Position.X + cos * firstVertex.Position.Y;

                //_primitiveBatch.DrawLine(ref firstVertex, ref secondVertex, ref _emptyBatchItemData, sortKey);

                firstVertex.Position.X = secondVertex.Position.X;
                firstVertex.Position.Y = secondVertex.Position.Y;
            }
        }

        public void DrawCircleOutline(Vector2 centerPoint, float radius, Vector2? axis = null, Color? color = null, float depth = 0, int? circleSegments = null, uint sortKey = 0)
        {
            var segments1 = circleSegments ?? GetNumberOfCircleSegments(radius);
            var color1 = color ?? Color.White;

            var firstLineVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);
            var secondLineVertex = new VertexPositionColorTexture(new Vector3(0, 0, depth), color1, Vector2.Zero);

            _shapeBuilder.CreateCircle(centerPoint, radius, segments1);

            var firstPoint = _shapeBuilder.Buffer[0];
            firstLineVertex.Position.X = firstPoint.X;
            firstLineVertex.Position.Y = firstPoint.Y;

            for (var i = 1; i < _shapeBuilder.Count; i++)
            {
                var point = _shapeBuilder.Buffer[i];

                secondLineVertex.Position.X = point.X;
                secondLineVertex.Position.Y = point.Y;

                _primitiveBatch.DrawLine(ref firstLineVertex, ref secondLineVertex, ref _emptyBatchItemData, sortKey);

                firstLineVertex.Position.X = secondLineVertex.Position.X;
                firstLineVertex.Position.Y = secondLineVertex.Position.Y;
            }

            firstLineVertex.Position.X = firstPoint.X;
            firstLineVertex.Position.Y = firstPoint.Y;

            _primitiveBatch.DrawLine(ref firstLineVertex, ref secondLineVertex, ref _emptyBatchItemData, sortKey);
        }

        //        public static void DrawCircle(this PrimitiveBatch<VertexPositionColor> primitiveBatch, Effect effect, Vector2 centerPoint, float radius, Color? color = null, float depth = 0, float? segments = null, uint sortKey = 0)
        //        {
        //            var theta = 0.0;
        //            var thetaStep = Math.PI * 2.0 / segments;
        //            var color1 = color ?? Color.White;
        //
        //            var radiusCos = radius * (float)Math.Cos(theta);
        //            var radiusSin = radius * (float)Math.Sin(theta);
        //            var firstVertex = new VertexPositionColor(new Vector3(centerPoint.X + radiusCos, centerPoint.Y + radiusSin, depth), color1);
        //            var secondVertex = new VertexPositionColor(new Vector3(centerPoint.X + radiusCos, centerPoint.Y + radiusSin, depth), color1);
        //            var thirdVertex = new VertexPositionColor(new Vector3(0, 0, depth), color1);
        //
        //            for (var i = 0; i < segments; i++)
        //            {
        //                theta += thetaStep;
        //                radiusCos = radius * (float)Math.Cos(theta);
        //                radiusSin = radius * (float)Math.Sin(theta);
        //
        //                thirdVertex.Position.X = centerPoint.X + radiusCos;
        //                thirdVertex.Position.Y = centerPoint.Y + radiusSin;
        //
        //                primitiveBatch.DrawTriangle(effect, ref firstVertex, ref secondVertex, ref thirdVertex, sortKey);
        //
        //                secondVertex.Position.X = thirdVertex.Position.X;
        //                secondVertex.Position.Y = thirdVertex.Position.Y;
        //            }
        //        }

        internal struct BatchItemData : IBatchItemData<BatchItemData, BatchEffect>
        {
            internal Texture2D Texture;

            public bool Equals(ref BatchItemData other)
            {
                return true;
            }

            public void ApplyTo(BatchEffect effect)
            {
            }
        }
    }
}
