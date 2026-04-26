using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Tilemaps.Ogmo.Converters;
using MonoGame.Extended.Tilemaps.Ogmo.Document;
using MonoGame.Extended.Tilemaps.Parsers;

namespace MonoGame.Extended.Tilemaps.Ogmo;

/// <summary>
/// Parser for Ogmo Editor 3 JSON tilemap files.
/// </summary>
public sealed class OgmoJsonParser : ITilemapParser
{
    private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly string _projectPath;
    private readonly string _baseDirectory;
    private readonly ExternalResourceResolver _resourceResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="OgmoJsonParser"/> class.
    /// </summary>
    /// <param name="projectPath">The path to the Ogmo project file (.ogmo). This parameter is required.</param>
    /// <param name="baseDirectory">
    /// Optional base directory for resolving relative level file paths. If not provided,
    /// the project file's directory will be used.
    /// </param>
    /// <param name="resourceResolver">
    /// Optional resolver used to open external resources referenced by the project. If
    /// <see langword="null"/>, resources are opened from the local file system.
    /// </param>
    public OgmoJsonParser(string projectPath, string baseDirectory = null, ExternalResourceResolver resourceResolver = null)
    {
        ArgumentNullException.ThrowIfNull(projectPath);

        _projectPath = projectPath;
        _baseDirectory = baseDirectory ?? GetDirectoryOrCurrent(projectPath);
        _resourceResolver = resourceResolver ?? ExternalResourceResolvers.OpenFile;
    }

    /// <summary>
    /// Gets the file extensions supported by this parser.
    /// </summary>
    public IReadOnlyList<string> SupportedExtensions { get; } = new[] { ".json" };

    /// <summary>
    /// Determines whether this parser can parse the specified file.
    /// </summary>
    /// <param name="filePath">The path to the file to check.</param>
    /// <returns><see langword="true"/> if the file can be parsed; otherwise, <see langword="false"/>.</returns>
    public bool CanParse(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        string extension = Path.GetExtension(filePath);
        return SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Parses a tilemap from the specified Ogmo level file.
    /// </summary>
    /// <param name="filePath">
    /// The path to the Ogmo level file (.json). Can be absolute or relative to the base directory
    /// specified in the constructor.
    /// </param>
    /// <param name="graphicsDevice">The graphics device used for loading textures.</param>
    /// <returns>A <see cref="Tilemap"/> representing the level.</returns>
    /// <exception cref="FileNotFoundException">The specified level file does not exist.</exception>
    /// <exception cref="TilemapParseException">An error occurred while parsing the file.</exception>
    public Tilemap ParseFromFile(string filePath, GraphicsDevice graphicsDevice)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        string fullPath = Path.Combine(_baseDirectory, filePath);

        try
        {
            OgmoProject project = LoadProject(_projectPath);
            OgmoLevel level = LoadLevel(fullPath);

            string projectDirectory = GetDirectoryOrCurrent(_projectPath);

            return ConvertLevel(level, project, graphicsDevice, projectDirectory);
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException($"Failed to parse Ogmo level file: {fullPath}", ex);
        }
    }

    /// <summary>
    /// Parses a tilemap from the specified stream.
    /// </summary>
    /// <param name="stream">The stream containing the Ogmo level data.</param>
    /// <param name="graphicsDevice">The graphics device used for loading textures.</param>
    /// <param name="basePath">
    /// Optional base path for resolving relative file references. If not provided,
    /// uses the project file's directory.
    /// </param>
    /// <returns>A <see cref="Tilemap"/> representing the level.</returns>
    /// <exception cref="TilemapParseException">An error occurred while parsing the stream.</exception>
    public Tilemap ParseFromStream(Stream stream, GraphicsDevice graphicsDevice, string basePath = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        try
        {
            OgmoLevel level = JsonSerializer.Deserialize<OgmoLevel>(stream, s_jsonOptions);

            if (level == null)
            {
                throw new TilemapParseException("Failed to deserialize Ogmo level: result was null.");
            }

            OgmoProject project = LoadProject(_projectPath);

            string resolvedBasePath = basePath ?? GetDirectoryOrCurrent(_projectPath);

            return ConvertLevel(level, project, graphicsDevice, resolvedBasePath);
        }
        catch (JsonException ex)
        {
            throw new TilemapParseException("Failed to parse Ogmo JSON data.", ex);
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException("An unexpected error occurred while parsing Ogmo data.", ex);
        }
    }

    private OgmoProject LoadProject(string projectPath)
    {
        try
        {
            using Stream stream = OpenProjectStream(projectPath);
            OgmoProject project = JsonSerializer.Deserialize<OgmoProject>(stream, s_jsonOptions);

            if (project == null)
            {
                throw new TilemapParseException($"Failed to deserialize Ogmo project from file: {projectPath}");
            }

            return project;
        }
        catch (JsonException ex)
        {
            throw new TilemapParseException($"Failed to parse Ogmo project file: {projectPath}", ex);
        }
    }

    private OgmoLevel LoadLevel(string levelPath)
    {
        using Stream stream = OpenLevelStream(levelPath);
        OgmoLevel level = JsonSerializer.Deserialize<OgmoLevel>(stream, s_jsonOptions);

        if (level == null)
        {
            throw new TilemapParseException($"Failed to deserialize Ogmo level from file: {levelPath}");
        }

        return level;
    }

    private Tilemap ConvertLevel(OgmoLevel level, OgmoProject project, GraphicsDevice graphicsDevice, string baseDirectory)
    {
        TilemapData data = OgmoTilemapDataConverter.Convert(level, project, baseDirectory, _resourceResolver);
        return TilemapFactory.Build(data, graphicsDevice, baseDirectory, _resourceResolver);
    }

    private Stream OpenProjectStream(string projectPath)
    {
        try
        {
            return _resourceResolver(projectPath);
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException($"Failed to open Ogmo project file: {projectPath}", ex);
        }
    }

    private Stream OpenLevelStream(string levelPath)
    {
        try
        {
            return _resourceResolver(levelPath);
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException($"Failed to open Ogmo level file: {levelPath}", ex);
        }
    }

    private static string GetDirectoryOrCurrent(string path)
    {
        string directory = Path.GetDirectoryName(path);

        if (string.IsNullOrEmpty(directory))
        {
            return Directory.GetCurrentDirectory();
        }

        return directory;
    }
}
