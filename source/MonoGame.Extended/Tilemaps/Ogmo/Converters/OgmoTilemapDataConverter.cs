using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Content;
using MonoGame.Extended.Tilemaps.Ogmo.Document;

namespace MonoGame.Extended.Tilemaps.Ogmo.Converters;

internal static class OgmoTilemapDataConverter
{
    private readonly struct TilesetGidInfo
    {
        public TilesetGidInfo(int firstGid, int columns)
        {
            FirstGid = firstGid;
            Columns = columns;
        }

        public int FirstGid { get; }
        public int Columns { get; }
    }

    public static TilemapData Convert(OgmoLevel level, OgmoProject project)
    {
        return Convert(level, project, Directory.GetCurrentDirectory(), ExternalResourceResolvers.OpenFile);
    }

    public static TilemapData Convert(OgmoLevel level, OgmoProject project, string baseDirectory, ExternalResourceResolver resourceResolver)
    {
        ArgumentNullException.ThrowIfNull(level);
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(baseDirectory);
        ArgumentNullException.ThrowIfNull(resourceResolver);

        int gridSize = (int)(project.LayerGridDefaultSize?.X ?? 16);
        if (gridSize <= 0)
        {
            gridSize = 16;
        }

        TilemapData data = new TilemapData();
        data.Name = Path.GetFileNameWithoutExtension(level.OgmoVersion) ?? "Ogmo Level";
        data.Width = (int)(level.Width / gridSize);
        data.Height = (int)(level.Height / gridSize);
        data.TileWidth = gridSize;
        data.TileHeight = gridSize;
        // Ogmo only supports orthogonal maps.
        data.Orientation = TilemapOrientation.Orthogonal;

        if (!string.IsNullOrEmpty(project.BackgroundColor))
        {
            data.BackgroundColor = OgmoColorParser.ParseColor(project.BackgroundColor);
        }

        Dictionary<string, TilesetGidInfo> tilesetInfo = ConvertTilesets(project, data, baseDirectory, resourceResolver);
        ConvertLayers(level, project, data, tilesetInfo);
        ConvertLevelProperties(level, data);

        return data;
    }

    #region Tilesets

    private static Dictionary<string, TilesetGidInfo> ConvertTilesets(OgmoProject project, TilemapData data, string baseDirectory, ExternalResourceResolver resourceResolver)
    {
        Dictionary<string, TilesetGidInfo> tilesetInfo = new Dictionary<string, TilesetGidInfo>();

        if (project.Tilesets == null)
        {
            return tilesetInfo;
        }

        int currentFirstGid = 1;

        foreach (OgmoTilesetTemplate tilesetTemplate in project.Tilesets)
        {
            if (tilesetTemplate.TileWidth <= 0 || tilesetTemplate.TileHeight <= 0)
            {
                continue;
            }

            string imagePath = tilesetTemplate.Image ?? tilesetTemplate.Path;
            if (string.IsNullOrEmpty(imagePath))
            {
                continue;
            }

            Point imgSize = ReadImageDimensions(imagePath, baseDirectory, resourceResolver);
            int columns = CalculateCount(imgSize.X, tilesetTemplate.TileWidth, tilesetTemplate.TileSeparationX);
            int rows = CalculateCount(imgSize.Y, tilesetTemplate.TileHeight, tilesetTemplate.TileSeparationY);
            int tileCount = columns * rows;

            if (tileCount <= 0)
            {
                continue;
            }

            TilemapTilesetData tilesetData = new TilemapTilesetData();
            tilesetData.Name = tilesetTemplate.Label ?? string.Empty;
            tilesetData.TexturePath = imagePath;
            tilesetData.TileWidth = tilesetTemplate.TileWidth;
            tilesetData.TileHeight = tilesetTemplate.TileHeight;
            tilesetData.TileCount = tileCount;
            tilesetData.Columns = columns;
            tilesetData.Spacing = tilesetTemplate.TileSeparationX;
            tilesetData.Margin = 0;

            TilemapTilesetEntry entry = new TilemapTilesetEntry();
            entry.FirstGlobalId = currentFirstGid;
            entry.IsExternal = false;
            entry.InlineData = tilesetData;

            data.Tilesets.Add(entry);
            tilesetInfo[tilesetTemplate.Label ?? string.Empty] = new TilesetGidInfo(currentFirstGid, columns);
            currentFirstGid += tileCount;
        }

        return tilesetInfo;
    }

