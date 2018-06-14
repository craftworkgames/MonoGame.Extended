using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// This class holds data on a collision. It is passed as a parameter to
    /// OnCollision methods.
    /// </summary>
    public class CollisionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the object being collided with.
        /// </summary>
        public ICollisionActor Other { get; internal set; }

        /// <summary>
        /// Gets a vector representing the overlap between the two objects.
        /// </summary>
        /// <remarks>
        /// This vector starts at the edge of <see cref="Other"/> and ends at
        /// the Actor's location.
        /// </remarks>
        public Vector2 PenetrationVector { get; internal set; }
    }
}