using System;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// An actor that can be collided with.
    /// </summary>
    public interface ICollisionActor
    {
        /// <summary>
        /// A name of layer, which will contains this actor.
        /// If it equals null, an actor will insert into a default layer
        /// </summary>
        string LayerName { get => null; }

        /// <summary>
        /// A bounds of an actor. It is using for collision calculating
        /// </summary>
        IShapeF Bounds { get; }

        /// <summary>
        /// It will called, when collision with an another actor fires
        /// </summary>
        /// <param name="collisionInfo">Data about collision</param>
        void OnCollision(CollisionEventArgs collisionInfo);
    }
}
