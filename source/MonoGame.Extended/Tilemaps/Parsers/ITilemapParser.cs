using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps.Parsers;

/// <summary>
/// Defines the interface for tilemap format parsers.
/// </summary>
public interface ITilemapParser
{
    /// <summary>
    /// Gets the file extensions supported by this parser (e.g., ".tmx", ".ldtk").
    /// </summary>
    IReadOnlyList<string> SupportedExtensions { get; }

    /// <summary>
    /// Parses a tilemap from the specified file path.
    /// </summary>
    /// <param name="path">The path to the tilemap file.</param>
    /// <param name="graphicsDevice">The graphics device used for loading textures.</param>
    /// <returns>The parsed tilemap.</returns>
    /// <exception cref="TilemapParseException">Thrown when the file cannot be parsed.</exception>
    Tilemap ParseFromFile(string path, GraphicsDevice graphicsDevice);

    /// <summary>
    /// Parses a tilemap from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the tilemap data.</param>
    /// <param name="graphicsDevice">The graphics device used for loading textures.</param>
    /// <param name="basePath">The base path for resolving relative file references.</param>
    /// <returns>The parsed tilemap.</returns>
    /// <exception cref="TilemapParseException">Thrown when the stream cannot be parsed.</exception>
    Tilemap ParseFromStream(Stream stream, GraphicsDevice graphicsDevice, string basePath);
}
