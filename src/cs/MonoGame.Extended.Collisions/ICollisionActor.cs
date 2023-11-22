using System;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// An actor that can be collided with.
    /// </summary>
    public interface ICollisionActor
    {
        string LayerName { get => null; }
        IShapeF Bounds { get; }

        void OnCollision(CollisionEventArgs collisionInfo);
    }
}
