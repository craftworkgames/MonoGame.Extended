using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Platformer.Collisions
{
    public struct CollisionPair
    {
        public Body BodyA;
        public Body BodyB;
    }

    public class World
    {
        public World(Vector2 gravity)
        {
            Gravity = gravity;
            Bodies = new List<Body>();
        }

        private readonly List<CollisionPair> _collisionPairs = new List<CollisionPair>();

        public Vector2 Gravity { get; set; }
        public List<Body> Bodies { get; }
        public Action<Manifold> OnCollision { get; set; }

        public void Update(float deltaTime)
        {
            foreach (var body in Bodies.Where(b => b.BodyType == BodyType.Dynamic))
            {
                body.Position += body.Velocity * deltaTime;
                body.Velocity += Gravity * deltaTime;
            }

            foreach (var bodyA in Bodies)
            {
                foreach (var bodyB in Bodies.Where(b => b.BodyType == BodyType.Dynamic))
                {
                    if (bodyA != bodyB && CollisionTester.AabbAabb(bodyA.BoundingBox, bodyB.BoundingBox))
                        _collisionPairs.Add(new CollisionPair {BodyA = bodyA, BodyB = bodyB});
                }
            }

            foreach (var collisionPair in _collisionPairs)
            {
                if (CollisionTester.AabbAabb(collisionPair.BodyA, collisionPair.BodyB, out var manifold))
                    OnCollision?.Invoke(manifold);
            }

            _collisionPairs.Clear();
        }
    }
}