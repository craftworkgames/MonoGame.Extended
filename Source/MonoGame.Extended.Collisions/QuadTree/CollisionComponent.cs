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

        /// <summary>
        /// Creates a collision tree covering the specified area.
        /// </summary>
        /// <param name="boundary">Boundary of the collision tree.</param>
        public CollisionComponent(RectangleF boundary)
        {
            _collisionTree = new Quadtree(boundary);
        }

        /// <summary>
        /// Update the collision tree and process collisions.
        /// </summary>
        /// <remarks>
        /// Boundary shapes are updated if they were changed since the last
        /// update.
        /// </remarks>
        /// <param name="gameTime"></param>
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

        /// <summary>
        /// Inserts the target into the collision tree.
        /// The target will have its OnCollision called when collisions occur.
        /// </summary>
        /// <param name="target">Target to insert.</param>
        public void Insert(ICollisionActor target)
        {
            if (!_targetDataDictionary.ContainsKey(target))
            {
                var data = new QuadtreeData(target);
                _targetDataDictionary.Add(target, data);
                _collisionTree.Insert(data);
            }
        }

        /// <summary>
        /// Removes the target from the collision tree.
        /// </summary>
        /// <param name="target">Target to remove.</param>
        public void Remove(ICollisionActor target)
        {
            if (_targetDataDictionary.ContainsKey(target))
            {
                var data = _targetDataDictionary[target];
                _collisionTree.Remove(data);
                _targetDataDictionary.Remove(target);
            }
        }

        /// <summary>
        /// Gets if the target is inserted in the collision tree.
        /// </summary>
        /// <param name="target">Actor to check if contained</param>
        /// <returns>True if the target is contained in the collision tree.</returns>
        public bool Contains(ICollisionActor target)
        {
            return _targetDataDictionary.ContainsKey(target);
        }

        #region Penetration Vectors

        /// <summary>
        /// Calculate a's penetration into b
        /// </summary>
        /// <param name="a">The penetrating shape.</param>
        /// <param name="b">The shape being penetrated.</param>
        /// <returns>The distance vector from the edge of b to a's Position</returns>
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
            Debug.Assert(circ1.Intersects(circ2));

            var displacement = Point2.Displacement(circ1.Center, circ2.Center);

            Vector2 desiredDisplacement;
            if (displacement != Vector2.Zero)
            {
                desiredDisplacement = displacement.NormalizedCopy() * (circ1.Radius + circ2.Radius);
            }
            else
            {
                desiredDisplacement = -Vector2.UnitY * (circ1.Radius + circ2.Radius);
            }


            var penetration = displacement - desiredDisplacement;
            return penetration;
        }

        private static Vector2 PenetrationVector(CircleF circ, RectangleF rect)
        {
            var displacement = Point2.Displacement(circ.Center, rect.Center);

            Vector2 desiredDisplacement;
            if (displacement != Vector2.Zero)
            {
                // Calculate penetration as only in X or Y direction.
                // Whichever is lower.
                var dispx = new Vector2(displacement.X, 0);
                var dispy = new Vector2(0, displacement.Y);
                dispx.Normalize();
                dispy.Normalize();

                dispx *= (circ.Radius + rect.Width / 2);
                dispy *= (circ.Radius + rect.Height / 2);

                if (dispx.LengthSquared() < dispy.LengthSquared())
                {
                    desiredDisplacement = dispx;
                    displacement.Y = 0;
                }
                else
                {
                    desiredDisplacement = dispy;
                    displacement.X = 0;
                }
            }
            else
            {
                desiredDisplacement = -Vector2.UnitY * (circ.Radius + rect.Height / 2);
            }

            var penetration = displacement - desiredDisplacement;
            return penetration;
        }

        private static Vector2 PenetrationVector(RectangleF rect, CircleF circ)
        {
            return -PenetrationVector(circ, rect);
        }

        #endregion
    }
}