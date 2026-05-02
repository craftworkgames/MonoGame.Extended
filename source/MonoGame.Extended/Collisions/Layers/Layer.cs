using System;

namespace MonoGame.Extended.Collisions.Layers;

/// <summary>
/// Groups collision actors that share one broadphase structure and one set of layer-level rules.
/// </summary>
public class Layer
{
    /// <summary>
    /// Gets the broadphase structure that stores the layer's actors.
    /// </summary>
    public readonly ICollisionBroadphase2D Space;

    /// <summary>
    /// Gets or sets a value indicating whether the broadphase is rebuilt during each reset.
    /// </summary>
    public bool IsDynamic { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Layer"/> class.
    /// </summary>
    /// <param name="space">The broadphase structure used to store and query actors in this layer.</param>
    /// <exception cref="ArgumentNullException"><paramref name="space"/> is <see langword="null"/>.</exception>
    public Layer(ICollisionBroadphase2D space)
    {
        ArgumentNullException.ThrowIfNull(space);

        Space = space;
    }

    /// <summary>
    /// Rebuilds the layer broadphase when the layer is dynamic.
    /// </summary>
    public virtual void Reset()
    {
        if (IsDynamic)
        {
            Space.Reset();
        }
    }
}
