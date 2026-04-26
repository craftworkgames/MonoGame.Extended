using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Tilemaps.Parsers;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Constructs a <see cref="Tilemap"/> from a <see cref="TilemapData"/> intermediate
/// representation, loading textures using the provided graphics device.
/// </summary>
public static class TilemapFactory
{
    /// <summary>
    /// Builds a <see cref="Tilemap"/> from the given <see cref="TilemapData"/>.
    /// </summary>
    /// <param name="data">The tilemap data.</param>
    /// <param name="graphicsDevice">The graphics device used to load textures.</param>
    /// <param name="baseDirectory">The directory used to resolve relative texture paths.</param>
    /// <returns>The constructed tilemap.</returns>
    /// <exception cref="TilemapParseException">Thrown when a texture cannot be loaded.</exception>
    public static Tilemap Build(TilemapData data, GraphicsDevice graphicsDevice, string baseDirectory)
    {
        return Build(data, graphicsDevice, baseDirectory, ExternalResourceResolvers.OpenFile);
    }

    /// <summary>
    /// Builds a <see cref="Tilemap"/> from the given <see cref="TilemapData"/>.
    /// </summary>
    /// <param name="data">The tilemap data.</param>
    /// <param name="graphicsDevice">The graphics device used to load textures.</param>
    /// <param name="baseDirectory">The directory used to resolve relative texture paths.</param>
    /// <param name="resourceResolver">The resolver used to open texture streams.</param>
    /// <returns>The constructed tilemap.</returns>
    /// <exception cref="TilemapParseException">Thrown when a texture cannot be loaded.</exception>
    public static Tilemap Build(TilemapData data, GraphicsDevice graphicsDevice, string baseDirectory, ExternalResourceResolver resourceResolver)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(graphicsDevice);
        ArgumentNullException.ThrowIfNull(baseDirectory);
        ArgumentNullException.ThrowIfNull(resourceResolver);

        Tilemap tilemap = new Tilemap(
            name: data.Name ?? "Untitled",
            width: data.Width,
            height: data.Height,
            tileWidth: data.TileWidth,
            tileHeight: data.TileHeight,
            orientation: data.Orientation
        );

        tilemap.BackgroundColor = data.BackgroundColor;
        tilemap.ParallaxOrigin = new Vector2(data.ParallaxOriginX, data.ParallaxOriginY);
        tilemap.WorldPosition = new Vector2(data.WorldX, data.WorldY);
        tilemap.WorldDepth = data.WorldDepth;
        tilemap.HexSideLength = data.HexSideLength;
        tilemap.StaggerAxis = data.StaggerAxis;
        tilemap.StaggerIndex = data.StaggerIndex;

        ApplyProperties(data.Properties, tilemap.Properties);

        foreach (TilemapTilesetEntry entry in data.Tilesets)
        {
            if (entry.IsExternal || entry.InlineData == null)
            {
                // External tilesets are resolved by the content pipeline; not expected at runtime.
                continue;
            }

            TilemapTileset tileset = BuildTileset(entry, graphicsDevice, baseDirectory, resourceResolver);
            tilemap.Tilesets.Add(tileset);
        }

        foreach (TilemapLayerData layerData in data.Layers)
        {
            FlattenLayers(layerData, string.Empty, data, graphicsDevice, baseDirectory, resourceResolver, tilemap.Layers);
        }

