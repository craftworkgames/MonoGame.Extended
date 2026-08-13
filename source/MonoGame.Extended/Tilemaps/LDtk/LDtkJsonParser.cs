using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Tilemaps.LDtk.Converters;
using MonoGame.Extended.Tilemaps.LDtk.Document;
using MonoGame.Extended.Tilemaps.Parsers;

namespace MonoGame.Extended.Tilemaps.LDtk;

/// <summary>
/// Parser for LDtk JSON tilemap files.
/// </summary>
public class LDtkJsonParser : ITilemapParser
{
    private readonly string _baseDirectory;
    private readonly ExternalResourceResolver _resourceResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="LDtkJsonParser"/> class.
    /// </summary>
    /// <param name="baseDirectory">
    /// Optional base directory for resolving relative file paths. If provided, file paths in
    /// <see cref="ParseFromFile"/> will be resolved relative to this directory.
    /// If <see langword="null"/>, paths are resolved from the file's own location.
    /// </param>
    /// <param name="resourceResolver">
    /// Optional resolver used to open external resources referenced by the project. If
    /// <see langword="null"/>, resources are opened from the local file system.
    /// </param>
    public LDtkJsonParser(string baseDirectory = null, ExternalResourceResolver resourceResolver = null)
    {
        _baseDirectory = baseDirectory;
        _resourceResolver = resourceResolver ?? ExternalResourceResolvers.OpenFile;
    }

    /// <summary>
    /// Gets the file extensions supported by this parser.
    /// </summary>
    public IReadOnlyList<string> SupportedExtensions { get; } = new[] { ".ldtk" };

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
    /// Parses a tilemap from the specified file.
    /// </summary>
    /// <param name="filePath">The path to the LDtk project file (.ldtk).</param>
    /// <param name="graphicsDevice">The graphics device used for loading textures.</param>
    /// <returns>A <see cref="Tilemap"/> representing the first level in the project.</returns>
    /// <exception cref="FileNotFoundException">The specified file does not exist.</exception>
    /// <exception cref="TilemapParseException">An error occurred while parsing the file.</exception>
    public Tilemap ParseFromFile(string filePath, GraphicsDevice graphicsDevice)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        string fullPath = _baseDirectory != null
            ? Path.Combine(_baseDirectory, filePath)
            : filePath;

