using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    ///     Data structure for the quad tree.
    ///     Holds the entity and collision data for it.
    /// </summary>
    public class QuadTreeData
    {
        public QuadTreeData(ICollisionActor target, int collisionLayerFlags = 1, int collisionMaskFlags = 1)
        {
            Target = target;
            CollisionLayerFlags = collisionLayerFlags;
            CollisionMaskFlags = collisionMaskFlags;
            Bounds = target.Bounds;
        }

        public void RemoveParent(Quadtree parent)
        {
            Parents.Remove(parent);
        }

        public void AddParent(Quadtree parent)
        {
            Parents.Add(parent);
        }

        public void RemoveFromAllParents()
        {
            foreach (Quadtree parent in Parents.ToList())
            {
                parent.Remove(this);
            }
            Parents.Clear();
        }

        /// <summary>
        ///     The layer that other colliders collide with.
        /// </summary>
        public int CollisionLayerFlags { get; }

        /// <summary>
        ///     The layer that this collider collides with.
        /// </summary>
        public int CollisionMaskFlags { get; }

        public HashSet<Quadtree> Parents = new HashSet<Quadtree>();

        /// <summary>
        ///     Gets or sets the Target for collision.
        /// </summary>
        public ICollisionActor Target { get; set; }

        /// <summary>
        ///     Gets or sets whether Target has had its collision handled this
        ///     iteration.
        /// </summary>
        public bool Dirty { get; private set; }

        public void MarkDirty()
        {
            Dirty = true;
        }

        public void MarkClean()
        {
            Dirty = false;
        }

        private Point2 _previousPosition;

        /// <summary>
        /// Indicating whether the position of the target bounds changed.
        /// </summary>
        public bool PositionDirty => _previousPosition != Target.Bounds.Position;

        public void MarkPositionClean()
        {
            _previousPosition = Target.Bounds.Position;
        }

        /// <summary>
        ///     Gets or sets the bounding box for collision detection.
        /// </summary>
        public IShapeF Bounds { get; set; }
    }
}