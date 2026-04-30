using System;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Represents collision data for an actor collision callback or query result.
    /// </summary>
    public sealed class CollisionEvent2D : EventArgs
    {
        /// <summary>
        /// Gets the other actor involved in the collision.
        /// </summary>
        public required ICollisionActor Other { get; init; }

        /// <summary>
        /// Gets the stable identity of the other actor involved in the collision.
        /// </summary>
        public int OtherId => Other.Id;

        /// <summary>
        /// Gets the collision result for this actor relative to <see cref="Other"/>.
        /// </summary>
        public required CollisionResult2D Result { get; init; }
    }
}
