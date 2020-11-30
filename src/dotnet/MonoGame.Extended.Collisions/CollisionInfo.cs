using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// This class holds data on a collision. It is passed as a parameter to
    /// OnCollision methods.
    /// </summary>
    public class CollisionInfo
    {
        /// <summary>
        /// Gets the object being collided with.
        /// </summary>
        public ICollidable Other { get; internal set; }

        /// <summary>
        /// Gets a vector representing the overlap between the two objects.
        /// </summary>
        public Vector2 PenetrationVector { get; internal set; }
    }
}