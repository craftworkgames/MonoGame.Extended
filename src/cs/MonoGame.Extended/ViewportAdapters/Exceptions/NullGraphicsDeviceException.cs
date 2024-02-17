namespace MonoGame.Extended.ViewportAdapters.Exceptions;

using System;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Thrown when the <see cref="GraphicsDevice"/> is null
/// </summary>
public class NullGraphicsDeviceException : Exception
{
    /// <summary>
    /// Thrown when the <see cref="GraphicsDevice"/> is null
    /// </summary>
    /// <param name="message">Exception message</param>
    public NullGraphicsDeviceException(string message) : base(message)
    {
    }
}
