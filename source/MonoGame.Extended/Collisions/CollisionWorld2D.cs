using System;
using System.Collections.Generic;
using System.Data;
using MonoGame.Extended.Collisions.Layers;

namespace MonoGame.Extended.Collisions;

/// <summary>
/// Stores collision actors in named layers and tracks which layer currently owns each actor.
/// </summary>
public class CollisionWorld2D
{
    /// <summary>
    /// The name of the default collision layer.
    /// </summary>
    public const string DefaultLayerName = "default";

    private readonly Dictionary<string, Layer> _layers = new();
    private readonly Dictionary<ICollisionActor, string> _actorLayerNames = new();
    private readonly HashSet<LayerPair> _layerCollision = new();

    /// <summary>
    /// Gets the registered collision layers by name.
    /// </summary>
    public IReadOnlyDictionary<string, Layer> Layers => _layers;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollisionWorld2D"/> class.
    /// </summary>
    public CollisionWorld2D()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollisionWorld2D"/> class with a default layer.
    /// </summary>
    /// <param name="defaultLayer">The layer used for actors inserted into <see cref="DefaultLayerName"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="defaultLayer"/> is <see langword="null"/>.</exception>
    public CollisionWorld2D(Layer defaultLayer)
    {
        ArgumentNullException.ThrowIfNull(defaultLayer);
        SetDefaultLayer(defaultLayer);
    }

    /// <summary>
    /// Sets the default collision layer.
    /// </summary>
    /// <param name="layer">The layer used for actors inserted into <see cref="DefaultLayerName"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="layer"/> is <see langword="null"/>.</exception>
    public void SetDefaultLayer(Layer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        if (_layers.ContainsKey(DefaultLayerName))
        {
            RemoveLayer(DefaultLayerName);
        }

        _layers[DefaultLayerName] = layer;

        foreach (Layer otherLayer in _layers.Values)
        {
            EnableCollisionBetweenLayers(layer, otherLayer);
        }
    }

    /// <summary>
    /// Adds a named collision layer.
    /// </summary>
    /// <param name="name">The unique layer name.</param>
    /// <param name="layer">The layer to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="layer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is null, empty, or whitespace.</exception>
    /// <exception cref="DuplicateNameException">A layer with the same name already exists.</exception>
    public void AddLayer(string name, Layer layer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(layer);

        if (!_layers.TryAdd(name, layer))
        {
            throw new DuplicateNameException(name);
        }

        if (name != DefaultLayerName)
        {
            EnableCollisionBetweenLayers(layer, layer);

            if (_layers.TryGetValue(DefaultLayerName, out Layer defaultLayer))
            {
                EnableCollisionBetweenLayers(defaultLayer, layer);
            }
        }
    }

    /// <summary>
    /// Removes a named collision layer.
    /// </summary>
    /// <param name="name">The layer name to remove.</param>
    /// <returns><see langword="true"/> if the layer was removed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is null, empty, or whitespace.</exception>
    public bool RemoveLayer(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_layers.Remove(name, out Layer layer))
        {
            return false;
        }

        List<ICollisionActor> actorsToRemove = new();

        foreach (KeyValuePair<ICollisionActor, string> pair in _actorLayerNames)
        {
            if (pair.Value == name)
            {
                actorsToRemove.Add(pair.Key);
            }
        }

        foreach (ICollisionActor actor in actorsToRemove)
        {
            _actorLayerNames.Remove(actor);
        }

