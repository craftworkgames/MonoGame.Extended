using System.Collections.Generic;

namespace MonoGame.Extended.Collisions;

/// <summary>
/// Interface, which split space for optimization of collisions.
/// </summary>
public interface ISpaceAlgorithm
{
    /// <summary>
    /// Inserts the actor into the space.
    /// The actor will have its OnCollision called when collisions occur.
    /// </summary>
    /// <param name="actor">Actor to insert.</param>
    void Insert(ICollisionActor actor);

    /// <summary>
    /// Removes the actor into the space.
    /// </summary>
    /// <param name="actor">Actor to remove.</param>
    bool Remove(ICollisionActor actor);

    /// <summary>
    /// Removes the actor into the space.
    /// The actor will have its OnCollision called when collisions occur.
    /// </summary>
    /// <param name="actor">Actor to remove.</param>
    IEnumerable<ICollisionActor> Query(RectangleF boundsBoundingRectangle);

    /// <summary>
    /// for foreach
    /// </summary>
    /// <returns></returns>
    List<ICollisionActor>.Enumerator GetEnumerator();

    /// <summary>
    /// Restructure the space with new positions.
    /// </summary>
    void Reset();
}
