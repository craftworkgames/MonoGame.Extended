using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// An actor that can be collided with.
    /// </summary>
    public interface ICollisionActor
    {
        IShapeF Bounds { get; }

        void OnCollision(CollisionEventArgs collisionInfo);
    }

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
        public Vector2 PenetrationVector { get; internal set; }
    }
}