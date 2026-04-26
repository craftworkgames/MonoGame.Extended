using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Collisions.QuadTree;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Handles basic collision between actors.
    /// When two actors collide, their OnCollision method is called.
    /// </summary>
    public class CollisionComponent : SimpleGameComponent
    {
        public const string DEFAULT_LAYER_NAME = "default";

        private readonly Dictionary<string, Layer> _layers = new();
        private readonly HashSet<LayerPair> _layerCollision = new();

        /// <summary>
        /// List of collision's layers
        /// </summary>
        public IReadOnlyDictionary<string, Layer> Layers => _layers;

        /// <summary>
        /// Creates component with default layer, which is a collision tree covering the specified area (using <see cref="QuadTree"/>.
        /// </summary>
        /// <param name="boundary">Boundary of the collision tree.</param>
        public CollisionComponent(RectangleF boundary)
        {
            SetDefaultLayer(new Layer(new QuadTreeSpace(new BoundingBox2D(boundary.TopLeft, boundary.BottomRight))));
        }

        /// <summary>
        /// Creates component with specifies default layer.
        /// If layer is null, method creates component without default layer.
        /// </summary>
        /// <param name="layer">Default layer</param>
        public CollisionComponent(Layer layer = null)
        {
            if (layer is not null)
                SetDefaultLayer(layer);
        }

        /// <summary>
        /// Sets the layer used for actors whose <see cref="ICollisionActor.LayerName"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="layer">Layer to set default</param>
        /// <remarks>
        /// The default layer always has the name <see cref="DEFAULT_LAYER_NAME"/>.
        /// When a default layer is set, collision is enabled between that layer and itself and between that layer and every currently registered layer.
        /// When additional non-default layers are later added, collision is enabled between the new layer and itself and between the new layer and the current default layer.
        /// No other cross-layer collision rules are created automatically.
        /// </remarks>
        /// <param name="layer">Layer to set default</param>
        public void SetDefaultLayer(Layer layer)
        {
            if (_layers.ContainsKey(DEFAULT_LAYER_NAME))
                Remove(DEFAULT_LAYER_NAME);

            Add(DEFAULT_LAYER_NAME, layer);

            foreach (Layer otherLayer in _layers.Values)
                EnableCollisionBetweenLayers(layer, otherLayer);
        }

        /// <summary>
        /// Update the collision tree and process collisions.
        /// </summary>
        /// <remarks>
        /// Boundary shapes are updated if they were changed since the last
        /// update.
        /// </remarks>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            foreach (Layer layer in _layers.Values)
                layer.Reset();

            foreach (LayerPair layerPair in _layerCollision)
            {
                Layer firstLayer = layerPair.First;
                Layer secondLayer = layerPair.Second;

                foreach (ICollisionActor actor in firstLayer.Space)
                {
                    IEnumerable<ICollisionActor> collisions = secondLayer.Space.Query(actor.Shape.BoundingBox);

                    foreach (ICollisionActor other in collisions)
                    {
                        if (actor == other)
                            continue;

                        if (!actor.Shape.TryGetCollision(other.Shape, out CollisionResult2D result))
                            continue;

                        actor.OnCollision(new CollisionEventArgs
                        {
                            Other = other,
                            PenetrationVector = -result.MinimumTranslationVector
                        });
                        other.OnCollision(new CollisionEventArgs
                        {
                            Other = actor,
                            PenetrationVector = result.MinimumTranslationVector
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Inserts the target into the collision tree.
        /// The target will have its OnCollision called when collisions occur.
        /// </summary>
        /// <param name="target">Target to insert.</param>
        public void Insert(ICollisionActor target)
        {
            string layerName = target.LayerName ?? DEFAULT_LAYER_NAME;
            if (!_layers.TryGetValue(layerName, out Layer layer))
                throw new UndefinedLayerException(layerName);

            layer.Space.Insert(target);
        }

        /// <summary>
        /// Removes the target from the collision tree.
        /// </summary>
        /// <param name="target">Target to remove.</param>
        public void Remove(ICollisionActor target)
        {
            if (target.LayerName is not null)
            {
                _layers[target.LayerName].Space.Remove(target);
                return;
            }

            foreach (Layer layer in _layers.Values)
            {
                if (layer.Space.Remove(target))
                    return;
            }
        }

        #region Layers

        /// <summary>
        /// Adds a named collision layer.
        /// </summary>
        /// <param name="name">Name of layer</param>
        /// <param name="layer">The new layer</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null</exception>
        /// <remarks>
        /// If <paramref name="name"/> is not <see cref="DEFAULT_LAYER_NAME"/>, collision is enabled between the new layer and itself.
        /// If a default layer is already present, collision is also enabled between the new layer and the default layer.
        /// No other cross-layer collision rules are created automatically.
        /// </remarks>
        public void Add(string name, Layer layer)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (!_layers.TryAdd(name, layer))
                throw new DuplicateNameException(name);

            if (name != DEFAULT_LAYER_NAME)
            {
                EnableCollisionBetweenLayers(layer, layer);

                if (_layers.TryGetValue(DEFAULT_LAYER_NAME, out Layer defaultLayer))
                {
                    EnableCollisionBetweenLayers(defaultLayer, layer);
                }
            }
        }

        /// <summary>
        /// Remove the layer and all layer's collisions.
        /// </summary>
        /// <param name="name">The name of the layer to delete</param>
        /// <param name="layer">The layer to delete</param>
        public void Remove(string name = null, Layer layer = null)
        {
            name ??= _layers.First(x => x.Value == layer).Key;
            _layers.Remove(name, out layer);
            _layerCollision.RemoveWhere(pair => ReferenceEquals(pair.First, layer) || ReferenceEquals(pair.Second, layer));
        }

        public void EnableCollisionBetweenLayers(Layer firstLayer, Layer secondLayer)
        {
            _layerCollision.Add(new LayerPair(firstLayer, secondLayer));
        }

        public void EnableCollisionBetweenLayers(string firstLayerName, string secondLayerName)
        {
            _layerCollision.Add(new LayerPair(_layers[firstLayerName], _layers[secondLayerName]));
        }

        public void DisableCollisionBetweenLayers(Layer firstLayer, Layer secondLayer)
        {
            _layerCollision.Remove(new LayerPair(firstLayer, secondLayer));
        }

        public void DisableCollisionBetweenLayers(string firstLayerName, string secondLayerName)
        {
            _layerCollision.Remove(new LayerPair(_layers[firstLayerName], _layers[secondLayerName]));
        }

        public bool IsCollisionEnabledBetweenLayers(Layer firstLayer, Layer secondLayer)
        {
            return _layerCollision.Contains(new LayerPair(firstLayer, secondLayer));
        }

        public bool IsCollisionEnabledBetweenLayers(string firstLayerName, string secondLayerName)
        {
            return _layerCollision.Contains(new LayerPair(_layers[firstLayerName], _layers[secondLayerName]));
        }

        public void EnableSelfCollision(Layer layer)
        {
            EnableCollisionBetweenLayers(layer, layer);
        }

        public void EnableSelfCollision(string layerName)
        {
            EnableCollisionBetweenLayers(layerName, layerName);
        }

        public void DisableSelfCollision(Layer layer)
        {
            DisableCollisionBetweenLayers(layer, layer);
        }

        public void DisableSelfCollision(string layerName)
        {
            DisableCollisionBetweenLayers(layerName, layerName);
        }

        public bool IsSelfCollisionEnabled(Layer layer)
        {
            return IsCollisionEnabledBetweenLayers(layer, layer);
        }

        public bool IsSelfCollisionEnabled(string layerName)
        {
            return IsCollisionEnabledBetweenLayers(layerName, layerName);
        }

        #endregion
    }
}