    #endregion

    #region Layers

    private static void ConvertLayers(OgmoLevel level, OgmoProject project, TilemapData data, Dictionary<string, TilesetGidInfo> tilesetInfo)
    {
        if (level.Layers == null)
        {
            return;
        }

        foreach (OgmoLayerData layerData in level.Layers)
        {
            OgmoLayerTemplate template = FindTemplate(project, layerData.Name);

            TilemapLayerData converted = null;

            if (layerData is OgmoTileLayerData tileLayerData)
            {
                converted = ConvertTileLayer(tileLayerData, template, tilesetInfo);
            }
            else if (layerData is OgmoEntityLayerData entityLayerData)
            {
                converted = ConvertEntityLayer(entityLayerData);
            }
            else if (layerData is OgmoGridLayerData gridLayerData)
            {
                converted = ConvertGridLayer(gridLayerData, template);
            }
            else if (layerData is OgmoDecalLayerData decalLayerData)
            {
                converted = ConvertDecalLayer(decalLayerData);
            }

            if (converted != null)
            {
                data.Layers.Add(converted);
            }
        }
    }

    private static TilemapTileLayerData ConvertTileLayer(OgmoTileLayerData layerData, OgmoLayerTemplate template, Dictionary<string, TilesetGidInfo> tilesetInfo)
    {
        TilemapTileLayerData result = new TilemapTileLayerData();
        result.Name = layerData.Name ?? string.Empty;
        result.Width = layerData.GridCellsX;
        result.Height = layerData.GridCellsY;
        result.IsVisible = true;
        result.Opacity = 1.0f;
        result.OffsetX = layerData.OffsetX;
        result.OffsetY = layerData.OffsetY;
        // Ogmo layers don't have parallax.
        result.ParallaxX = 1.0f;
        result.ParallaxY = 1.0f;

        string tilesetName = layerData.Tileset ?? (template?.DefaultTileset ?? string.Empty);
        if (!tilesetInfo.TryGetValue(tilesetName, out TilesetGidInfo info))
        {
            return result;
        }

        int firstGid = info.FirstGid;
        int columns = info.Columns;

        if (layerData.ExportMode == 0)
        {
            if (layerData.ArrayMode == 0)
            {
                DecodeTileIds1D(result, layerData.Data, firstGid);
            }
            else
            {
                DecodeTileIds2D(result, layerData.Data2D, firstGid);
            }
        }
        else
        {
            if (layerData.ArrayMode == 0)
            {
                DecodeTileCoords1D(result, layerData.DataCoords, firstGid, columns);
            }
            else
            {
                DecodeTileCoords2D(result, layerData.DataCoords2D, firstGid, columns);
            }
        }

        return result;
    }

    private static TilemapObjectLayerData ConvertEntityLayer(OgmoEntityLayerData layerData)
    {
        TilemapObjectLayerData result = new TilemapObjectLayerData();
        result.Name = layerData.Name ?? string.Empty;
        result.IsVisible = true;
        result.Opacity = 1.0f;
        result.OffsetX = layerData.OffsetX;
        result.OffsetY = layerData.OffsetY;
        result.DrawOrder = TilemapObjectDrawOrder.TopDown;

        if (layerData.Entities != null)
        {
            foreach (OgmoEntity entity in layerData.Entities)
            {
                TilemapRectangleObjectData obj = new TilemapRectangleObjectData();
                obj.Id = entity.Id;
                obj.Name = entity.Name ?? string.Empty;
                obj.Class = string.Empty;
                obj.X = entity.X;
                obj.Y = entity.Y;
                obj.Rotation = entity.Rotation;
                obj.IsVisible = true;
                obj.Width = entity.Width;
                obj.Height = entity.Height;

                if (entity.Values != null)
                {
                    foreach (KeyValuePair<string, object> kvp in entity.Values)
                    {
                        ConvertCustomValue(obj.Properties, kvp.Key, kvp.Value);
                    }
                }

                result.Objects.Add(obj);
            }
        }

        return result;
    }

