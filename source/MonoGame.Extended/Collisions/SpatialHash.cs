using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Collisions;

/// <summary>
/// Stores collision actors in a fixed-size spatial hash for broadphase overlap queries.
/// </summary>
public class SpatialHash : ICollisionBroadphase2D
{
    private readonly Dictionary<CellKey, List<ICollisionActor>> _cells = new();
    private readonly Dictionary<ICollisionActor, List<CellKey>> _actorCells = new();
    private readonly List<ICollisionActor> _actors = new();
    private readonly SizeF _cellSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpatialHash"/> class.
    /// </summary>
    /// <param name="size">The width and height of each hash cell in world units.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="size"/> has a width or height less than or equal to zero.
    /// </exception>
    public SpatialHash(SizeF size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size.Width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size.Height);

        _cellSize = size;
    }

    /// <summary>
    /// Inserts the specified actor into the spatial hash.
    /// </summary>
    /// <param name="actor">The actor to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    public void Insert(ICollisionActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        if (_actorCells.ContainsKey(actor))
        {
            return;
        }

        _actors.Add(actor);
        InsertIntoCells(actor);
    }

    /// <summary>
    /// Removes the specified actor from the spatial hash.
    /// </summary>
    /// <param name="actor">The actor to remove.</param>
    /// <returns><see langword="true"/> if the actor was removed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    public bool Remove(ICollisionActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        if (!_actorCells.TryGetValue(actor, out List<CellKey> occupiedCells))
        {
            return false;
        }

        foreach (CellKey cell in occupiedCells)
        {
            if (_cells.TryGetValue(cell, out List<ICollisionActor> actors))
            {
                actors.Remove(actor);

                if (actors.Count == 0)
                {
                    _cells.Remove(cell);
                }
            }
        }

        _actorCells.Remove(actor);
        _actors.Remove(actor);
        return true;
    }

    /// <summary>
    /// Queries the spatial hash for actors whose broadphase bounds overlap the specified area.
    /// </summary>
    /// <param name="bounds">The axis-aligned query bounds in world space.</param>
    /// <returns>The actors whose broadphase bounds overlap <paramref name="bounds"/>.</returns>
    public IEnumerable<ICollisionActor> Query(BoundingBox2D bounds)
    {
        HashSet<ICollisionActor> results = new();
        GetCellRange(bounds, out int minX, out int minY, out int maxX, out int maxY);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                CellKey cell = new CellKey(x, y);

                if (_cells.TryGetValue(cell, out List<ICollisionActor> actors))
                {
                    foreach (ICollisionActor actor in actors)
                    {
                        if (bounds.Intersects(actor.Shape.BoundingBox))
                        {
                            results.Add(actor);
                        }
                    }
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Returns an enumerator for the actors currently stored in the spatial hash.
    /// </summary>
    /// <returns>An enumerator over the stored actors.</returns>
    public List<ICollisionActor>.Enumerator GetEnumerator()
    {
        return _actors.GetEnumerator();
    }

    /// <summary>
    /// Rebuilds the spatial hash using the actors' current broadphase bounds.
    /// </summary>
    public void Reset()
    {
        _cells.Clear();
        _actorCells.Clear();

        foreach (ICollisionActor actor in _actors)
        {
            InsertIntoCells(actor);
        }
    }

    private void InsertIntoCells(ICollisionActor actor)
    {
        BoundingBox2D actorBounds = actor.Shape.BoundingBox;
        List<CellKey> occupiedCells = new();
        GetCellRange(actorBounds, out int minX, out int minY, out int maxX, out int maxY);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                AddToCell(x, y, actor);
                occupiedCells.Add(new CellKey(x, y));
            }
        }

        _actorCells[actor] = occupiedCells;
    }

    private void AddToCell(int x, int y, ICollisionActor actor)
    {
        CellKey cell = new CellKey(x, y);

        if (_cells.TryGetValue(cell, out List<ICollisionActor> actors))
        {
            actors.Add(actor);
        }
        else
        {
            _cells[cell] = new List<ICollisionActor> { actor };
        }
    }

    private void GetCellRange(BoundingBox2D bounds, out int minX, out int minY, out int maxX, out int maxY)
    {
        minX = GetCellIndex(bounds.Min.X, _cellSize.Width);
        minY = GetCellIndex(bounds.Min.Y, _cellSize.Height);
        maxX = GetCellIndex(bounds.Max.X, _cellSize.Width);
        maxY = GetCellIndex(bounds.Max.Y, _cellSize.Height);
    }

    private static int GetCellIndex(float value, float cellSize)
    {
        return (int)MathF.Floor(value / cellSize);
    }

    private readonly struct CellKey : IEquatable<CellKey>
    {
        public int X { get; }

        public int Y { get; }

        public CellKey(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(CellKey other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is CellKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