        _layerCollision.RemoveWhere(pair => ReferenceEquals(pair.First, layer) || ReferenceEquals(pair.Second, layer));
        return true;
    }

    /// <summary>
    /// Inserts an actor into the default collision layer.
    /// </summary>
    /// <param name="actor">The actor to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="actor"/> is already present in this collision world.</exception>
    /// <exception cref="UndefinedLayerException"><see cref="DefaultLayerName"/> is not registered.</exception>
    public void Insert(ICollisionActor actor)
    {
        Insert(actor, DefaultLayerName);
    }

    /// <summary>
    /// Inserts an actor into the specified collision layer.
    /// </summary>
    /// <param name="actor">The actor to insert.</param>
    /// <param name="layerName">The name of the registered layer that will contain the actor.</param>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="layerName"/> is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="actor"/> is already present in this collision world.</exception>
    /// <exception cref="UndefinedLayerException"><paramref name="layerName"/> is not registered.</exception>
    public void Insert(ICollisionActor actor, string layerName)
    {
        ArgumentNullException.ThrowIfNull(actor);
        ArgumentException.ThrowIfNullOrWhiteSpace(layerName);

        if (_actorLayerNames.ContainsKey(actor))
        {
            throw new InvalidOperationException("The actor is already present in this collision world.");
        }

        Layer layer = GetLayer(layerName);
        layer.Space.Insert(actor);
        _actorLayerNames.Add(actor, layerName);
    }

    /// <summary>
    /// Returns whether the specified actor is present in this collision world.
    /// </summary>
    /// <param name="actor">The actor to look up.</param>
    /// <returns><see langword="true"/> if the actor is present in this collision world; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    public bool Contains(ICollisionActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);
        return _actorLayerNames.ContainsKey(actor);
    }

    /// <summary>
    /// Tries to get the name of the layer that currently contains the specified actor.
    /// </summary>
    /// <param name="actor">The actor to look up.</param>
    /// <param name="layerName">When this method returns, contains the actor's current layer name if the actor is present; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the actor is present in this collision world; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    public bool TryGetLayerName(ICollisionActor actor, out string layerName)
    {
        ArgumentNullException.ThrowIfNull(actor);
        return _actorLayerNames.TryGetValue(actor, out layerName);
    }

    /// <summary>
    /// Gets the name of the layer that currently contains the specified actor.
    /// </summary>
    /// <param name="actor">The actor to look up.</param>
    /// <returns>The name of the layer that currently contains <paramref name="actor"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="actor"/> is not present in this collision world.</exception>
    public string GetLayerName(ICollisionActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        if (!_actorLayerNames.TryGetValue(actor, out string layerName))
        {
            throw new InvalidOperationException("The actor is not present in this collision world.");
        }

        return layerName;
    }

    /// <summary>
    /// Moves an actor from its current collision layer into another registered layer in this world.
    /// </summary>
    /// <param name="actor">The actor to move.</param>
    /// <param name="layerName">The name of the registered layer that will contain the actor.</param>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="layerName"/> is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="actor"/> is not present in this collision world.</exception>
    /// <exception cref="UndefinedLayerException"><paramref name="layerName"/> is not registered.</exception>
    public void MoveToLayer(ICollisionActor actor, string layerName)
    {
        ArgumentNullException.ThrowIfNull(actor);
        ArgumentException.ThrowIfNullOrWhiteSpace(layerName);

        string currentLayerName = GetLayerName(actor);

        if (currentLayerName == layerName)
        {
            return;
        }

        Layer currentLayer = GetLayer(currentLayerName);
        Layer targetLayer = GetLayer(layerName);
        currentLayer.Space.Remove(actor);
        targetLayer.Space.Insert(actor);
        _actorLayerNames[actor] = layerName;
    }

    /// <summary>
    /// Removes an actor from its assigned collision layer.
    /// </summary>
    /// <param name="actor">The actor to remove.</param>
    /// <returns><see langword="true"/> if the actor was removed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
    public bool Remove(ICollisionActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        if (!_actorLayerNames.TryGetValue(actor, out string layerName))
        {
            return false;
        }

        Layer layer = GetLayer(layerName);
        _actorLayerNames.Remove(actor);
        return layer.Space.Remove(actor);
    }

    /// <summary>
    /// Rebuilds every registered layer.
    /// </summary>
    public void RebuildDynamicLayers()
    {
        foreach (Layer layer in _layers.Values)
        {
            layer.Reset();
        }
    }

    /// <summary>
    /// Enables collision between two layers.
    /// </summary>
    /// <param name="firstLayerName">The first layer name.</param>
    /// <param name="secondLayerName">The second layer name.</param>
    /// <exception cref="UndefinedLayerException">Either layer is not registered.</exception>
    public void EnableCollisionBetweenLayers(string firstLayerName, string secondLayerName)
    {
        EnableCollisionBetweenLayers(GetLayer(firstLayerName), GetLayer(secondLayerName));
    }

    /// <summary>
    /// Disables collision between two layers.
    /// </summary>
    /// <param name="firstLayerName">The first layer name.</param>
    /// <param name="secondLayerName">The second layer name.</param>
    /// <exception cref="UndefinedLayerException">Either layer is not registered.</exception>
    public void DisableCollisionBetweenLayers(string firstLayerName, string secondLayerName)
    {
        _layerCollision.Remove(new LayerPair(GetLayer(firstLayerName), GetLayer(secondLayerName)));
    }

    /// <summary>
    /// Returns whether collision is enabled between two layers.
    /// </summary>
    /// <param name="firstLayerName">The first layer name.</param>
    /// <param name="secondLayerName">The second layer name.</param>
    /// <returns><see langword="true"/> if collision is enabled between the two layers; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="UndefinedLayerException">Either layer is not registered.</exception>
    public bool IsCollisionEnabledBetweenLayers(string firstLayerName, string secondLayerName)
    {
        return _layerCollision.Contains(new LayerPair(GetLayer(firstLayerName), GetLayer(secondLayerName)));
    }

    private void EnableCollisionBetweenLayers(Layer firstLayer, Layer secondLayer)
    {
        _layerCollision.Add(new LayerPair(firstLayer, secondLayer));
    }

    private Layer GetLayer(string layerName)
    {
        string resolvedLayerName = layerName ?? DefaultLayerName;

        if (!_layers.TryGetValue(resolvedLayerName, out Layer layer))
        {
            throw new UndefinedLayerException(resolvedLayerName);
        }

        return layer;
    }
}
