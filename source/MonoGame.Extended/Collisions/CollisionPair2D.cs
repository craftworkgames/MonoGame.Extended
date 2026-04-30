namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Represents a collision pair and the collision results for both actors.
    /// </summary>
    public sealed class CollisionPair2D
    {
        /// <summary>
        /// Gets the first actor in the collision pair.
        /// </summary>
        public required ICollisionActor First { get; init; }

        /// <summary>
        /// Gets the stable identity of the first actor in the collision pair.
        /// </summary>
        public int FirstId => First.Id;

        /// <summary>
        /// Gets the second actor in the collision pair.
        /// </summary>
        public required ICollisionActor Second { get; init; }

        /// <summary>
        /// Gets the stable identity of the second actor in the collision pair.
        /// </summary>
        public int SecondId => Second.Id;

        /// <summary>
        /// Gets the collision result that moves <see cref="First"/> out of <see cref="Second"/>.
        /// </summary>
        public required CollisionResult2D FirstResult { get; init; }

        /// <summary>
        /// Gets the collision result that moves <see cref="Second"/> out of <see cref="First"/>.
        /// </summary>
        public CollisionResult2D SecondResult
        {
            get
            {
                return FirstResult.Invert();
            }
        }
    }
}
