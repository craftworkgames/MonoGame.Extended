using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MonoGame.Extended.Collisions;

public class SpatialHash: ISpaceAlgorithm
{
    private readonly Dictionary<int, List<ICollisionActor>> _dictionary = new();
    private readonly List<ICollisionActor> _actors = new();

    private readonly SizeF _size;

    public SpatialHash(SizeF size)
    {
        _size = size;
    }

    public void Insert(ICollisionActor actor)
    {
        InsertToHash(actor);
        _actors.Add(actor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InsertToHash(ICollisionActor actor)
    {
        var rect = actor.Bounds.BoundingRectangle;
        for (var x = rect.Left; x < rect.Right; x+=_size.Width)
        for (var y = rect.Top; y < rect.Bottom; y+=_size.Height)
            AddToCell(x, y, actor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddToCell(float x, float y, ICollisionActor actor)
    {
        var index = GetIndex(x, y);
        if (_dictionary.TryGetValue(index, out var actors))
            actors.Add(actor);
        else
            _dictionary[index] = new() { actor };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndex(float x, float y)
    {
        return (int)(x / _size.Width) << 16 + (int)(y / _size.Height);
    }

    public bool Remove(ICollisionActor actor)
    {
        foreach (var actors in _dictionary.Values)
            actors.Remove(actor);
        return _actors.Remove(actor);
    }

    public IEnumerable<ICollisionActor> Query(RectangleF boundsBoundingRectangle)
    {
        var results = new HashSet<ICollisionActor>();
        var bounds = boundsBoundingRectangle.BoundingRectangle;

        for (var x = boundsBoundingRectangle.Left; x < boundsBoundingRectangle.Right; x+=_size.Width)
        for (var y = boundsBoundingRectangle.Top; y < boundsBoundingRectangle.Bottom; y+=_size.Height)
            if (_dictionary.TryGetValue(GetIndex(x, y), out var actors))
                foreach (var actor in actors)
                    if (bounds.Intersects(actor.Bounds))
                        results.Add(actor);
        return results;
    }

    public List<ICollisionActor>.Enumerator GetEnumerator() => _actors.GetEnumerator();

    public void Reset()
    {
        _dictionary.Clear();
        foreach (var actor in _actors)
            InsertToHash(actor);
    }
}
