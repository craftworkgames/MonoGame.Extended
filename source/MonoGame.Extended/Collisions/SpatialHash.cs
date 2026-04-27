using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MonoGame.Extended.Collisions;

public class SpatialHash : ICollisionBroadphase2D
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
        RectangleF rect = ToRectangleF(actor.Shape.BoundingBox);

        for (float x = rect.Left; x < rect.Right; x += _size.Width)
        for (float y = rect.Top; y < rect.Bottom; y += _size.Height)
            AddToCell(x, y, actor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddToCell(float x, float y, ICollisionActor actor)
    {
        int index = GetIndex(x, y);
        if (_dictionary.TryGetValue(index, out List<ICollisionActor> actors))
            actors.Add(actor);
        else
            _dictionary[index] = new List<ICollisionActor> { actor };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndex(float x, float y)
    {
        return (int)(x / _size.Width) << 16 + (int)(y / _size.Height);
    }

    public bool Remove(ICollisionActor actor)
    {
        foreach (List<ICollisionActor> actors in _dictionary.Values)
            actors.Remove(actor);

        return _actors.Remove(actor);
    }

    public IEnumerable<ICollisionActor> Query(BoundingBox2D boundsBoundingBox)
    {
        HashSet<ICollisionActor> results = new();
        RectangleF boundsBoundingRectangle = ToRectangleF(boundsBoundingBox);
        RectangleF bounds = boundsBoundingRectangle.BoundingRectangle;

        for (float x = boundsBoundingRectangle.Left; x < boundsBoundingRectangle.Right; x += _size.Width)
        for (float y = boundsBoundingRectangle.Top; y < boundsBoundingRectangle.Bottom; y += _size.Height)
        {
            if (_dictionary.TryGetValue(GetIndex(x, y), out List<ICollisionActor> actors))
            {
                foreach (ICollisionActor actor in actors)
                {
                    if (bounds.Intersects(ToRectangleF(actor.Shape.BoundingBox)))
                        results.Add(actor);
                }
            }
        }

        return results;
    }

    public List<ICollisionActor>.Enumerator GetEnumerator() => _actors.GetEnumerator();

    public void Reset()
    {
        _dictionary.Clear();
        foreach (ICollisionActor actor in _actors)
            InsertToHash(actor);
    }

    private static RectangleF ToRectangleF(BoundingBox2D boundingBox)
    {
        return new RectangleF(boundingBox.Min, new SizeF(boundingBox.Size.X, boundingBox.Size.Y));
    }
}