    private static TilemapObjectLayerData ConvertDecalLayer(OgmoDecalLayerData layerData)
    {
        TilemapObjectLayerData result = new TilemapObjectLayerData();
        result.Name = layerData.Name ?? string.Empty;
        result.IsVisible = true;
        result.Opacity = 1.0f;
        result.OffsetX = layerData.OffsetX;
        result.OffsetY = layerData.OffsetY;
        result.ParallaxX = 1.0f;
        result.ParallaxY = 1.0f;
        result.DrawOrder = TilemapObjectDrawOrder.TopDown;

        if (layerData.Decals != null)
        {
            foreach (OgmoDecal decal in layerData.Decals)
            {
                TilemapRectangleObjectData obj = new TilemapRectangleObjectData();
                obj.Name = decal.Texture ?? string.Empty;
                obj.X = decal.X;
                obj.Y = decal.Y;
                obj.Rotation = decal.Rotation;
                obj.IsVisible = true;

                // Decals have no fixed size; use a placeholder of 16x16.
                obj.Width = 16;
                obj.Height = 16;

                AddStringProperty(obj.Properties, "Texture", decal.Texture ?? string.Empty);
                AddStringProperty(obj.Properties, "Folder", layerData.Folder ?? string.Empty);
                AddFloatProperty(obj.Properties, "ScaleX", decal.ScaleX);
                AddFloatProperty(obj.Properties, "ScaleY", decal.ScaleY);

                if (decal.Values != null)
                {
                    foreach (KeyValuePair<string, object> kvp in decal.Values)
                    {
                        ConvertCustomValue(obj.Properties, kvp.Key, kvp.Value);
                    }
                }

                result.Objects.Add(obj);
            }
        }

        return result;
    }

    private static TilemapTileLayerData ConvertGridLayer(OgmoGridLayerData layerData, OgmoLayerTemplate template)
    {
        TilemapTileLayerData result = new TilemapTileLayerData();
        result.Name = layerData.Name ?? string.Empty;
        result.Width = layerData.GridCellsX;
        result.Height = layerData.GridCellsY;
        result.IsVisible = true;
        result.Opacity = 1.0f;
        result.OffsetX = layerData.OffsetX;
        result.OffsetY = layerData.OffsetY;
        // Ogmo layers don't have parallax.
        result.ParallaxX = 1.0f;
        result.ParallaxY = 1.0f;

        if (layerData.ArrayMode == 0 && layerData.Grid != null)
        {
            string gridJson = JsonSerializer.Serialize(layerData.Grid);
            AddStringProperty(result.Properties, "Ogmo_GridValues", gridJson);
        }
        else if (layerData.ArrayMode == 1 && layerData.Grid2D != null)
        {
            string gridJson = JsonSerializer.Serialize(layerData.Grid2D);
            AddStringProperty(result.Properties, "Ogmo_GridValues", gridJson);
        }

        AddIntProperty(result.Properties, "Ogmo_GridArrayMode", layerData.ArrayMode);

        if (template?.Legend != null && template.Legend.Count > 0)
        {
            string legendJson = JsonSerializer.Serialize(template.Legend);
            AddStringProperty(result.Properties, "Ogmo_GridLegend", legendJson);
        }

        return result;
    }

    #endregion

    #region Level Properties

    private static void ConvertLevelProperties(OgmoLevel level, TilemapData data)
    {
        AddStringProperty(data.Properties, "Ogmo_Version", level.OgmoVersion ?? string.Empty);
        AddIntProperty(data.Properties, "Ogmo_PixelWidth", (int)level.Width);
        AddIntProperty(data.Properties, "Ogmo_PixelHeight", (int)level.Height);
        AddIntProperty(data.Properties, "Ogmo_OffsetX", (int)level.OffsetX);
        AddIntProperty(data.Properties, "Ogmo_OffsetY", (int)level.OffsetY);

        if (level.Values != null && level.Values.Count > 0)
        {
            foreach (KeyValuePair<string, object> kvp in level.Values)
            {
                ConvertCustomValue(data.Properties, $"Level_{kvp.Key}", kvp.Value);
            }
        }
    }

    #endregion

    #region Tile Decode Helpers

