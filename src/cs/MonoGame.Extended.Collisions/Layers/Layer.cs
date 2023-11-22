using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Collisions.QuadTree;

namespace MonoGame.Extended.Collisions.Layers;

/// <summary>
///
/// </summary>
public class Layer
{
    /// <summary>
    /// Name of layer
    /// </summary>
    public string Name { get;  }

    public bool IsDynamic { get; set; } = true;

    public ISpaceAlgorithm Space { get;  }

    public Layer(string name, ISpaceAlgorithm spaceAlgorithm)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if (spaceAlgorithm is null)
            throw new ArgumentNullException(nameof(spaceAlgorithm));

        Name = name;
        Space = spaceAlgorithm;
    }

    /// <summary>
    /// Restructure a inner collection, if layer is dynamic, because actors can change own position
    /// </summary>
    public virtual void Reset()
    {
        if (IsDynamic)
            Space.Reset();
    }
}