        return tilemap;
    }

    private static void FlattenLayers(TilemapLayerData layerData, string pathPrefix, TilemapData mapData, GraphicsDevice graphicsDevice, string baseDirectory, ExternalResourceResolver resourceResolver, TilemapLayerCollection layers)
    {
        if (layerData is TilemapGroupLayerData groupData)
        {
            string groupPath = string.IsNullOrEmpty(pathPrefix)
                ? (groupData.Name ?? string.Empty)
                : pathPrefix + "/" + (groupData.Name ?? string.Empty);

            foreach (TilemapLayerData child in groupData.Layers)
            {
                FlattenLayers(child, groupPath, mapData, graphicsDevice, baseDirectory, resourceResolver, layers);
            }
        }
        else
        {
            // Apply path prefix to the DTO name before building so the layer
            // is constructed with the full "Group/SubGroup/Layer" name.
            if (!string.IsNullOrEmpty(pathPrefix))
            {
                layerData.Name = pathPrefix + "/" + (layerData.Name ?? string.Empty);
            }

            TilemapLayer layer = BuildLayer(layerData, mapData, graphicsDevice, baseDirectory, resourceResolver);

            if (layer != null)
            {
                layers.Add(layer);
            }
        }
    }

    #region Tilesets

    private static TilemapTileset BuildTileset(TilemapTilesetEntry entry, GraphicsDevice graphicsDevice, string baseDirectory, ExternalResourceResolver resourceResolver)
    {
        TilemapTilesetData tilesetData = entry.InlineData;
        Texture2D texture = null;

        if (!string.IsNullOrEmpty(tilesetData.TexturePath))
        {
            try
            {
                texture = LoadTexture(tilesetData.TexturePath, graphicsDevice, baseDirectory, resourceResolver);
            }
            catch (TilemapParseException ex)
            {
                throw new TilemapParseException(
                    $"Failed to load texture for tileset '{tilesetData.Name}': {ex.Message}", ex);
            }
        }

        TilemapTileset tileset = new TilemapTileset(
            name: tilesetData.Name,
            texture: texture,
            tileWidth: tilesetData.TileWidth,
            tileHeight: tilesetData.TileHeight,
            tileCount: tilesetData.TileCount,
            columns: tilesetData.Columns,
            spacing: tilesetData.Spacing,
            margin: tilesetData.Margin
        );

        tileset.FirstGlobalId = entry.FirstGlobalId;
        tileset.TileOffset = new Vector2(tilesetData.DrawOffsetX, tilesetData.DrawOffsetY);

        ApplyProperties(tilesetData.Properties, tileset.Properties);

        foreach (TilemapTileEntryData tileEntry in tilesetData.Tiles)
        {
            TilemapTileData tileData = BuildTileData(tileEntry, graphicsDevice, baseDirectory, resourceResolver);
            tileset.AddTileData(tileData);
        }

        return tileset;
    }

    private static TilemapTileData BuildTileData(TilemapTileEntryData entryData, GraphicsDevice graphicsDevice, string baseDirectory, ExternalResourceResolver resourceResolver)
    {
        TilemapTileData tileData = new TilemapTileData(entryData.LocalId);
        tileData.Class = entryData.Class ?? string.Empty;
        tileData.Probability = entryData.Probability;

        ApplyProperties(entryData.Properties, tileData.Properties);

        if (!string.IsNullOrEmpty(entryData.ImagePath))
        {
            try
            {
                tileData.CustomImage = LoadTexture(entryData.ImagePath, graphicsDevice, baseDirectory, resourceResolver);
            }
            catch (TilemapParseException ex)
            {
                throw new TilemapParseException(
                    $"Failed to load image for tile {entryData.LocalId}: {ex.Message}", ex);
            }
        }

        if (entryData.Animation != null && entryData.Animation.Frames.Count > 0)
        {
            TilemapTileAnimationFrame[] frames = new TilemapTileAnimationFrame[entryData.Animation.Frames.Count];

            for (int i = 0; i < entryData.Animation.Frames.Count; i++)
            {
                TilemapAnimationFrameData frameData = entryData.Animation.Frames[i];
                frames[i] = new TilemapTileAnimationFrame(frameData.TileId, frameData.Duration);
            }

            tileData.Animation = new TilemapTileAnimation(frames);
        }

        foreach (TilemapObjectData objData in entryData.CollisionObjects)
        {
            TilemapObject obj = BuildObject(objData);

            if (obj != null)
            {
                tileData.CollisionObjects.Add(obj);
            }
        }

        return tileData;
    }

    #endregion

    #region Layers

    private static TilemapLayer BuildLayer(TilemapLayerData layerData, TilemapData mapData, GraphicsDevice graphicsDevice, string baseDirectory, ExternalResourceResolver resourceResolver)
    {
        TilemapLayer layer;

        switch (layerData)
        {
            case TilemapTileLayerData tileLayerData:
                layer = BuildTileLayer(tileLayerData, mapData);
                break;

            case TilemapObjectLayerData objectLayerData:
                layer = BuildObjectLayer(objectLayerData);
                break;

            case TilemapImageLayerData imageLayerData:
                layer = BuildImageLayer(imageLayerData, graphicsDevice, baseDirectory, resourceResolver);
                break;

            case TilemapDataLayerData dataLayerData:
                layer = BuildDataLayer(dataLayerData, mapData);
                break;

            default:
                return null;
        }

        if (layer != null)
        {
            ApplyLayerBase(layerData, layer);
        }

        return layer;
    }

    private static void ApplyLayerBase(TilemapLayerData source, TilemapLayer target)
    {
        target.Class = source.Class ?? string.Empty;
        target.IsVisible = source.IsVisible;
        target.Opacity = source.Opacity;
        target.TintColor = source.TintColor;
        target.ParallaxFactor = new Vector2(source.ParallaxX, source.ParallaxY);

        // Chunked tile layers carry a chunk-origin offset in OffsetX/OffsetY in addition to
        // the editor-set offset. These are stored together in the DTO; apply as a single offset.
        target.Offset = new Vector2(source.OffsetX, source.OffsetY);

        ApplyProperties(source.Properties, target.Properties);
    }

    private static TilemapTileLayer BuildTileLayer(TilemapTileLayerData data, TilemapData mapData)
    {
        TilemapTileLayer layer = new TilemapTileLayer(
            name: data.Name ?? string.Empty,
            width: data.Width,
            height: data.Height,
            tileWidth: mapData.TileWidth,
            tileHeight: mapData.TileHeight
        );

        foreach (TilemapDecodedTile tile in data.Tiles)
        {
            layer.SetTile(tile.X, tile.Y, new TilemapTile(tile.GlobalId, tile.FlipFlags));
        }

        return layer;
    }

    private static TilemapObjectLayer BuildObjectLayer(TilemapObjectLayerData data)
    {
        TilemapObjectLayer layer = new TilemapObjectLayer(data.Name ?? string.Empty);
        layer.DrawOrder = data.DrawOrder;

        foreach (TilemapObjectData objData in data.Objects)
        {
            TilemapObject obj = BuildObject(objData);

            if (obj != null)
            {
                layer.AddObject(obj);
            }
        }

        return layer;
    }

    private static TilemapImageLayer BuildImageLayer(TilemapImageLayerData data, GraphicsDevice graphicsDevice, string baseDirectory, ExternalResourceResolver resourceResolver)
    {
        if (string.IsNullOrEmpty(data.TexturePath))
        {
            return null;
        }

        Texture2D texture;

        try
        {
            texture = LoadTexture(data.TexturePath, graphicsDevice, baseDirectory, resourceResolver);
        }
        catch (TilemapParseException ex)
        {
            throw new TilemapParseException(
                $"Failed to load texture for image layer '{data.Name}': {ex.Message}", ex);
        }

        TilemapImageLayer layer = new TilemapImageLayer(
            name: data.Name ?? string.Empty,
            texture: texture,
            position: Vector2.Zero
        );

        layer.RepeatX = data.RepeatX;
        layer.RepeatY = data.RepeatY;

        return layer;
    }

    private static TilemapDataLayer BuildDataLayer(TilemapDataLayerData data, TilemapData mapData)
    {
        return new TilemapDataLayer(
            name: data.Name ?? string.Empty,
            width: data.Width,
            height: data.Height,
            tileWidth: mapData.TileWidth,
            tileHeight: mapData.TileHeight
        );
    }

    #endregion

    #region Objects

    private static TilemapObject BuildObject(TilemapObjectData data)
    {
        if (data == null)
        {
            return null;
        }

        Vector2 position = new Vector2(data.X, data.Y);
        TilemapObject obj;

        switch (data)
        {
            case TilemapRectangleObjectData r:
                obj = new TilemapRectangleObject(r.Id, position, new Vector2(r.Width, r.Height));
                break;

            case TilemapEllipseObjectData e:
                obj = new TilemapEllipseObject(e.Id, position, new Vector2(e.Width, e.Height));
                break;

            case TilemapPointObjectData p:
                obj = new TilemapPointObject(p.Id, position);
                break;

            case TilemapPolygonObjectData poly:
                obj = new TilemapPolygonObject(id: data.Id, position: position,
                                               points: poly.Points.ToArray());
                break;

            case TilemapPolylineObjectData line:
                obj = new TilemapPolylineObject(id: data.Id, position: position,
                                                points: line.Points.ToArray());
                break;

            case TilemapTileObjectData t:
                TilemapTile tile = new TilemapTile(t.GlobalId, t.FlipFlags);
                obj = new TilemapTileObject(t.Id, position, tile, new Vector2(t.Width, t.Height));
                break;

            case TilemapTextObjectData txt:
                TilemapTextObject textObj = new TilemapTextObject(
                    id: txt.Id,
                    position: position,
                    size: new Vector2(txt.Width, txt.Height),
                    text: txt.Text ?? string.Empty
                );
                textObj.FontFamily = txt.FontFamily ?? "sans-serif";
                textObj.PixelSize = txt.PixelSize;
                textObj.WordWrap = txt.WordWrap;
                textObj.Color = txt.Color;
                textObj.Bold = txt.Bold;
                textObj.Italic = txt.Italic;
                textObj.Underline = txt.Underline;
                textObj.Strikethrough = txt.Strikethrough;
                textObj.HorizontalAlign = txt.HorizontalAlign;
                textObj.VerticalAlign = txt.VerticalAlign;
                obj = textObj;
                break;

            default:
                return null;
        }

        obj.Name = data.Name ?? string.Empty;
        obj.Class = data.Class ?? string.Empty;
        obj.Rotation = data.Rotation;
        obj.IsVisible = data.IsVisible;

        ApplyProperties(data.Properties, obj.Properties);

        return obj;
    }

    #endregion

    #region Properties

    private static void ApplyProperties(List<TilemapPropertyData> source, TilemapProperties target)
    {
        if (source == null || source.Count == 0)
        {
            return;
        }

        foreach (TilemapPropertyData prop in source)
        {
            if (string.IsNullOrEmpty(prop.Key))
            {
                continue;
            }

            switch (prop.Type)
            {
                case TilemapPropertyType.String:
                case TilemapPropertyType.File:
                    target.SetString(prop.Key, prop.StringValue ?? string.Empty);
                    break;

                case TilemapPropertyType.Int:
                case TilemapPropertyType.Object:
                    target.SetInt(prop.Key, prop.IntValue);
                    break;

                case TilemapPropertyType.Float:
                    target.SetFloat(prop.Key, prop.FloatValue);
                    break;

                case TilemapPropertyType.Bool:
                    target.SetBool(prop.Key, prop.BoolValue);
                    break;

                case TilemapPropertyType.Color:
                    target.SetColor(prop.Key, prop.ColorValue);
                    break;
            }
        }
    }

    #endregion

    #region Texture Loading

    private static Texture2D LoadTexture(string path, GraphicsDevice graphicsDevice, string baseDirectory, ExternalResourceResolver resourceResolver)
    {
        // Support Ogmo-style embedded tileset images stored as base64 data URIs.
        if (path.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
        {
            return LoadTextureFromDataUri(path, graphicsDevice);
        }

        string fullPath = Path.IsPathRooted(path)
            ? path
            : Path.Combine(baseDirectory, path);

        try
        {
            using Stream stream = resourceResolver(fullPath);
            return Texture2D.FromStream(graphicsDevice, stream);
        }
        catch (Exception ex)
        {
            throw new TilemapParseException($"Failed to load texture: {fullPath}", ex);
        }
    }

    private static Texture2D LoadTextureFromDataUri(string dataUri, GraphicsDevice graphicsDevice)
    {
        int commaIndex = dataUri.IndexOf(',');
        if (commaIndex < 0)
        {
            throw new TilemapParseException("Invalid data URI: missing comma separator.");
        }

        try
        {
            byte[] bytes = Convert.FromBase64String(dataUri.Substring(commaIndex + 1));
            using MemoryStream ms = new MemoryStream(bytes);
            return Texture2D.FromStream(graphicsDevice, ms);
        }
        catch (Exception ex)
        {
            throw new TilemapParseException("Failed to load texture from data URI.", ex);
        }
    }

    #endregion
}
