namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// An actor that can be collided with.
    /// </summary>
    public interface ICollisionActor
    {
        int CollisionLayerFlags { get; }
        int CollisionMaskFlags { get; }

        IShapeF Bounds { get; }

        void OnCollision(CollisionEventArgs collisionInfo);
    }
}