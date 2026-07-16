using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Tilemaps.Parsers;
using MonoGame.Extended.Tilemaps.Tiled.Converters;
using MonoGame.Extended.Tilemaps.Tiled.Document;

namespace MonoGame.Extended.Tilemaps.Tiled;

/// <summary>
/// Parser for Tiled TMX (Tile Map XML) files.
/// </summary>
public class TiledTmxParser : ITilemapParser
{
    private readonly string _baseDirectory;
    private readonly ExternalResourceResolver _resourceResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="TiledTmxParser"/> class.
    /// </summary>
    /// <param name="baseDirectory">
    /// Optional base directory for resolving relative file paths. If provided, file paths in
    /// <see cref="ParseFromFile"/> will be resolved relative to this directory.
    /// If <see langword="null"/>, paths are resolved from the file's own location.
    /// </param>
    /// <param name="resourceResolver">
    /// Optional resolver used to open external resources referenced by the map. If
    /// <see langword="null"/>, resources are opened from the local file system.
    /// </param>
    public TiledTmxParser(string baseDirectory = null, ExternalResourceResolver resourceResolver = null)
    {
        _baseDirectory = baseDirectory;
        _resourceResolver = resourceResolver ?? ExternalResourceResolvers.OpenFile;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> SupportedExtensions => new[] { ".tmx" };

    /// <summary>
    /// Parses a Tiled TMX file from disk.
    /// </summary>
    /// <param name="path">The path to the TMX file.</param>
    /// <param name="graphicsDevice">The graphics device for loading textures.</param>
    /// <returns>The parsed tilemap.</returns>
    /// <exception cref="ArgumentNullException">Thrown when path or graphicsDevice is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    /// <exception cref="TilemapParseException">Thrown when parsing fails.</exception>
    public Tilemap ParseFromFile(string path, GraphicsDevice graphicsDevice)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        ArgumentNullException.ThrowIfNull(graphicsDevice);

        string fullPath = _baseDirectory != null
            ? Path.Combine(_baseDirectory, path)
            : path;

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Tilemap file not found: {fullPath}", fullPath);
        }

        try
        {
            string baseDirectory = Path.GetDirectoryName(Path.GetFullPath(fullPath));

            TiledMapXml mapXml;

            using (Stream stream = File.OpenRead(fullPath))
            {
                mapXml = DeserializeMap(stream);
            }

            LoadExternalTilesets(mapXml, baseDirectory);

            TilemapData data = TiledTilemapDataConverter.Convert(mapXml);
            return TilemapFactory.Build(data, graphicsDevice, baseDirectory, _resourceResolver);
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException($"Failed to parse TMX file: {fullPath}", ex);
        }
    }

    /// <summary>
    /// Parses a Tiled TMX file from a stream.
    /// </summary>
    /// <param name="stream">The stream containing TMX data.</param>
    /// <param name="graphicsDevice">The graphics device for loading textures.</param>
    /// <param name="basePath">
    /// Optional base path for resolving relative file references. If not provided,
    /// uses the base directory from the constructor, or the current directory if neither is set.
    /// </param>
    /// <returns>The parsed tilemap.</returns>
    /// <exception cref="TilemapParseException">Thrown when parsing fails.</exception>
    public Tilemap ParseFromStream(Stream stream, GraphicsDevice graphicsDevice, string basePath = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        try
        {
            string baseDirectory = basePath ?? _baseDirectory ?? Directory.GetCurrentDirectory();

            TiledMapXml mapXml = DeserializeMap(stream);

            LoadExternalTilesets(mapXml, baseDirectory);

            TilemapData data = TiledTilemapDataConverter.Convert(mapXml);
            return TilemapFactory.Build(data, graphicsDevice, baseDirectory, _resourceResolver);
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException("Failed to parse TMX from stream", ex);
        }
    }

    private static TiledMapXml DeserializeMap(Stream stream)
    {
        try
        {
            XDocument document = XDocument.Load(stream);
            if (document.Root == null)
            {
                throw new TilemapParseException("TMX document has no XML root element.");
            }

            return TiledXmlParser.ParseMap(document.Root);
        }
        catch (Exception ex)
        {
            throw new TilemapParseException("Failed to deserialize TMX XML", ex);
        }
    }

    private void LoadExternalTilesets(TiledMapXml mapXml, string baseDirectory)
    {
        if (mapXml.Tilesets == null)
        {
            return;
        }

        foreach (TiledTilesetRefXml tilesetRef in mapXml.Tilesets)
        {
            if (string.IsNullOrEmpty(tilesetRef.Source))
            {
                continue;
            }

            string tsxPath = Path.Combine(baseDirectory, tilesetRef.Source);

            TiledTilesetXml tilesetXml;

            using (Stream stream = OpenExternalTilesetStream(tilesetRef, tsxPath))
            {
                try
                {
                    XDocument document = XDocument.Load(stream);
                    if (document.Root == null)
                    {
                        throw new TilemapParseException("TSX document has no XML root element.");
                    }

                    tilesetXml = TiledXmlParser.ParseTileset(document.Root);
                }
                catch (Exception ex)
                {
                    throw new TilemapParseException($"Failed to parse TSX file: {tsxPath}", ex);
                }
            }

            tilesetXml.FirstGlobalId = tilesetRef.FirstGlobalId;
            tilesetRef.TilesetData = tilesetXml;
        }
    }

    private Stream OpenExternalTilesetStream(TiledTilesetRefXml tilesetRef, string tsxPath)
    {
        try
        {
            return _resourceResolver(tsxPath);
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException(
                $"External tileset '{tilesetRef.Source}' (firstgid={tilesetRef.FirstGlobalId}) " +
                $"could not be opened. Expected at: {tsxPath}", ex);
        }
    }
}