    private static void DecodeTileIds1D(TilemapTileLayerData data, List<int> tileIds, int firstGid)
    {
        if (tileIds == null)
        {
            return;
        }

        for (int i = 0; i < tileIds.Count; i++)
        {
            int localId = tileIds[i];
            if (localId < 0)
            {
                continue;
            }

            ushort x = (ushort)(i % data.Width);
            ushort y = (ushort)(i / data.Width);
            data.Tiles.Add(new TilemapDecodedTile(x, y, firstGid + localId, TilemapTileFlipFlags.None));
        }
    }

    private static void DecodeTileIds2D(TilemapTileLayerData data, List<List<int>> tileIds2D, int firstGid)
    {
        if (tileIds2D == null)
        {
            return;
        }

        for (int y = 0; y < tileIds2D.Count && y < data.Height; y++)
        {
            List<int> row = tileIds2D[y];
            if (row == null)
            {
                continue;
            }

            for (int x = 0; x < row.Count && x < data.Width; x++)
            {
                int localId = row[x];
                if (localId < 0)
                {
                    continue;
                }

                data.Tiles.Add(new TilemapDecodedTile(
                    (ushort)x, (ushort)y, firstGid + localId, TilemapTileFlipFlags.None));
            }
        }
    }

    private static void DecodeTileCoords1D(TilemapTileLayerData data, List<Point?> coords, int firstGid, int columns)
    {
        if (coords == null || columns <= 0)
        {
            return;
        }

        for (int i = 0; i < coords.Count; i++)
        {
            Point? coord = coords[i];
            if (!coord.HasValue)
            {
                continue;
            }

            int localId = coord.Value.Y * columns + coord.Value.X;
            ushort x = (ushort)(i % data.Width);
            ushort y = (ushort)(i / data.Width);
            data.Tiles.Add(new TilemapDecodedTile(x, y, firstGid + localId, TilemapTileFlipFlags.None));
        }
    }

    private static void DecodeTileCoords2D(TilemapTileLayerData data, List<List<Point?>> coords2D, int firstGid, int columns)
    {
        if (coords2D == null || columns <= 0)
        {
            return;
        }

        for (int y = 0; y < coords2D.Count && y < data.Height; y++)
        {
            List<Point?> row = coords2D[y];
            if (row == null)
            {
                continue;
            }

            for (int x = 0; x < row.Count && x < data.Width; x++)
            {
                Point? coord = row[x];
                if (!coord.HasValue)
                {
                    continue;
                }

                int localId = coord.Value.Y * columns + coord.Value.X;
                data.Tiles.Add(new TilemapDecodedTile(
                    (ushort)x, (ushort)y, firstGid + localId, TilemapTileFlipFlags.None));
            }
        }
    }

    #endregion

    #region Image Dimension Helpers

