using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Extended.Collisions.QuadTree;

/// <summary>
/// Data structure for the quad tree.
/// Holds the entity and collision data for it.
/// </summary>
public class QuadtreeData
{
    private readonly ICollisionActor _target;
    private readonly HashSet<QuadTree> _parents = new();

    /// <summary>
    /// Initialize a new instance of QuadTreeData.
    /// </summary>
    /// <param name="target"></param>
    public QuadtreeData(ICollisionActor target)
    {
        _target = target;
        Bounds = _target.Bounds.BoundingRectangle;
    }

    /// <summary>
    /// Remove a parent node.
    /// </summary>
    /// <param name="parent"></param>
    public void RemoveParent(QuadTree parent)
    {
        _parents.Remove(parent);
    }

    /// <summary>
    /// Add a parent node.
    /// </summary>
    /// <param name="parent"></param>
    public void AddParent(QuadTree parent)
    {
        _parents.Add(parent);
        Bounds = _target.Bounds.BoundingRectangle;
    }

    /// <summary>
    /// Remove all parent nodes from this node.
    /// </summary>
    public void RemoveFromAllParents()
    {
        foreach (var parent in _parents.ToList())
        {
            parent.Remove(this);
        }

        _parents.Clear();
    }

    /// <summary>
    /// Gets the bounding box for collision detection.
    /// </summary>
    public RectangleF Bounds { get; set; }

    /// <summary>
    /// Gets the collision actor target.
    /// </summary>
    public ICollisionActor Target => _target;

    /// <summary>
    /// Gets or sets whether Target has had its collision handled this
    /// iteration.
    /// </summary>
    public bool Dirty { get; private set; }

    /// <summary>
    /// Mark node as dirty.
    /// </summary>
    public void MarkDirty()
    {
        Dirty = true;
    }

    /// <summary>
    /// Mark node as clean, i.e. not dirty.
    /// </summary>
    public void MarkClean()
    {
        Dirty = false;
    }
}
