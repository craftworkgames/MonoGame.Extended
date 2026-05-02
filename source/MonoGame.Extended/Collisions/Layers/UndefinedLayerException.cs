using System;

namespace MonoGame.Extended.Collisions.Layers;

/// <summary>
/// The exception that is thrown when a collision layer name is not registered in a collision world.
/// </summary>
public class UndefinedLayerException : Exception
{
    /// <summary>
    /// Gets the layer name that could not be resolved.
    /// </summary>
    public string LayerName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UndefinedLayerException"/> class.
    /// </summary>
    /// <param name="layerName">The layer name that could not be resolved.</param>
    /// <exception cref="ArgumentException"><paramref name="layerName"/> is null, empty, or whitespace.</exception>
    public UndefinedLayerException(string layerName)
        : base(CreateMessage(layerName))
    {
        LayerName = layerName;
    }

    private static string CreateMessage(string layerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(layerName);

        return $"Layer '{layerName}' is not defined.";
    }
}