    private static Point ReadImageDimensions(string imagePath, string baseDirectory, ExternalResourceResolver resourceResolver)
    {
        if (imagePath.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
        {
            return ReadDimensionsFromDataUri(imagePath);
        }

        try
        {
            // PNG IHDR chunk: bytes 0-7 = signature, 8-11 = IHDR length, 12-15 = IHDR type,
            // 16-19 = width (big-endian), 20-23 = height (big-endian).
            byte[] header = new byte[24];

            string fullPath = Path.IsPathRooted(imagePath)
                ? imagePath
                : Path.Combine(baseDirectory, imagePath);

            using Stream stream = resourceResolver(fullPath);
            int bytesRead = stream.Read(header, 0, 24);

            if (bytesRead < 24)
            {
                return Point.Zero;
            }

            // Verify PNG signature: 0x89 0x50 0x4E 0x47 0x0D 0x0A 0x1A 0x0A
            if (header[0] != 0x89 || header[1] != 0x50 || header[2] != 0x4E || header[3] != 0x47)
            {
                return Point.Zero;
            }

            int width = (header[16] << 24) | (header[17] << 16) | (header[18] << 8) | header[19];
            int height = (header[20] << 24) | (header[21] << 16) | (header[22] << 8) | header[23];
            return new Point(width, height);
        }
        catch (Exception)
        {
            return Point.Zero;
        }
    }

    private static Point ReadDimensionsFromDataUri(string dataUri)
    {
        try
        {
            int commaIndex = dataUri.IndexOf(',');
            if (commaIndex < 0)
            {
                return Point.Zero;
            }

            byte[] bytes = System.Convert.FromBase64String(dataUri.Substring(commaIndex + 1));
            if (bytes.Length < 24)
            {
                return Point.Zero;
            }

            if (bytes[0] != 0x89 || bytes[1] != 0x50 || bytes[2] != 0x4E || bytes[3] != 0x47)
            {
                return Point.Zero;
            }

            int width = (bytes[16] << 24) | (bytes[17] << 16) | (bytes[18] << 8) | bytes[19];
            int height = (bytes[20] << 24) | (bytes[21] << 16) | (bytes[22] << 8) | bytes[23];
            return new Point(width, height);
        }
        catch (Exception)
        {
            return Point.Zero;
        }
    }

    private static int CalculateCount(int imageSize, int tileSize, int separation)
    {
        if (tileSize <= 0 || imageSize <= 0)
        {
            return 0;
        }

        int effectiveSize = tileSize + separation;
        return (imageSize + separation) / effectiveSize;
    }

    #endregion

    #region Template Lookup

    private static OgmoLayerTemplate FindTemplate(OgmoProject project, string layerName)
    {
        if (project.Layers == null || string.IsNullOrEmpty(layerName))
        {
            return null;
        }

        foreach (OgmoLayerTemplate template in project.Layers)
        {
            if (template.Name == layerName)
            {
                return template;
            }
        }

        return null;
    }

    #endregion

    #region Custom Value Conversion

    private static void ConvertCustomValue(List<TilemapPropertyData> properties, string key, object value)
    {
        switch (value)
        {
            case null:
                AddStringProperty(properties, key, string.Empty);
                break;

            case string str:
                AddStringProperty(properties, key, str);
                break;

            case bool boolValue:
                AddBoolProperty(properties, key, boolValue);
                break;

            case int intValue:
                AddIntProperty(properties, key, intValue);
                break;

            case long longValue:
                AddIntProperty(properties, key, (int)longValue);
                break;

            case float floatValue:
                AddFloatProperty(properties, key, floatValue);
                break;

            case double doubleValue:
                AddFloatProperty(properties, key, (float)doubleValue);
                break;

            case JsonElement element:
                ConvertJsonElement(properties, key, element);
                break;

            default:
                AddStringProperty(properties, key, value.ToString() ?? string.Empty);
                break;
        }
    }

    private static void ConvertJsonElement(List<TilemapPropertyData> properties, string key, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.True:
                AddBoolProperty(properties, key, true);
                break;

            case JsonValueKind.False:
                AddBoolProperty(properties, key, false);
                break;

            case JsonValueKind.Number:
                if (element.TryGetInt32(out int intVal))
                {
                    AddIntProperty(properties, key, intVal);
                }
                else if (element.TryGetSingle(out float floatVal))
                {
                    AddFloatProperty(properties, key, floatVal);
                }
                else
                {
                    AddStringProperty(properties, key, element.GetRawText());
                }
                break;

            case JsonValueKind.String:
                AddStringProperty(properties, key, element.GetString() ?? string.Empty);
                break;

            default:
                AddStringProperty(properties, key, element.GetRawText());
                break;
        }
    }

    #endregion

    #region Property Helpers

    private static void AddStringProperty(List<TilemapPropertyData> target, string key, string value)
    {
        target.Add(new TilemapPropertyData { Key = key, Type = TilemapPropertyType.String, StringValue = value });
    }

    private static void AddIntProperty(List<TilemapPropertyData> target, string key, int value)
    {
        target.Add(new TilemapPropertyData { Key = key, Type = TilemapPropertyType.Int, IntValue = value });
    }

    private static void AddFloatProperty(List<TilemapPropertyData> target, string key, float value)
    {
        target.Add(new TilemapPropertyData { Key = key, Type = TilemapPropertyType.Float, FloatValue = value });
    }

    private static void AddBoolProperty(List<TilemapPropertyData> target, string key, bool value)
    {
        target.Add(new TilemapPropertyData { Key = key, Type = TilemapPropertyType.Bool, BoolValue = value });
    }

    #endregion
}
