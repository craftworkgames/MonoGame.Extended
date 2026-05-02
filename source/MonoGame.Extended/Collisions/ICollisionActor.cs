namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Defines an actor that participates in collision queries.
    /// </summary>
    public interface ICollisionActor
    {
        /// <summary>
        /// Gets the stable identity of this actor for collision reporting.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the collision shape used for broadphase and narrowphase collision queries.
        /// </summary>
        CollisionShape2D Shape { get; }
    }
}
