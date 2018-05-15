namespace MonoGame.Extended.Collisions
{
    /// <summary>
    ///     Data structure for the quad tree.
    ///     Holds the entity and collision data for it.
    /// </summary>
    public class QuadTreeData
    {
        public QuadTreeData(IActorTarget target)
        {
            Target = target;
            Flag = false;
        }

        /// <summary>
        ///     Gets or sets the Target for collision.
        /// </summary>
        public IActorTarget Target { get; set; }

        /// <summary>
        ///     Gets or sets whether Target has had its collision handled this
        ///     iteration.
        /// </summary>
        public bool Flag { get; set; }
    }
}