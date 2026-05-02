using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Extended.Collisions.QuadTree;

/// <summary>
/// Stores an actor and the quadtree nodes that currently reference it.
/// </summary>
public class QuadtreeData
{
    private readonly HashSet<QuadTree> _parents = new();

    /// <summary>
    /// Gets the bounding box used to place this actor in the quadtree.
    /// </summary>
    public BoundingBox2D Bounds { get; set; }

    /// <summary>
    /// Gets a value indicating whether this entry has already been visited during the current traversal.
    /// </summary>
    public bool Dirty { get; private set; }

    /// <summary>
    /// Gets the collision actor stored in this entry.
    /// </summary>
    public ICollisionActor Target { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuadtreeData"/> class.
    /// </summary>
    /// <param name="target">The actor stored in this entry.</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    public QuadtreeData(ICollisionActor target)
    {
        ArgumentNullException.ThrowIfNull(target);

        Target = target;
        Bounds = target.Shape.BoundingBox;
    }

    /// <summary>
    /// Removes a parent node reference from this entry.
    /// </summary>
    /// <param name="parent">The parent node to remove.</param>
    /// <exception cref="ArgumentNullException"><paramref name="parent"/> is <see langword="null"/>.</exception>
    public void RemoveParent(QuadTree parent)
    {
        ArgumentNullException.ThrowIfNull(parent);

        _parents.Remove(parent);
    }

    /// <summary>
    /// Adds a parent node reference to this entry and refreshes its cached bounds.
    /// </summary>
    /// <param name="parent">The parent node to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="parent"/> is <see langword="null"/>.</exception>
    public void AddParent(QuadTree parent)
    {
        ArgumentNullException.ThrowIfNull(parent);

        _parents.Add(parent);
        Bounds = Target.Shape.BoundingBox;
    }

    /// <summary>
    /// Removes this entry from every parent node that currently contains it.
    /// </summary>
    public void RemoveFromAllParents()
    {
        foreach (QuadTree parent in _parents.ToList())
        {
            parent.Remove(this);
        }

        _parents.Clear();
    }

    /// <summary>
    /// Marks this entry as visited for the current traversal.
    /// </summary>
    public void MarkDirty()
    {
        Dirty = true;
    }

    /// <summary>
    /// Marks this entry as unvisited for the next traversal.
    /// </summary>
    public void MarkClean()
    {
        Dirty = false;
    }
}
