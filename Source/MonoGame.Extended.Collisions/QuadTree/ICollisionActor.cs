namespace MonoGame.Extended.Collisions.QuadTree
{
    /// <summary>
    /// An actor that can be collided with.
    /// </summary>
    public interface ICollisionActor
    {
        IShapeF Bounds { get; }

        void OnCollision(CollisionEventArgs collisionInfo);
    }
}