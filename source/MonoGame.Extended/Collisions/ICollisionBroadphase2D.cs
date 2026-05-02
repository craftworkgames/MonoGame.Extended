using System.Collections.Generic;

namespace MonoGame.Extended.Collisions;

/// <summary>
/// Defines a broadphase structure for collision actors.
/// </summary>
public interface ICollisionBroadphase2D
{
    /// <summary>
    /// Inserts the specified actor into the broadphase structure.
    /// </summary>
    /// <param name="actor">The actor to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    void Insert(ICollisionActor actor);

    /// <summary>
    /// Removes the specified actor from the broadphase structure.
    /// </summary>
    /// <param name="actor">The actor to remove.</param>
    /// <returns>
    /// <see langword="true"/> if the actor was removed; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    bool Remove(ICollisionActor actor);

    /// <summary>
    /// Queries the broadphase structure for actors whose broadphase bounds overlap the specified area.
    /// </summary>
    /// <param name="bounds">The axis-aligned query bounds in world space.</param>
    /// <returns>The actors whose broadphase bounds overlap <paramref name="bounds"/>.</returns>
    IEnumerable<ICollisionActor> Query(BoundingBox2D bounds);

    /// <summary>
    /// Returns an enumerator for the actors currently stored in the broadphase structure.
    /// </summary>
    /// <returns>An enumerator over the stored actors.</returns>
    List<ICollisionActor>.Enumerator GetEnumerator();

    /// <summary>
    /// Rebuilds or refreshes the broadphase structure using the actors' current broadphase bounds.
    /// </summary>
    void Reset();
}
