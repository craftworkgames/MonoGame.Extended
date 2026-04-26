namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Defines an actor that participates in collision queries.
    /// </summary>
    /// <remarks>
    /// This contract belongs to the actor/world collision layer.
    /// Low-level collision math remains in <see cref="Collision2D"/> and the bounding volume types.
    /// </remarks>
    public interface ICollisionActor
    {
        /// <summary>
        /// Gets the stable identity of this actor for collision reporting.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the name of the collision layer that contains this actor.
        /// </summary>
        /// <value>
        /// The layer name for this actor, or <see langword="null"/> to use the default collision layer.
        /// </value>
        string LayerName { get => null; }

        /// <summary>
        /// Gets the collision shape used for broadphase and narrowphase collision queries.
        /// </summary>
        CollisionShape2D Shape { get; }

        /// <summary>
        /// Called when this actor collides with another actor.
        /// </summary>
        /// <param name="collisionInfo">The collision data for this actor.</param>
        void OnCollision(CollisionEventArgs collisionInfo);
    }
}
