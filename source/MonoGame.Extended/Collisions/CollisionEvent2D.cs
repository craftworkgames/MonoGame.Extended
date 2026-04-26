using System;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Represents collision data for an actor collision callback or query result.
    /// </summary>
    /// <remarks>
    /// The collision <see cref="Result"/> is relative to the receiving actor.
    /// <see cref="CollisionResult2D.MinimumTranslationVector"/> moves the receiving actor out of <see cref="Other"/>.
    /// </remarks>
    public sealed class CollisionEvent2D : EventArgs
    {
        /// <summary>
        /// Gets the other actor involved in the collision.
        /// </summary>
        public required ICollisionActor Other { get; init; }

        /// <summary>
        /// Gets the stable identity of the other actor involved in the collision.
        /// </summary>
        public int OtherId
        {
            get
            {
                return Other.Id;
            }
        }

        /// <summary>
        /// Gets the collision result for this actor relative to <see cref="Other"/>.
        /// </summary>
        /// <value>
        /// A collision result whose direction data moves the receiving actor out of <see cref="Other"/>.
        /// </value>
        public required CollisionResult2D Result { get; init; }
    }
}
