using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Collisions.QuadTree;

namespace MonoGame.Extended.Collisions.Layers;

/// <summary>
///
/// </summary>
public class Layer
{
    /// <summary>
    /// Name of layer
    /// </summary>
    public string Name { get;  }

    public bool IsDynamic { get; set; } = true;

    private readonly Dictionary<ICollisionActor, QuadtreeData> _targetDataDictionary = new();

    private readonly List<ICollisionActor> _actors = new();
    private readonly Quadtree _collisionTree;

    public Layer(string name, RectangleF boundary)
    {
        Name = name;
        _collisionTree = new Quadtree(boundary);
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
    public void Remove(ICollisionActor target)
    {
        if (_targetDataDictionary.ContainsKey(target))
        {
            var data = _targetDataDictionary[target];
            data.RemoveFromAllParents();
            _targetDataDictionary.Remove(target);
            _collisionTree.Shake();
            _actors.Remove(target);
        }
    }

    /// <summary>
    /// Restructure a inner collection, if layer is dynamic, because actors can change own position
    /// </summary>
    public virtual void Reset()
    {
        if (IsDynamic)
        {
            _collisionTree.ClearAll();
            foreach (var value in _targetDataDictionary.Values)
            {
                _collisionTree.Insert(value);
            }
            _collisionTree.Shake();
        }
    }

    /// <summary>
    /// foreach support
    /// </summary>
    /// <returns></returns>
    public List<ICollisionActor>.Enumerator GetEnumerator() => _actors.GetEnumerator();

    /// <inheritdoc cref="Quadtree.Query"/>
    public IEnumerable<ICollisionActor> Query(RectangleF boundsBoundingRectangle)
    {
        return _collisionTree.Query(boundsBoundingRectangle).Select(x => x.Target);
    }
}
