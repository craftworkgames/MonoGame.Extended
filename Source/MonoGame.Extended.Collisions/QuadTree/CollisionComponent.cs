using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Handles basic collision between actors.
    /// When two actors collide, their OnCollision method is called.
    /// </summary>
    public class CollisionComponent : SimpleGameComponent
    {
        private readonly Dictionary<ICollisionActor, QuadtreeData> _targetDataDictionary =
            new Dictionary<ICollisionActor, QuadtreeData>();

        private readonly Quadtree _collisionTree;

        public CollisionComponent(RectangleF boundary)
        {
            _collisionTree = new Quadtree(boundary);
        }

        public override void Update(GameTime gameTime)
        {
            // Update bounding box locations.
            foreach (var value in _targetDataDictionary.Values)
            {
                _collisionTree.Remove(value);
                value.Bounds = value.Target.Bounds;
                _collisionTree.Insert(value);
            }

            // Detect collisions
            foreach (var value in _targetDataDictionary.Values)
            {
                var target = value.Target;
                var collisions =_collisionTree.Query(target.Bounds)
                    .Where(data => data.Target != target);

                // Generate list of collision Infos
                foreach (var other in collisions)
                {
                    var collisionInfo = new CollisionEventArgs()
                    {
                        Other = other.Target,
                        PenetrationVector = CalculatePenetrationVector(value.Bounds, other.Bounds)
                    };

                    target.OnCollision(collisionInfo);
                }
            }
        }

        public void Insert(ICollisionActor target)
        {
            if (!_targetDataDictionary.ContainsKey(target))
            {
                var data = new QuadtreeData(target);
                _targetDataDictionary.Add(target, data);
                _collisionTree.Insert(data);
            }
        }

        public void Remove(ICollisionActor target)
        {
            if (_targetDataDictionary.ContainsKey(target))
            {
                var data = _targetDataDictionary[target];
                _collisionTree.Remove(data);
                _targetDataDictionary.Remove(target);
            }
        }

        #region Penetration Vectors

        private static Vector2 CalculatePenetrationVector(IShapeF a, IShapeF b)
        {
            switch (a)
            {
                case RectangleF rectA when b is RectangleF rectB:
                    return PenetrationVector(rectA, rectB);
                case CircleF circA when b is CircleF circB:
                    return PenetrationVector(circA, circB);
                case CircleF circA when b is RectangleF rectB:
                    return PenetrationVector(circA, rectB);
                case RectangleF rectA when b is CircleF circB:
                    return PenetrationVector(rectA, circB);
            }

            throw new NotSupportedException("Shapes must be either a CircleF or RectangleF");
        }

        private static Vector2 PenetrationVector(RectangleF rect1, RectangleF rect2)
        {
            var intersectingRectangle = RectangleF.Intersection(rect1, rect2);
            Debug.Assert(!intersectingRectangle.IsEmpty,
                "Violation of: !intersect.IsEmpty; Rectangles must intersect to calculate a penetration vector.");

            Vector2 penetration;
            if (intersectingRectangle.Width < intersectingRectangle.Height)
            {
                var d = rect1.Center.X < rect2.Center.X
                    ? intersectingRectangle.Width
                    : -intersectingRectangle.Width;
                penetration = new Vector2(d, 0);
            }
            else
            {
                var d = rect1.Center.Y < rect2.Center.Y
                    ? intersectingRectangle.Height
                    : -intersectingRectangle.Height;
                penetration = new Vector2(0, d);
            }

            return penetration;
        }

        private static Vector2 PenetrationVector(CircleF circ1, CircleF circ2)
        {
            var rWant = circ1.Radius + circ2.Radius;
            var disp = Point2.Displacement(circ1.Center, circ2.Center);
            var rCurr = Math.Sqrt(disp.X * disp.X + disp.Y * disp.Y);

            var penVector = rWant/(float)rCurr * disp;

            return penVector;
        }

        private static Vector2 PenetrationVector(CircleF circ, RectangleF rect)
        {
            var closestPoint = rect.ClosestPointTo(circ.Center);
            var closestVector = Point2.Displacement(circ.Center, closestPoint);
            var distance = closestVector.Length();

            return (circ.Radius / distance) * closestVector;
        }

        private static Vector2 PenetrationVector(RectangleF rect, CircleF circ)
        {
            return -PenetrationVector(circ, rect);
        }

        #endregion
    }
}