        try
        {
            using Stream stream = OpenProjectStream(fullPath);
            string directory = Path.GetDirectoryName(fullPath);
            return ParseFromStream(stream, graphicsDevice, directory);
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
            throw new TilemapParseException($"Failed to parse LDtk file: {fullPath}", ex);
        }
    }

    /// <summary>
    /// Parses a tilemap from the specified stream.
    /// </summary>
    /// <param name="stream">The stream containing the LDtk project data.</param>
    /// <param name="graphicsDevice">The graphics device used for loading textures.</param>
    /// <param name="basePath">
    /// Optional base path for resolving relative file references. If not provided,
    /// uses the base directory from the constructor, or the current directory if neither is set.
    /// </param>
    /// <returns>A <see cref="Tilemap"/> representing the first level in the project.</returns>
    /// <exception cref="TilemapParseException">An error occurred while parsing the stream.</exception>
    public Tilemap ParseFromStream(Stream stream, GraphicsDevice graphicsDevice, string basePath = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        try
        {
            LDtkProject project = JsonSerializer.Deserialize(stream, LDtkJsonSerializerContext.Default.LDtkProject);

            if (project == null)
            {
                throw new TilemapParseException("Failed to deserialize LDtk project: result was null.");
            }

            if (project.Levels == null || project.Levels.Count == 0)
            {
                throw new TilemapParseException("LDtk project contains no levels.");
            }

            string resolvedBasePath = basePath ?? _baseDirectory ?? Directory.GetCurrentDirectory();

            LDtkLevel firstLevel = project.Levels[0];
            return ConvertLevel(firstLevel, project, graphicsDevice, resolvedBasePath);
        }
        catch (JsonException ex)
        {
            throw new TilemapParseException("Failed to parse LDtk JSON data.", ex);
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException("An unexpected error occurred while parsing LDtk data.", ex);
        }
    }

    /// <summary>
    /// Parses a specific level from an LDtk project file.
    /// </summary>
    /// <param name="filePath">The path to the LDtk project file (.ldtk).</param>
    /// <param name="levelIdentifier">The identifier of the level to parse.</param>
    /// <param name="graphicsDevice">The graphics device used for loading textures.</param>
    /// <returns>A <see cref="Tilemap"/> representing the specified level.</returns>
    /// <exception cref="ArgumentException">The specified level was not found in the project.</exception>
    /// <exception cref="FileNotFoundException">The specified file does not exist.</exception>
    /// <exception cref="TilemapParseException">An error occurred while parsing the file.</exception>
    public Tilemap ParseLevel(string filePath, string levelIdentifier, GraphicsDevice graphicsDevice)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(levelIdentifier);
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        string fullPath = _baseDirectory != null
            ? Path.Combine(_baseDirectory, filePath)
            : filePath;

        try
        {
            string projectDirectory = Path.GetDirectoryName(fullPath);
            LDtkProject project = LoadProject(fullPath);

            LDtkLevel level = project.Levels.FirstOrDefault(l => l.Identifier == levelIdentifier);
            if (level == null)
            {
                throw new ArgumentException($"Level '{levelIdentifier}' not found in project.", nameof(levelIdentifier));
            }

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
            throw new TilemapParseException($"Failed to parse level '{levelIdentifier}' from LDtk file: {fullPath}", ex);
        }
    }

    /// <summary>
    /// Parses all levels from an LDtk project file.
    /// </summary>
    /// <param name="filePath">The path to the LDtk project file (.ldtk).</param>
    /// <param name="graphicsDevice">The graphics device used for loading textures.</param>
    /// <returns>A collection of <see cref="Tilemap"/> objects representing all levels in the project.</returns>
    /// <exception cref="FileNotFoundException">The specified file does not exist.</exception>
    /// <exception cref="TilemapParseException">An error occurred while parsing the file.</exception>
    public IReadOnlyList<Tilemap> ParseAllLevels(string filePath, GraphicsDevice graphicsDevice)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        string fullPath = _baseDirectory != null
            ? Path.Combine(_baseDirectory, filePath)
            : filePath;

        try
        {
            string projectDirectory = Path.GetDirectoryName(fullPath);
            LDtkProject project = LoadProject(fullPath);

            List<Tilemap> tilemaps = new List<Tilemap>(project.Levels.Count);
            foreach (LDtkLevel level in project.Levels)
            {
                Tilemap tilemap = ConvertLevel(level, project, graphicsDevice, projectDirectory);
                tilemaps.Add(tilemap);
            }

            return tilemaps;
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
            throw new TilemapParseException($"Failed to parse LDtk file: {fullPath}", ex);
        }
    }

    /// <summary>
    /// Gets the world position of a level from its properties.
    /// </summary>
    /// <param name="tilemap">The tilemap to get world position from.</param>
    /// <returns>A tuple containing the world X and Y coordinates, or null if not available.</returns>
    public static Point? GetWorldPosition(Tilemap tilemap)
    {
        if (tilemap == null)
        {
            return null;
        }

        bool hasWorldX = tilemap.Properties.TryGetValue("LDtk_WorldX", out TilemapPropertyValue worldXValue);
        bool hasWorldY = tilemap.Properties.TryGetValue("LDtk_WorldY", out TilemapPropertyValue worldYValue);

        if (hasWorldX && hasWorldY)
        {
            return new Point(worldXValue.AsInt(), worldYValue.AsInt());
        }

        return null;
    }

    /// <summary>
    /// Finds a level by its world IID (Instance Identifier).
    /// </summary>
    /// <param name="tilemaps">The collection of tilemaps to search.</param>
    /// <param name="worldIid">The world IID to search for.</param>
    /// <returns>The tilemap with matching world IID, or null if not found.</returns>
    public static Tilemap FindLevelByIid(IEnumerable<Tilemap> tilemaps, string worldIid)
    {
        if (tilemaps == null || string.IsNullOrEmpty(worldIid))
        {
            return null;
        }

        return tilemaps.FirstOrDefault(t =>
            t.Properties.TryGetValue("LDtk_Iid", out TilemapPropertyValue iidValue) &&
            iidValue.AsString() == worldIid);
    }

    /// <summary>
    /// Gets the table of contents from an LDtk project file.
    /// </summary>
    /// <param name="filePath">The path to the LDtk project file (.ldtk).</param>
    /// <returns>A dictionary mapping entity identifiers to lists of their instance data.</returns>
    /// <exception cref="FileNotFoundException">The specified file does not exist.</exception>
    /// <exception cref="TilemapParseException">An error occurred while parsing the file.</exception>
    public Dictionary<string, List<LDtkTocInstanceData>> GetTableOfContents(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        string fullPath = _baseDirectory != null
            ? Path.Combine(_baseDirectory, filePath)
            : filePath;

        try
        {
            LDtkProject project = LoadProject(fullPath);

            Dictionary<string, List<LDtkTocInstanceData>> toc = new Dictionary<string, List<LDtkTocInstanceData>>();

            if (project.Toc != null)
            {
                foreach (LDtkTableOfContentEntry entry in project.Toc)
                {
                    if (!string.IsNullOrEmpty(entry.Identifier) && entry.InstancesData != null)
                    {
                        toc[entry.Identifier] = entry.InstancesData;
                    }
                }
            }

            return toc;
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
            throw new TilemapParseException($"Failed to read table of contents from LDtk file: {fullPath}", ex);
        }
    }

    /// <summary>
    /// Parses all levels from a specific world in a multi-world LDtk project.
    /// </summary>
    /// <param name="filePath">The path to the LDtk project file (.ldtk).</param>
    /// <param name="worldIdentifier">The identifier of the world to parse.</param>
    /// <param name="graphicsDevice">The graphics device used for loading textures.</param>
    /// <returns>A collection of <see cref="Tilemap"/> objects representing all levels in the specified world.</returns>
    /// <exception cref="ArgumentException">The specified world was not found in the project.</exception>
    /// <exception cref="FileNotFoundException">The specified file does not exist.</exception>
    /// <exception cref="TilemapParseException">An error occurred while parsing the file.</exception>
    public IReadOnlyList<Tilemap> ParseWorld(string filePath, string worldIdentifier, GraphicsDevice graphicsDevice)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(worldIdentifier);
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        string fullPath = _baseDirectory != null
            ? Path.Combine(_baseDirectory, filePath)
            : filePath;

        try
        {
            string projectDirectory = Path.GetDirectoryName(fullPath);
            LDtkProject project = LoadProject(fullPath);

            if (project.Worlds != null && project.Worlds.Count > 0)
            {
                LDtkWorld world = project.Worlds.FirstOrDefault(w => w.Identifier == worldIdentifier);
                if (world == null)
                {
                    throw new ArgumentException($"World '{worldIdentifier}' not found in project.", nameof(worldIdentifier));
                }

                List<Tilemap> tilemaps = new List<Tilemap>(world.Levels.Count);
                foreach (LDtkLevel level in world.Levels)
                {
                    Tilemap tilemap = ConvertLevel(level, project, graphicsDevice, projectDirectory);

                    tilemap.Properties.SetString("LDtk_WorldIid", world.Iid);
                    tilemap.Properties.SetString("LDtk_WorldIdentifier", world.Identifier);

                    tilemaps.Add(tilemap);
                }

                return tilemaps;
            }
            else
            {
                throw new InvalidOperationException(
                    "This LDtk project does not have multi-worlds enabled. " +
                    "Use ParseAllLevels() instead, or enable multi-worlds in LDtk project settings.");
            }
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException($"Failed to parse world '{worldIdentifier}' from LDtk file: {fullPath}", ex);
        }
    }

    /// <summary>
    /// Gets the list of world identifiers from an LDtk project file.
    /// </summary>
    /// <param name="filePath">The path to the LDtk project file (.ldtk).</param>
    /// <returns>A list of world identifiers, or an empty list if multi-worlds is not enabled.</returns>
    /// <exception cref="FileNotFoundException">The specified file does not exist.</exception>
    /// <exception cref="TilemapParseException">An error occurred while parsing the file.</exception>
    public IReadOnlyList<string> GetWorldIdentifiers(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        string fullPath = _baseDirectory != null
            ? Path.Combine(_baseDirectory, filePath)
            : filePath;

        try
        {
            LDtkProject project = LoadProject(fullPath);

            if (project.Worlds != null && project.Worlds.Count > 0)
            {
                return project.Worlds
                    .Where(w => !string.IsNullOrEmpty(w.Identifier))
                    .Select(w => w.Identifier)
                    .ToList();
            }

            return new List<string>();
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
            throw new TilemapParseException($"Failed to read world list from LDtk file: {fullPath}", ex);
        }
    }

    private LDtkProject LoadProject(string filePath)
    {
        using Stream stream = OpenProjectStream(filePath);
        LDtkProject project = JsonSerializer.Deserialize(stream, LDtkJsonSerializerContext.Default.LDtkProject);

        if (project == null)
        {
            throw new TilemapParseException($"Failed to deserialize LDtk project from file: {filePath}");
        }

        return project;
    }

    private Tilemap ConvertLevel(LDtkLevel level, LDtkProject project, GraphicsDevice graphicsDevice, string projectDirectory)
    {
        string baseDirectory = projectDirectory ?? Directory.GetCurrentDirectory();

        if (!string.IsNullOrEmpty(level.ExternalRelPath))
        {
            string levelPath = Path.Combine(baseDirectory, level.ExternalRelPath);

            LDtkLevel externalLevel = LoadExternalLevel(level, levelPath);

            if (externalLevel != null)
            {
                level = externalLevel;
            }
        }

        TilemapData data = LDtkTilemapDataConverter.Convert(level, project);
        return TilemapFactory.Build(data, graphicsDevice, baseDirectory, _resourceResolver);
    }

    private Stream OpenProjectStream(string filePath)
    {
        try
        {
            return _resourceResolver(filePath);
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
            throw new TilemapParseException($"Failed to open LDtk file: {filePath}", ex);
        }
    }

    private LDtkLevel LoadExternalLevel(LDtkLevel level, string levelPath)
    {
        try
        {
            using Stream stream = _resourceResolver(levelPath);
            return JsonSerializer.Deserialize(stream, LDtkJsonSerializerContext.Default.LDtkLevel);
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException(
                $"Level '{level.Identifier}' references external file '{level.ExternalRelPath}' " +
                $"which could not be opened. Expected at: {levelPath}", ex);
        }
    }
}
