using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a layer that contains other layers as children.
/// </summary>
public class TilemapGroupLayer : TilemapLayer
{
    private readonly List<TilemapLayer> _childLayers = new List<TilemapLayer>();

    /// <summary>
    /// Gets the collection of child layers.
    /// </summary>
    public IReadOnlyList<TilemapLayer> ChildLayers
    {
        get
        {
            return _childLayers;
        }
    }

    /// <inheritdoc/>
    public override Rectangle Bounds
    {
        get
        {
            if (_childLayers.Count == 0)
            {
                return Rectangle.Empty;
            }

            Rectangle bounds = _childLayers[0].Bounds;

            for (int i = 1; i < _childLayers.Count; i++)
            {
                bounds = Rectangle.Union(bounds, _childLayers[i].Bounds);
            }

            return bounds;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapGroupLayer"/> class.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    public TilemapGroupLayer(string name) : base(name)
    {
    }

    /// <summary>
    /// Adds a child layer to this group.
    /// </summary>
    /// <param name="layer">The layer to add.</param>
    public void AddLayer(TilemapLayer layer)
    {
        _childLayers.Add(layer);
    }

    /// <summary>
    /// Removes a child layer from this group.
    /// </summary>
    /// <param name="layer">The layer to remove.</param>
    /// <returns><see langword="true"/> if the layer was removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveLayer(TilemapLayer layer)
    {
        return _childLayers.Remove(layer);
    }

    /// <summary>
    /// Gets the child layer with the specified name.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    /// <returns>The layer with the specified name, or <see langword="null"/> if not found.</returns>
    /// <remarks>
    /// This method searches recursively through all child layers and their descendants.
    /// </remarks>
    public TilemapLayer GetLayer(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        foreach (TilemapLayer layer in _childLayers)
        {
            if (layer.Name == name)
            {
                return layer;
            }
        }

        foreach (TilemapLayer layer in _childLayers)
        {
            if (layer is TilemapGroupLayer groupLayer)
            {
                TilemapLayer found = groupLayer.GetLayer(name);

                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }
}
