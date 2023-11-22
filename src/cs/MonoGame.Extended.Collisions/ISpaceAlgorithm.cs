using System.Collections.Generic;

namespace MonoGame.Extended.Collisions;

public interface ISpaceAlgorithm
{
    void Insert(ICollisionActor actor);

    bool Remove(ICollisionActor actor);

    IEnumerable<ICollisionActor> Query(RectangleF boundsBoundingRectangle);

    List<ICollisionActor>.Enumerator GetEnumerator();

    /// <summary>
    /// Restructure a space with new positions
    /// </summary>
    void Reset();
}
