using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collision.Detection.Broadphase;

namespace MonoGame.Extended.Collision.Detection.Narrowphase
{
    public class SeperatingAxisNarrowphase2D : INarrowphase2D
    {
        public CollisionSimulation2D CollisionSimulation { get; }

        public SeperatingAxisNarrowphase2D(CollisionSimulation2D collisionSimulation)
        {
            if (collisionSimulation == null)
            {
                throw new ArgumentNullException(nameof(collisionSimulation));
            }

            CollisionSimulation = collisionSimulation;
        }

        public void Query(ref BroadphaseCollisionResult2D broadphaseResult, GameTime gameTime, out NarrowphaseCollisionResult2D? narrowphaseResult)
        {
            var colliderA = broadphaseResult.FirstCollider;
            var colliderB = broadphaseResult.SecondCollider;

            float minimumPenetrationA;
            float minimumPenetrationB;
            Vector2 minimumPenetrationAxisA;
            Vector2 minimumPenetrationAxisB;

            if (!FoundMinimumPenerationAxis(colliderA, colliderB, out minimumPenetrationA, out minimumPenetrationAxisA))
            {
                narrowphaseResult = null;
                return;
            }

            if (!FoundMinimumPenerationAxis(colliderB, colliderA, out minimumPenetrationB, out minimumPenetrationAxisB))
            {
                narrowphaseResult = null;
                return;
            }

            Vector2 minimumPenetrationAxis;
            float minimumPenetration;

            if (minimumPenetrationA > minimumPenetrationB)
            {
                minimumPenetrationAxis = minimumPenetrationAxisA;
                minimumPenetration = minimumPenetrationA;
            }
            else
            {
                minimumPenetrationAxis = -minimumPenetrationAxisB;
                minimumPenetration = minimumPenetrationB;
            }

            narrowphaseResult = new NarrowphaseCollisionResult2D(colliderA, colliderB, minimumPenetrationAxis, minimumPenetration);
        }

        private static bool FoundMinimumPenerationAxis(Collider2D colliderA, Collider2D colliderB, out float minimumPenetration, out Vector2 minimumPenetrationAxis)
        {
            var shapeA = colliderA.Shape;
            var shapeB = colliderB.Shape;
            var normals = shapeA.WorldNormals;
            var vertices = shapeA.WorldVertices;
            var normalsCount = normals.Count;

            minimumPenetration = float.MinValue;
            minimumPenetrationAxis = default(Vector2);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < normalsCount; i++)
            {
                var normal = normals[i];
                var supportPoint = shapeB.GetSupportPoint(-normal);
                var vertex = vertices[i];

                var distance = normal.Dot(supportPoint - vertex);
                if (distance > 0)
                    return false;
                if (distance <= minimumPenetration)
                    continue;

                minimumPenetration = distance;
                minimumPenetrationAxis = normal;
            }

            return true;
        }
    }
}
