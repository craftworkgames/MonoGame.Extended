using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Collisions.QuadTree;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Handles basic collision between actors.
    /// When two actors collide, their OnCollision method is called.
    /// </summary>
    public class CollisionComponent : SimpleGameComponent
    {
        public const string DEFAULT_LAYER_NAME = "default";

        private Dictionary<string, Layer> _layers = new();

        /// <summary>
        /// List of collision's layers
        /// </summary>
        public IReadOnlyDictionary<string, Layer> Layers => _layers;

        private HashSet<(Layer, Layer)> _layerCollision = new();

        /// <summary>
        /// Creates component with default layer, which is a collision tree covering the specified area (using <see cref="QuadTree"/>.
        /// </summary>
        /// <param name="boundary">Boundary of the collision tree.</param>
        public CollisionComponent(RectangleF boundary)
        {
            SetDefaultLayer(new Layer(new QuadTreeSpace(boundary)));
        }

        /// <summary>
        /// Creates component with specifies default layer.
        /// If layer is null, method creates component without default layer.
        /// </summary>
        /// <param name="layer">Default layer</param>
        public CollisionComponent(Layer layer = null)
        {
            if (layer is not null)
                SetDefaultLayer(layer);
        }

        /// <summary>
        /// The main layer has the name from <see cref="DEFAULT_LAYER_NAME"/>.
        /// The main layer collision with itself and all other layers.
        /// </summary>
        /// <param name="layer">Layer to set default</param>
        public void SetDefaultLayer(Layer layer)
        {
            if (_layers.ContainsKey(DEFAULT_LAYER_NAME))
                Remove(DEFAULT_LAYER_NAME);
            Add(DEFAULT_LAYER_NAME, layer);
            foreach (var otherLayer in _layers.Values)
                AddCollisionBetweenLayer(layer, otherLayer);
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
            foreach (var layer in _layers.Values)
                layer.Reset();

            foreach (var (firstLayer, secondLayer) in _layerCollision)
            foreach (var actor in firstLayer.Space)
            {
                var collisions = secondLayer.Space.Query(actor.Bounds.BoundingRectangle);
                foreach (var other in collisions)
                    if (actor != other && actor.Bounds.Intersects(other.Bounds))
                    {
                        var penetrationVector = CalculatePenetrationVector(actor.Bounds, other.Bounds);

                        actor.OnCollision(new CollisionEventArgs
                        {
                            Other = other,
                            PenetrationVector = penetrationVector
                        });
                        other.OnCollision(new CollisionEventArgs
                        {
                            Other = actor,
                            PenetrationVector = -penetrationVector
                        });
                    }

            }
        }

        /// <summary>
        /// Cast an actor's colliders in a direction and check for collisions.
        /// </summary>
        /// <param name="actors">The list of actor colliders to cast.</param>
        /// <param name="hitBuffer">Buffer to receive results.</param>
        /// <param name="move">Normalized vector representing the direction to cast each shape.</param>
        /// <param name="distance">Maximum distance over which to cast the Collider(s).</param>
        public int Cast(IEnumerable<ICollisionActor> actors, List<CollisionEventArgs> hitBuffer, Vector2 move, float distance)
        {
            hitBuffer.Clear();
            Vector2 offset = move.NormalizedCopy() * distance;
            return Cast(actors, hitBuffer, offset);
        }

        /// <summary>
        /// Cast an actor's colliders in a direction and check for collisions.
        /// </summary>
        /// <param name="actors">The list of actor colliders to cast.</param>
        /// <param name="hitBuffer">Buffer to receive results.</param>
        /// <param name="offset">Vector representing the translation to use for each cast.</param>
        public int Cast(IEnumerable<ICollisionActor> actors, List<CollisionEventArgs> hitBuffer, Vector2 offset)
        {
            foreach (ICollisionActor actor in actors)
            {
                try
                {
                    actor.Bounds.Position += offset;
                    foreach ((Layer firstLayer, Layer secondLayer) in _layerCollision)
                    {
                        if (!firstLayer.Space.Contains(actor)) continue;
                        foreach (ICollisionActor other in secondLayer.Space.Query(actor.Bounds.BoundingRectangle))
                            if (actor != other && actor.Bounds.Intersects(other.Bounds))
                            {
                                CollisionEventArgs collisionInfo = new() { Other = other, PenetrationVector = CalculatePenetrationVector(actor.Bounds, other.Bounds) };
                                hitBuffer.Add(collisionInfo);
                            }
                    }
                }
                finally { actor.Bounds.Position -= offset; }
            }
            return hitBuffer.Count;
        }


        /// <summary>
        /// Cast an actor's colliders in a direction and check for collisions.
        /// Checks against the specified layer regardless of layer membership.
        /// </summary>
        /// <param name="actors">The list of actor colliders to cast.</param>
        /// <param name="hitBuffer">Buffer to receive results.</param>
        /// <param name="layer">The name of the layer to cast on.</param>
        /// <param name="direction">Normalized vector representing the direction to cast each shape.</param>
        /// <param name="distance">Maximum distance over which to cast the Collider(s).</param>
        public int Cast(IEnumerable<ICollisionActor> actors, List<CollisionEventArgs> hitBuffer, string layer, Vector2 direction, float distance)
            => Cast(actors, hitBuffer, _layers[layer], direction, distance);


        /// <summary>
        /// Cast an actor's colliders in a direction and check for collisions.
        /// Checks against the specified layer regardless of layer membership.
        /// </summary>
        /// <param name="actors">The list of actor colliders to cast.</param>
        /// <param name="hitBuffer">Buffer to receive results.</param>
        /// <param name="layer">The layer to cast on.</param>
        /// <param name="direction">Normalized vector representing the direction to cast each shape.</param>
        /// <param name="distance">Maximum distance over which to cast the Collider(s).</param>
        public int Cast(IEnumerable<ICollisionActor> actors, List<CollisionEventArgs> hitBuffer, Layer layer, Vector2 direction, float distance)
        {
            hitBuffer.Clear();
            Vector2 offset = direction.NormalizedCopy() * distance;
            return Cast(actors, hitBuffer, layer, offset);
        }

        /// <summary>
        /// Cast an actor's colliders in a direction and check for collisions.
        /// Checks against the specified layer regardless of layer membership.
        /// </summary>
        /// <param name="actors">The name of the list of actor colliders to cast.</param>
        /// <param name="hitBuffer">Buffer to receive results.</param>
        /// <param name="layer">The name of the layer to cast on.</param>
        /// <param name="offset">Vector representing the translation to use for each cast.</param>
        public int Cast(IEnumerable<ICollisionActor> actors, List<CollisionEventArgs> hitBuffer, string layer, Vector2 offset)
            => Cast(actors, hitBuffer, _layers[layer], offset);

        /// <summary>
        /// Cast an actor's colliders in a direction and check for collisions.
        /// Checks against the specified layer regardless of layer membership.
        /// </summary>
        /// <param name="actors">The name of the list of actor colliders to cast.</param>
        /// <param name="hitBuffer">Buffer to receive results.</param>
        /// <param name="layer">The layer to cast on.</param>
        /// <param name="offset">Vector representing the translation to use for each cast.</param>
        public int Cast(IEnumerable<ICollisionActor> actors, List<CollisionEventArgs> hitBuffer, Layer layer, Vector2 offset)
        {
            foreach (ICollisionActor actor in actors)
            {
                try
                {
                    actor.Bounds.Position += offset;
                    foreach (ICollisionActor other in layer.Space.Query(actor.Bounds.BoundingRectangle))
                        if (actor != other && actor.Bounds.Intersects(other.Bounds))
                        {
                            CollisionEventArgs collisionInfo = new() { Other = other, PenetrationVector = CalculatePenetrationVector(actor.Bounds, other.Bounds) };
                            hitBuffer.Add(collisionInfo);
                        }
                }
                finally { actor.Bounds.Position -= offset; }
            }
            return hitBuffer.Count;
        }

        /// <summary>
        /// Inserts the target into the collision tree.
        /// The target will have its OnCollision called when collisions occur.
        /// </summary>
        /// <param name="target">Target to insert.</param>
        public void Insert(ICollisionActor target)
        {
            _layers[target.LayerName ?? DEFAULT_LAYER_NAME].Space.Insert(target);
        }

        /// <summary>
        /// Removes the target from the collision tree.
        /// </summary>
        /// <param name="target">Target to remove.</param>
        public void Remove(ICollisionActor target)
        {
            if (target.LayerName is not null)
                _layers[target.LayerName].Space.Remove(target);
            else
                foreach (var layer in _layers.Values)
                    if (layer.Space.Remove(target))
                        return;
        }

        #region Layers

        /// <summary>
        /// Add the new layer. The name of layer must be unique.
        /// </summary>
        /// <param name="name">Name of layer</param>
        /// <param name="layer">The new layer</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null</exception>
        public void Add(string name, Layer layer)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (!_layers.TryAdd(name, layer))
                throw new DuplicateNameException(name);

            if (name != DEFAULT_LAYER_NAME)
                AddCollisionBetweenLayer(_layers[DEFAULT_LAYER_NAME], layer);
        }

        /// <summary>
        /// Remove the layer and all layer's collisions.
        /// </summary>
        /// <param name="name">The name of the layer to delete</param>
        /// <param name="layer">The layer to delete</param>
        public void Remove(string name = null, Layer layer = null)
        {
            name ??= _layers.First(x => x.Value == layer).Key;
            _layers.Remove(name, out layer);
            _layerCollision.RemoveWhere(tuple => tuple.Item1 == layer || tuple.Item2 == layer);
        }

        public void AddCollisionBetweenLayer(Layer a, Layer b)
        {
            _layerCollision.Add((a, b));
        }

        public void AddCollisionBetweenLayer(string nameA, string nameB)
        {
            _layerCollision.Add((_layers[nameA], _layers[nameB]));
        }

        #endregion

        #region Penetration Vectors

        /// <summary>
        /// Calculate a's penetration into b
        /// </summary>
        /// <param name="shapeA">The penetrating shape.</param>
        /// <param name="shapeB">The shape being penetrated.</param>
        /// <returns>The distance vector from the edge of b to a's Position</returns>
        private static Vector2 CalculatePenetrationVector(IShapeF shapeA, IShapeF shapeB)
        {
            switch (shapeA)
            {
                case RectangleF a when shapeB is RectangleF b:
                    return PenetrationVector(a, b);
                case RectangleF a when shapeB is CircleF b:
                    return PenetrationVector(a, b);
                case RectangleF a when shapeB is TriangleF b:
                    return PenetrationVector(a, b);

                case CircleF a when shapeB is CircleF b:
                    return PenetrationVector(a, b);
                case CircleF a when shapeB is RectangleF b:
                    return PenetrationVector(a, b);
                case CircleF a when shapeB is TriangleF b:
                    return PenetrationVector(a, b);

                case TriangleF a when shapeB is CircleF b:
                    return PenetrationVector(a, b);
                case TriangleF a when shapeB is RectangleF b:
                    return PenetrationVector(a, b);
                case TriangleF a when shapeB is TriangleF b:
                    return PenetrationVector(a, b);
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
            if (!circ1.Intersects(circ2))
            {
                return Vector2.Zero;
            }

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
            var collisionPoint = rect.ClosestPointTo(circ.Center);
            var cToCollPoint = collisionPoint - circ.Center;

            if (rect.Contains(circ.Center) || cToCollPoint.Equals(Vector2.Zero))
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
            else
            {
                var penetration = circ.Radius * cToCollPoint.NormalizedCopy() - cToCollPoint;
                return penetration;
            }
        }

        private static Vector2 PenetrationVector(RectangleF rect, CircleF circ)
        {
            return -PenetrationVector(circ, rect);
        }

        public static Vector2 PenetrationVector(TriangleF triangleA, TriangleF triangleB)
        {
            // Calculate the normals of the first triangle
            Vector2 n1 = new(triangleA.B.Y - triangleA.A.Y, triangleA.A.X - triangleA.B.X);
            Vector2 n2 = new(triangleA.C.Y - triangleA.B.Y, triangleA.B.X - triangleA.C.X);
            Vector2 n3 = new(triangleA.A.Y - triangleA.C.Y, triangleA.C.X - triangleA.A.X);

            // Calculate the penetration depth along each normal
            float p1 = PenetrationDepth(triangleA, triangleB, n1);
            float p2 = PenetrationDepth(triangleA, triangleB, n2);
            float p3 = PenetrationDepth(triangleA, triangleB, n3);

            // Choose the smallest penetration depth and corresponding normal
            float minPenetration = Math.Min(p1, Math.Min(p2, p3));
            Vector2 minNormal = minPenetration switch
            {
                _ when Math.Abs(p1 - minPenetration) < float.Epsilon => n1,
                _ when Math.Abs(p2 - minPenetration) < float.Epsilon => n2,
                _ => n3
            };

            // Calculate the penetration vector (MTV) using the chosen normal and penetration depth
            Vector2 penetrationVector = minNormal * minPenetration;

            return penetrationVector;

            float PenetrationDepth(TriangleF triangleA, TriangleF triangleB, Vector2 normal)
            {
                // Project the vertices of both triangles onto the normal and calculate the overlap
                float minA = MinProjection(triangleA, normal);
                float maxA = MaxProjection(triangleA, normal);
                float minB = MinProjection(triangleB, normal);
                float maxB = MaxProjection(triangleB, normal);

                // Calculate the overlap (negative if separated)
                float overlap = Math.Min(maxA, maxB) - Math.Max(minA, minB);

                return overlap;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                float MinProjection(TriangleF triangle, Vector2 normal)
                    => Math.Min(Vector2.Dot(triangle.A, normal),
                        Math.Min(Vector2.Dot(triangle.B, normal), Vector2.Dot(triangle.C, normal)));

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                float MaxProjection(TriangleF triangle, Vector2 normal)
                    => Math.Max(Vector2.Dot(triangle.A, normal),
                        Math.Max(Vector2.Dot(triangle.B, normal), Vector2.Dot(triangle.C, normal)));

            }
        }

        public static Vector2 PenetrationVector(TriangleF triangle, CircleF circle)
        {
            // Calculate the closest point on the triangle to the center of the circle
            Point2 closestPoint = triangle.ClosestPointTo(circle.Center);

            // Calculate the vector from the closest point on the triangle to the center of the circle
            Vector2 penetrationVector = new(circle.Center.X - closestPoint.X, circle.Center.Y - closestPoint.Y);

            // Adjust the penetration vector by the radius of the circle
            float penetrationDepth = penetrationVector.Length() - circle.Radius;
            penetrationVector.Normalize();
            penetrationVector *= penetrationDepth;

            return penetrationVector;
        }

        public static Vector2 PenetrationVector(CircleF circle, TriangleF triangle)
        {
            return -PenetrationVector(triangle, circle);
        }

        public static Vector2 PenetrationVector(TriangleF triangle, RectangleF rectangle)
        {
            // Calculate the closest point on the rectangle to each vertex of the triangle
            Point2 rectNearest = rectangle.ClosestPointTo(triangle.Center);
            Point2 triangleNearest = triangle.ClosestPointTo(rectNearest);

            // Calculate the vector from the closest point on the triangle to the closest point on the rectangle
            Vector2 penetrationVector = new(rectNearest.X - triangleNearest.X, rectNearest.Y - triangleNearest.Y);

            // Adjust the penetration vector by the dimensions of the rectangle
            float penetrationDepthX = Math.Abs(penetrationVector.X) - rectangle.Width / 2;
            float penetrationDepthY = Math.Abs(penetrationVector.Y) - rectangle.Height / 2;
            penetrationVector.X = Math.Sign(penetrationVector.X) * Math.Max(penetrationDepthX, 0);
            penetrationVector.Y = Math.Sign(penetrationVector.Y) * Math.Max(penetrationDepthY, 0);

            return penetrationVector;
        }

        public static Vector2 PenetrationVector(RectangleF rectangle, TriangleF triangle)
        {
            return -PenetrationVector(triangle, rectangle);
        }

        #endregion
    }
}
