using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Extended.Collisions.QuadTree;

public class QuadTreeSpace : ICollisionBroadphase2D
{
    private readonly QuadTree _collisionTree;
    private readonly List<ICollisionActor> _actors = new();
    private readonly Dictionary<ICollisionActor, QuadtreeData> _targetDataDictionary = new();

    /// <summary>
    /// Initializes a new quadtree-backed collision space for the specified axis-aligned boundary.
    /// </summary>
    /// <param name="boundary">The axis-aligned world boundary covered by the quadtree.</param>
    public QuadTreeSpace(BoundingBox2D boundary)
    {
        _collisionTree = new QuadTree(boundary);
    }

    /// <summary>
    /// Inserts the target into the collision tree.
    /// The target will have its OnCollision called when collisions occur.
    /// </summary>
    /// <param name="target">Target to insert.</param>
    public void Insert(ICollisionActor target)
    {
        if (!_targetDataDictionary.ContainsKey(target))
        {
            var data = new QuadtreeData(target);
            _targetDataDictionary.Add(target, data);
            _collisionTree.Insert(data);
            _actors.Add(target);
        }
    }

    /// <summary>
    /// Removes the target from the collision tree.
    /// </summary>
    /// <param name="target">Target to remove.</param>
    public bool Remove(ICollisionActor target)
    {
        if (_targetDataDictionary.ContainsKey(target))
        {
            var data = _targetDataDictionary[target];
            data.RemoveFromAllParents();
            _targetDataDictionary.Remove(target);
            _collisionTree.Shake();
            _actors.Remove(target);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Restructure a inner collection, if layer is dynamic, because actors can change own position
    /// </summary>
    public void Reset()
    {
        _collisionTree.ClearAll();
        foreach (var value in _targetDataDictionary.Values)
        {
            _collisionTree.Insert(value);
        }
        _collisionTree.Shake();
    }

    /// <summary>
    /// foreach support
    /// </summary>
    /// <returns></returns>
    public List<ICollisionActor>.Enumerator GetEnumerator() => _actors.GetEnumerator();

    /// <inheritdoc cref="QuadTree.Query"/>
    public IEnumerable<ICollisionActor> Query(BoundingBox2D bounds)
    {
        return _collisionTree.Query(bounds).Select(x => x.Target);
    }
}
