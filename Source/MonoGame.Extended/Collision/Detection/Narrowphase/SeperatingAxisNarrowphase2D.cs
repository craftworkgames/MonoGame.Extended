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

            if (!FindMinimumPenerationAxis(colliderA, colliderB, out minimumPenetrationA, out minimumPenetrationAxisA))
            {
                narrowphaseResult = null;
                return;
            }

            if (!FindMinimumPenerationAxis(colliderB, colliderA, out minimumPenetrationB, out minimumPenetrationAxisB))
            {
                narrowphaseResult = null;
                return;
            }

            Vector2 minimumPenetrationAxis;
            float minimumPenetration;

            // http://www.randygaul.net/2013/03/28/custom-physics-engine-part-2-manifold-generation/
            // when two axes have approx. the same minimum penetration, the simulation can flop between the two causing jitter
            // solution: favor one axis over another by comparing if the first peneration is greater than the second by a margin of error

            // 0.9800f in Farseer @ SourceFiles/Collision/Collision.cs:Farseer.Collision.CollidePolygons()
            // 1.0000f in Box2D   @ Box2D/Box2D/Box2D/Collision/b2CollidePolygon.cpp:b2CollidePolygons()
            const float biasRelative = 0.95f;
            // 0.0010f in Farseer @ SourceFiles/Collision/Collision.cs:Farseer.Collision.CollidePolygons()
            // 0.0005f in Box2D   @ Box2D/Box2D/Box2D/Collision/b2CollidePolygon.cpp:b2CollidePolygons()
            const float biasAbsolute = 0.01f;

            // use >= instead of > for NaN comparison safety
            if (minimumPenetrationA >= minimumPenetrationB * biasRelative + minimumPenetrationA * biasAbsolute)
            {
                minimumPenetrationAxis = minimumPenetrationAxisA;
                minimumPenetration = minimumPenetrationA;
            }
            else
            {
                minimumPenetrationAxis = -minimumPenetrationAxisB;
                minimumPenetration = minimumPenetrationB;
            }

            //TODO: Compute contact points

            narrowphaseResult = new NarrowphaseCollisionResult2D(colliderA, colliderB, minimumPenetrationAxis, minimumPenetration);
        }

        private static bool FindMinimumPenerationAxis(Collider2D colliderA, Collider2D colliderB, out float minimumPenetration, out Vector2 minimumPenetrationAxis)
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
