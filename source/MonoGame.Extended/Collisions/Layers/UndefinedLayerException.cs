namespace MonoGame.Extended.Collisions.Layers;

using System;

/// <summary>
/// Thrown when the collision system has no layer defined with the specified name
/// </summary>
public class UndefinedLayerException : Exception
{
    /// <summary>
    /// Thrown when the collision system has no layer defined with the specified name
    /// </summary>
    /// <param name="layerName">The undefined layer name</param>
    public UndefinedLayerException(string layerName)
        : base($"Layer with name '{layerName}' is undefined")
    {
    }
}
