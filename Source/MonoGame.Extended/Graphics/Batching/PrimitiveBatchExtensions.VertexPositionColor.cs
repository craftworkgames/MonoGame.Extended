using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public static partial class PrimitiveBatchExtensions
    {
        public static void DrawLine(this PrimitiveBatch<VertexPositionColor> primitiveBatch, Effect effect, Vector2 firstPoint, Vector2 secondPoint, Color? color = null, float depth = 0f, uint sortKey = 0)
        {
            var color1 = color ?? Color.White;
            var firstVertex = new VertexPositionColor(new Vector3(0, 0, depth), color1);
            var secondVertex = new VertexPositionColor(new Vector3(0, 0, depth), color1);
            primitiveBatch.DrawLine(effect, ref firstVertex, ref secondVertex, sortKey);
        }

        public static void DrawPolygonOutline(this PrimitiveBatch<VertexPositionColor> primitiveBatch, Effect effect, IList<Vector2> points, Color? color = null, float depth = 0f, uint sortKey = 0)
        {
            if (points.Count == 0)
            {
                return;
            }

            var color1 = color ?? Color.White;

            Vector2 firstPoint;
            Vector2 secondPoint;
            var firstVertex = new VertexPositionColor(new Vector3(0, 0, depth), color1);
            var secondVertex = new VertexPositionColor(new Vector3(0, 0, depth), color1);

            for (var i = 0; i < points.Count - 1; i++)
            {
                firstPoint = points[i];
                secondPoint = points[i + 1];

                firstVertex.Position.X = firstPoint.X;
                firstVertex.Position.Y = firstPoint.Y;
                secondVertex.Position.X = secondPoint.X;
                secondVertex.Position.Y = secondPoint.Y;

                primitiveBatch.DrawLine(effect, ref firstVertex, ref secondVertex, sortKey);
            }

            firstPoint = points[points.Count - 1];
            secondPoint = points[0];

            firstVertex.Position.X = firstPoint.X;
            firstVertex.Position.Y = firstPoint.Y;
            secondVertex.Position.X = secondPoint.X;
            secondVertex.Position.Y = secondPoint.Y;

            primitiveBatch.DrawLine(effect, ref firstVertex, ref secondVertex, sortKey);
        }

        public static void DrawPolygon(this PrimitiveBatch<VertexPositionColor> primitiveBatch, Effect effect, IList<Vector2> points, Color? color = null, float depth = 0f, uint sortKey = 0)
        {
            if (points.Count == 0)
            {
                return;
            }

            var color1 = color ?? Color.White;

            var firstVertex = new VertexPositionColor(new Vector3(points[0], depth), color1);
            var secondVertex = new VertexPositionColor(new Vector3(0, 0, depth), color1);
            var thirdVertex = new VertexPositionColor(new Vector3(0, 0, depth), color1);

            for (var i = 1; i < points.Count - 1; i++)
            {
                var secondPoint = points[i];
                var thirdPoint = points[i + 1];

                secondVertex.Position.X = secondPoint.X;
                secondVertex.Position.Y = secondPoint.Y;
                thirdVertex.Position.X = thirdPoint.X;
                thirdVertex.Position.Y = thirdPoint.Y;

                primitiveBatch.DrawTriangle(effect, ref firstVertex, ref secondVertex, ref thirdVertex, sortKey);
            }
        }

        public static void DrawCircleOutline(this PrimitiveBatch<VertexPositionColor> primitiveBatch, Effect effect, Vector2 centerPoint, float radius, Vector2? axis = null, Color? color = null, float depth = 0, float circleSegments = 32, uint sortKey = 0)
        {
            var theta = 0.0;
            var thetaStep = Math.PI * 2.0 / circleSegments;
            var color1 = color ?? Color.White;

            var radiusCos = radius * (float)Math.Cos(theta);
            var radiusSin = radius * (float)Math.Sin(theta);
            var firstVertex = new VertexPositionColor(new Vector3(centerPoint.X + radiusCos, centerPoint.Y + radiusSin, depth), color1);
            var secondVertex = new VertexPositionColor(new Vector3(0, 0, depth), color1);

            for (var i = 0; i < circleSegments; i++)
            {
                theta += thetaStep;
                radiusCos = radius * (float)Math.Cos(theta);
                radiusSin = radius * (float)Math.Sin(theta);

                secondVertex.Position.X = centerPoint.X + radiusCos;
                secondVertex.Position.Y = centerPoint.Y + radiusSin;

                primitiveBatch.DrawLine(effect, ref firstVertex, ref secondVertex, sortKey);

                firstVertex.Position.X = secondVertex.Position.X;
                firstVertex.Position.Y = secondVertex.Position.Y;
            }

            if (axis == null)
            {
                return;
            }

            firstVertex.Position.X = centerPoint.X;
            firstVertex.Position.Y = centerPoint.Y;
            secondVertex.Position.X = centerPoint.X + axis.Value.X * radius;
            secondVertex.Position.Y = centerPoint.Y + axis.Value.Y * radius;

            primitiveBatch.DrawLine(effect, ref firstVertex, ref secondVertex, sortKey);
        }

        public static void DrawCircle(this PrimitiveBatch<VertexPositionColor> primitiveBatch, Effect effect, Vector2 centerPoint, float radius, Color? color = null, float depth = 0, float circleSegments = 32, uint sortKey = 0)
        {
            var theta = 0.0;
            var thetaStep = Math.PI * 2.0 / circleSegments;
            var color1 = color ?? Color.White;

            var radiusCos = radius * (float)Math.Cos(theta);
            var radiusSin = radius * (float)Math.Sin(theta);
            var firstVertex = new VertexPositionColor(new Vector3(centerPoint.X + radiusCos, centerPoint.Y + radiusSin, depth), color1);
            var secondVertex = new VertexPositionColor(new Vector3(centerPoint.X + radiusCos, centerPoint.Y + radiusSin, depth), color1);
            var thirdVertex = new VertexPositionColor(new Vector3(0, 0, depth), color1);

            for (var i = 0; i < circleSegments; i++)
            {
                theta += thetaStep;
                radiusCos = radius * (float)Math.Cos(theta);
                radiusSin = radius * (float)Math.Sin(theta);

                thirdVertex.Position.X = centerPoint.X + radiusCos;
                thirdVertex.Position.Y = centerPoint.Y + radiusSin;

                primitiveBatch.DrawTriangle(effect, ref firstVertex, ref secondVertex, ref thirdVertex, sortKey);

                secondVertex.Position.X = thirdVertex.Position.X;
                secondVertex.Position.Y = thirdVertex.Position.Y;
            }
        }
    }
}
