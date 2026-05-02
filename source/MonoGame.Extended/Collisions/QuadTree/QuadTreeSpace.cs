using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Collisions.QuadTree;

/// <summary>
/// Adapts <see cref="QuadTree"/> to the <see cref="ICollisionBroadphase2D"/> contract.
/// </summary>
public class QuadTreeSpace : ICollisionBroadphase2D
{
    private readonly QuadTree _collisionTree;
    private readonly List<ICollisionActor> _actors = new();
    private readonly Dictionary<ICollisionActor, QuadtreeData> _actorData = new();

    /// <summary>
    /// Initializes a new quadtree-backed collision space for the specified axis-aligned boundary.
    /// </summary>
    /// <param name="boundary">The axis-aligned world boundary covered by the quadtree.</param>
    public QuadTreeSpace(BoundingBox2D boundary)
    {
        _collisionTree = new QuadTree(boundary);
    }

    /// <summary>
    /// Inserts the specified actor into the quadtree broadphase.
    /// </summary>
    /// <param name="actor">The actor to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    public void Insert(ICollisionActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        if (!_actorData.ContainsKey(actor))
        {
            QuadtreeData data = new QuadtreeData(actor);
            _actorData.Add(actor, data);
            _collisionTree.Insert(data);
            _actors.Add(actor);
        }
    }

    /// <summary>
    /// Removes the specified actor from the quadtree broadphase.
    /// </summary>
    /// <param name="actor">The actor to remove.</param>
    /// <returns><see langword="true"/> if the actor was removed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    public bool Remove(ICollisionActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        if (_actorData.TryGetValue(actor, out QuadtreeData data))
        {
            data.RemoveFromAllParents();
            _actorData.Remove(actor);
            _collisionTree.Shake();
            _actors.Remove(actor);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Queries the quadtree broadphase for actors whose bounds overlap the specified area.
    /// </summary>
    /// <param name="bounds">The axis-aligned query bounds in world space.</param>
    /// <returns>The actors whose broadphase bounds overlap <paramref name="bounds"/>.</returns>
    public IEnumerable<ICollisionActor> Query(BoundingBox2D bounds)
    {
        foreach (QuadtreeData data in _collisionTree.Query(bounds))
        {
            yield return data.Target;
        }
    }

    /// <summary>
    /// Returns an enumerator for the actors currently stored in the quadtree broadphase.
    /// </summary>
    /// <returns>An enumerator over the stored actors.</returns>
    public List<ICollisionActor>.Enumerator GetEnumerator()
    {
        return _actors.GetEnumerator();
    }

    /// <summary>
    /// Rebuilds the quadtree using each actor's current broadphase bounds.
    /// </summary>
    public void Reset()
    {
        _collisionTree.ClearAll();

        foreach (QuadtreeData value in _actorData.Values)
        {
            _collisionTree.Insert(value);
        }

        _collisionTree.Shake();
    }
}
