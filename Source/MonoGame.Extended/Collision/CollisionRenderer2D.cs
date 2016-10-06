using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collision.BoundingVolumes;
using MonoGame.Extended.Collision.Detection.Narrowphase;
using MonoGame.Extended.Collision.Shapes;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Collision
{
    public class CollisionRenderer2D
    {
        private readonly DynamicBatch2D _batch;

        public CollisionSimulation2D CollisionSimulation { get; }
        public Color BoundingVolumeFillColor = Color.AliceBlue * 0.5f;
        public Color BoundingVolumeStrokeColor = Color.AliceBlue;
        public Color ShapeFillColor = Color.Cornsilk * 0.5f;
        public Color ShapeStrokeColor = Color.Cornsilk;

        public CollisionRenderer2D(CollisionSimulation2D collisionSimulation, GraphicsDevice graphicsDevice)
        {

            if (collisionSimulation == null)
            {
                throw new ArgumentNullException(nameof(collisionSimulation));
            }

            CollisionSimulation = collisionSimulation;

            _batch = new DynamicBatch2D(graphicsDevice);
        }

        public void Begin()
        {
            _batch.Begin(rasterizerState: RasterizerState.CullNone);
        }

        public void End()
        {
            _batch.End();
        }

        public void DrawBoundingVolume(BoundingVolume2D boundingVolume)
        {
            switch (boundingVolume.Type)
            {
                case BoundingVolumeType2D.BoundingBox:
                    DrawBoundingBox((BoundingBox2D)boundingVolume);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawBoundingBox(BoundingBox2D boundingBox)
        {
            var size = boundingBox.Radius * 2;
            _batch.DrawRectangle(boundingBox.Centre, size, BoundingVolumeFillColor);
        }

        public void DrawShape(CollisionShape2D shape)
        {
            var transformedVertices = shape.WorldVertices;

            _batch.DrawConvexPolygon(transformedVertices, ShapeFillColor);
            
            var verticesCount = transformedVertices.Count;
            var previousVertex = transformedVertices[verticesCount - 1];

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < verticesCount; i++)
            {
                var vertex = transformedVertices[i];;
                var edge = previousVertex - vertex;
                var normal = -edge.PerpendicularCounterClockwise(); // negate because working in coordinate system with a flipped y-axis
                normal.Normalize();
                var startPoint = vertex + edge * 0.5f;
                var endPoint = startPoint + normal * 5;

                _batch.DrawAliasedLine(startPoint, endPoint, 2, Color.Cyan);

                previousVertex = vertex;
            }
        }

        public void DrawCollision(NarrowphaseCollisionResult2D result)
        {
            var minimumTranslationVector = result.MinimumPenetration * result.MinimumPenetrationAxis;

            var colliderA = result.FirstCollider;
            var centroidA = colliderA.Shape.WorldCentroid;

            _batch.DrawAliasedLine(centroidA, centroidA + minimumTranslationVector, 2, Color.DarkCyan);

            var colliderB = result.SecondCollider;
            var centroidB = colliderB.Shape.WorldCentroid;

            _batch.DrawAliasedLine(centroidB, centroidB - minimumTranslationVector, 2, Color.DarkCyan);

            if (result.ContactPointsCount >= 1)
            {
                _batch.DrawRectangle(result.FirstContactPoint, new SizeF(5, 5), Color.Orange);
            }
            if (result.ContactPointsCount == 2)
            {
                _batch.DrawRectangle(result.SecondContactPoint, new SizeF(5, 5), Color.Orange);
            }
        }
    }
}
