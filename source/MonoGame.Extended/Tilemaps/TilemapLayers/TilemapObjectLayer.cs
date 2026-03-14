using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a layer containing drawable objects.
/// </summary>
public class TilemapObjectLayer : TilemapLayer
{
    private readonly List<TilemapObject> _objects = new List<TilemapObject>();

    /// <summary>
    /// Gets the collection of objects in this layer.
    /// </summary>
    public IReadOnlyList<TilemapObject> Objects
    {
        get
        {
            return _objects;
        }
    }

    /// <summary>
    /// Gets or sets the rendering order for objects in this layer.
    /// </summary>
    public TilemapObjectDrawOrder DrawOrder { get; set; }

    /// <inheritdoc/>
    public override Rectangle Bounds
    {
        get
        {
            if (_objects.Count == 0)
            {
                return Rectangle.Empty;
            }

            BoundingBox2D? unionBounds = null;

            foreach (TilemapObject obj in _objects)
            {
                BoundingBox2D objBounds = obj.Bounds;

                if (unionBounds == null)
                {
                    unionBounds = objBounds;
                }
                else
                {
                    unionBounds = BoundingBox2D.CreateMerged(unionBounds.Value, objBounds);
                }
            }

            BoundingBox2D bounds = unionBounds.Value;

            return new Rectangle(
                (int)bounds.Min.X,
                (int)bounds.Min.Y,
                (int)Math.Ceiling(bounds.Max.X - bounds.Min.X),
                (int)Math.Ceiling(bounds.Max.Y - bounds.Min.Y)
            );
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapObjectLayer"/> class.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    public TilemapObjectLayer(string name) : base(name)
    {
        DrawOrder = TilemapObjectDrawOrder.TopDown;
    }

    /// <summary>
    /// Adds an object to the layer.
    /// </summary>
    /// <param name="obj">The object to add.</param>
    public void AddObject(TilemapObject obj)
    {
        _objects.Add(obj);
    }

    /// <summary>
    /// Removes an object from the layer.
    /// </summary>
    /// <param name="obj">The object to remove.</param>
    /// <returns><see langword="true"/> if the object was removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveObject(TilemapObject obj)
    {
        return _objects.Remove(obj);
    }

    /// <summary>
    /// Gets the object with the specified ID.
    /// </summary>
    /// <param name="id">The object ID.</param>
    /// <returns>The object with the specified ID, or <see langword="null"/> if not found.</returns>
    public TilemapObject GetObject(int id)
    {
        foreach (TilemapObject obj in _objects)
        {
            if (obj.Id == id)
            {
                return obj;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all objects whose bounds intersect the specified region.
    /// </summary>
    /// <param name="region">The region in world coordinates.</param>
    /// <returns>An enumerable of objects within the region.</returns>
    public IEnumerable<TilemapObject> GetObjectsInRegion(BoundingBox2D region)
    {
        foreach (TilemapObject obj in _objects)
        {
            if (region.Intersects(obj.Bounds))
            {
                yield return obj;
            }
        }
    }

    /// <summary>
    /// Gets all objects of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of objects to retrieve.</typeparam>
    /// <returns>An enumerable of objects of the specified type.</returns>
    public IEnumerable<T> GetObjects<T>() where T : TilemapObject
    {
        foreach (TilemapObject obj in _objects)
        {
            if (obj is T typedObj)
            {
                yield return typedObj;
            }
        }
    }
}
