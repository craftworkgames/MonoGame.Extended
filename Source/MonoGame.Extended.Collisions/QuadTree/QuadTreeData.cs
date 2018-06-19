namespace MonoGame.Extended.Collisions
{
    /// <summary>
    ///     Data structure for the quad tree.
    ///     Holds the entity and collision data for it.
    /// </summary>
    public class QuadtreeData
    {
        public QuadtreeData(ICollisionActor target)
        {
            Target = target;
            Bounds = target.Bounds;
            Flag = false;
        }

        /// <summary>
        ///     Gets or sets the Target for collision.
        /// </summary>
        public ICollisionActor Target { get; set; }

        /// <summary>
        ///     Gets or sets whether Target has had its collision handled this
        ///     iteration.
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        ///     Gets or sets the bounding box for collision detection.
        /// </summary>
        public IShapeF Bounds { get; set; }
    }
}