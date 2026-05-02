using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MonoGame.Extended.Collisions.Layers;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Stores collision actors in named layers and provides broadphase, narrowphase, and pair-query operations.
    /// </summary>
    /// <remarks>
    /// This is the primary query-oriented actor/world collision API.
    /// Low-level collision math remains in <see cref="Collision2D"/> and the bounding volume types.
    /// Layer filtering is explicit: collisions are considered only for layer pairs that have been enabled.
    /// </remarks>
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
        public CollisionWorld2D() { }

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
        /// <remarks>
        /// The default layer always has the name <see cref="DefaultLayerName"/>.
        /// When a default layer is set, self-collision is enabled for that layer and cross-layer collision is enabled
        /// between the default layer and every currently registered layer.
        /// Adding later non-default layers enables collision between those layers and the current default layer.
        /// </remarks>
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
        /// <remarks>
        /// New non-default layers automatically enable self-collision.
        /// If the default layer already exists, collision is also enabled between the new layer and the default layer.
        /// No other cross-layer rules are added automatically.
        /// </remarks>
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
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Layer name cannot be null or whitespace.", nameof(name));
            }

            if (!_layers.Remove(name, out Layer layer))
            {
                return false;
            }

            List<ICollisionActor> actorsToRemove = new List<ICollisionActor>();

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

            if (string.IsNullOrWhiteSpace(layerName))
            {
                throw new ArgumentException("Layer name cannot be null or whitespace.", nameof(layerName));
            }

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
        /// Rebuilds every dynamic layer in this collision world.
        /// </summary>
        /// <remarks>
        /// This is a convenience wrapper over <see cref="Layer.Reset"/> for each registered layer.
        /// <see cref="CollisionWorld2D"/> remains query-oriented and does not own frame progression or actor updates.
        /// Call this after you finish moving actors and updating their shapes for the current step.
        /// </remarks>
        public void RebuildDynamicLayers()
        {
            foreach (Layer layer in _layers.Values)
            {
                layer.Reset();
            }
        }

        /// <summary>
        /// Queries one collision layer for broadphase candidates whose bounds overlap the specified area.
        /// </summary>
        /// <param name="bounds">The axis-aligned query bounds in world space.</param>
        /// <param name="layerName">The layer name to query, or <see langword="null"/> to query the default layer.</param>
        /// <returns>The actors whose broadphase bounds overlap <paramref name="bounds"/> in the resolved layer.</returns>
        /// <exception cref="UndefinedLayerException">The resolved layer is not registered.</exception>
        public IEnumerable<ICollisionActor> QueryCandidates(BoundingBox2D bounds, string layerName = null)
        {
            Layer layer = GetLayer(layerName);
            return layer.Space.Query(bounds);
        }

        /// <summary>
        /// Queries a target layer for broadphase candidates for the specified actor after applying layer filtering.
        /// </summary>
        /// <param name="actor">The actor whose broadphase bounds are used for the query.</param>
        /// <param name="otherLayerName">The target layer name, or <see langword="null"/> to query the default layer.</param>
        /// <returns>
        /// The actors whose broadphase bounds overlap <paramref name="actor"/> in the target layer when collision is enabled between the two layers;
        /// otherwise, an empty sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="actor"/> is not present in this collision world.</exception>
        /// <exception cref="UndefinedLayerException">The actor layer or target layer is not registered.</exception>
        public IEnumerable<ICollisionActor> QueryCandidates(ICollisionActor actor, string otherLayerName)
        {
            ArgumentNullException.ThrowIfNull(actor);

            Layer actorLayer = GetActorLayer(actor);
            Layer otherLayer = GetLayer(otherLayerName);

            if (!_layerCollision.Contains(new LayerPair(actorLayer, otherLayer)))
            {
                return Enumerable.Empty<ICollisionActor>();
            }

            return otherLayer.Space.Query(actor.Shape.BoundingBox);
        }

        /// <summary>
        /// Queries one collision layer for actors that collide with the specified actor and returns collision results
        /// relative to that actor.
        /// </summary>
        /// <param name="actor">The actor to test against the target layer.</param>
        /// <param name="otherLayerName">The target layer name, or <see langword="null"/> to query the default layer.</param>
        /// <returns>
        /// The collision results for actors in the target layer whose shapes overlap <paramref name="actor"/> after
        /// layer filtering and narrowphase checks.
        /// </returns>
        /// <remarks>
        /// Each returned <see cref="CollisionEvent2D.Result"/> follows the receiver-relative direction convention.
        /// Its minimum translation vector moves <paramref name="actor"/> out of the returned <see cref="CollisionEvent2D.Other"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="actor"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="actor"/> is not present in this collision world.</exception>
        /// <exception cref="UndefinedLayerException">The actor layer or target layer is not registered.</exception>
        public IEnumerable<CollisionEvent2D> QueryCollisions(ICollisionActor actor, string otherLayerName)
        {
            ArgumentNullException.ThrowIfNull(actor);

            foreach (ICollisionActor candidate in QueryCandidates(actor, otherLayerName))
            {
                if (ReferenceEquals(actor, candidate))
                {
                    continue;
                }

                if (!actor.Shape.TryGetCollision(candidate.Shape, out CollisionResult2D result))
                {
                    continue;
                }

                yield return new CollisionEvent2D
                {
                    Other = candidate,
                    Result = result
                };
            }
        }

        /// <summary>
        /// Queries two layers for colliding actor pairs and returns collision results when they are needed.
        /// </summary>
        /// <param name="firstLayerName">The first layer name, or <see langword="null"/> to use the default layer.</param>
        /// <param name="secondLayerName">The second layer name, or <see langword="null"/> to use the default layer.</param>
        /// <returns>
        /// The colliding actor pairs between the two resolved layers. Each returned result contains the collision result
        /// relative to the first actor in the pair.
        /// </returns>
        /// <remarks>
        /// Each unordered actor pair is returned at most once per query, even when both actors are in the same layer
        /// or when broadphase storage places them in multiple candidate buckets.
        /// <see cref="CollisionPair2D.FirstResult"/> moves <see cref="CollisionPair2D.First"/> out of
        /// <see cref="CollisionPair2D.Second"/>.
        /// </remarks>
        /// <exception cref="UndefinedLayerException">Either resolved layer is not registered.</exception>
        public IEnumerable<CollisionPair2D> QueryCollisionPairs(string firstLayerName, string secondLayerName)
        {
            Layer firstLayer = GetLayer(firstLayerName);
            Layer secondLayer = GetLayer(secondLayerName);
            HashSet<ActorPairKey> processedPairs = new HashSet<ActorPairKey>();

            if (!_layerCollision.Contains(new LayerPair(firstLayer, secondLayer)))
            {
                yield break;
            }

            foreach (ICollisionActor actor in firstLayer.Space)
            {
                foreach (ICollisionActor candidate in secondLayer.Space.Query(actor.Shape.BoundingBox))
                {
                    if (ReferenceEquals(actor, candidate))
                    {
                        continue;
                    }

                    if (!actor.Shape.TryGetCollision(candidate.Shape, out CollisionResult2D result))
                    {
                        continue;
                    }

                    ActorPairKey actorPair = new ActorPairKey(actor, candidate);

                    if (!processedPairs.Add(actorPair))
                    {
                        continue;
                    }

                    yield return new CollisionPair2D
                    {
                        First = actor,
                        Second = candidate,
                        FirstResult = result
                    };
                }
            }
        }

        /// <summary>
        /// Enables collision between two layers.
        /// </summary>
        /// <param name="firstLayerName">The first layer name.</param>
        /// <param name="secondLayerName">The second layer name.</param>
        /// <remarks>
        /// Layer ordering does not affect the stored rule.
        /// Enabling collision between two layers also enables queries in the reversed order.
        /// </remarks>
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
        /// <remarks>
        /// Layer ordering does not affect the stored rule.
        /// Disabling collision between two layers also disables queries in the reversed order.
        /// </remarks>
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
        /// <returns><see langword="true"/> when collision is enabled between the two layers; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="UndefinedLayerException">Either layer is not registered.</exception>
        public bool IsCollisionEnabledBetweenLayers(string firstLayerName, string secondLayerName)
        {
            return _layerCollision.Contains(new LayerPair(GetLayer(firstLayerName), GetLayer(secondLayerName)));
        }

        private void EnableCollisionBetweenLayers(Layer firstLayer, Layer secondLayer)
        {
            _layerCollision.Add(new LayerPair(firstLayer, secondLayer));
        }

        private Layer GetActorLayer(ICollisionActor actor)
        {
            if (!_actorLayerNames.TryGetValue(actor, out string layerName))
            {
                throw new InvalidOperationException("The actor is not present in this collision world.");
            }

            return GetLayer(layerName);
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
}
