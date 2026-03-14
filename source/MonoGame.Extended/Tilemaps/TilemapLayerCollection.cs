using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// A collection of tilemap layers.
/// </summary>
public class TilemapLayerCollection : IReadOnlyList<TilemapLayer>
{
    private readonly List<TilemapLayer> _layers = new List<TilemapLayer>();
    private readonly Dictionary<string, TilemapLayer> _layersByName = new Dictionary<string, TilemapLayer>();

    /// <summary>
    /// Gets the layer at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the layer to get.</param>
    /// <returns>The layer at the specified index.</returns>
    public TilemapLayer this[int index]
    {
        get
        {
            return _layers[index];
        }
    }

    /// <summary>
    /// Gets the layer with the specified name.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    /// <returns>The layer with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">No layer with the specified name was found.</exception>
    public TilemapLayer this[string name]
    {
        get
        {
            return _layersByName[name];
        }
    }

    /// <summary>
    /// Gets the number of layers in the collection.
    /// </summary>
    public int Count => _layers.Count;

    /// <summary>
    /// Adds a layer to the collection.
    /// </summary>
    /// <param name="layer">The layer to add.</param>
    public void Add(TilemapLayer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        _layers.Add(layer);
        _layersByName[layer.Name] = layer;
    }

    /// <summary>
    /// Removes a layer from the collection.
    /// </summary>
    /// <param name="layer">The layer to remove.</param>
    /// <returns><see langword="true"/> if the layer was removed; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// This is an O(n) operation.
    /// </remarks>
    public bool Remove(TilemapLayer layer)
    {
        if (layer == null)
        {
            return false;
        }

        bool removed = _layers.Remove(layer);
        if (removed)
        {
            _layersByName.Remove(layer.Name);
        }

        return removed;
    }

    /// <summary>
    /// Removes all layers from the collection.
    /// </summary>
    public void Clear()
    {
        _layers.Clear();
        _layersByName.Clear();
    }

    /// <summary>
    /// Attempts to get the layer with the specified name.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    /// <param name="layer">
    /// When this method returns, contains the layer with the specified name,
    /// if found; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/> if the layer was found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetValue(string name, out TilemapLayer layer)
    {
        return _layersByName.TryGetValue(name, out layer);
    }

    /// <summary>
    /// Gets the layer with the specified name and type.
    /// </summary>
    /// <typeparam name="T">The type of layer to retrieve.</typeparam>
    /// <param name="name">The name of the layer.</param>
    /// <returns>The layer, or <see langword="null"/> if not found or not of the specified type.</returns>
    public T GetLayer<T>(string name) where T : TilemapLayer
    {
        if (_layersByName.TryGetValue(name, out TilemapLayer layer))
        {
            return layer as T;
        }

        return null;
    }

    /// <summary>
    /// Gets all layers of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of layers to retrieve.</typeparam>
    /// <returns>An enumerable of layers of the specified type.</returns>
    public IEnumerable<T> GetLayers<T>() where T : TilemapLayer
    {
        for (int i = 0; i < _layers.Count; i++)
        {
            if (_layers[i] is T typed)
            {
                yield return typed;
            }
        }
    }

    /// <summary>
    /// Determines the index of a specific layer in the collection.
    /// </summary>
    /// <param name="layer">The layer to locate in the collection.</param>
    /// <returns>The index of the layer if found in the collection; otherwise, -1.</returns>
    public int IndexOf(TilemapLayer layer)
    {
        return _layers.IndexOf(layer);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the layers in the collection.
    /// </summary>
    /// <returns>An enumerator for the layer collection.</returns>
    public IEnumerator<TilemapLayer> GetEnumerator()
    {
        return _layers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
