using System;

namespace MonoGame.Extended.Collisions.Layers;

/// <summary>
/// Layer is a group of collision's actors.
/// </summary>
public class Layer
{
    /// <summary>
    /// If this property equals true, layer always will reset collision space.
    /// </summary>
    public bool IsDynamic { get; set; } = true;


    /// <summary>
    /// The space, which contain actors.
    /// </summary>
    public readonly ISpaceAlgorithm Space;

    /// <summary>
    /// Constructor for layer
    /// </summary>
    /// <param name="spaceAlgorithm">A space algorithm for actors</param>
    /// <exception cref="ArgumentNullException"><paramref name="spaceAlgorithm"/> is null</exception>
    public Layer(ISpaceAlgorithm spaceAlgorithm)
    {
        Space = spaceAlgorithm ?? throw new ArgumentNullException(nameof(spaceAlgorithm));
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
