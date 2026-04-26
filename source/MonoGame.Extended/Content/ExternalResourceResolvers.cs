using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Content;

/// <summary>
/// Provides common external resource resolver implementations.
/// </summary>
public static class ExternalResourceResolvers
{
    /// <summary>
    /// Opens a resource from the local file system.
    /// </summary>
    /// <param name="path">The absolute or relative file path to open.</param>
    /// <returns>A readable stream for the file.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    public static Stream OpenFile(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return File.OpenRead(path);
    }

    /// <summary>
    /// Opens a resource by using <see cref="TitleContainer"/>.
    /// </summary>
    /// <param name="path">The title-relative resource path to open.</param>
    /// <returns>A readable stream for the resource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <see langword="null"/>.</exception>
    public static Stream OpenTitleContainerStream(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return TitleContainer.OpenStream(path);
    }
}
