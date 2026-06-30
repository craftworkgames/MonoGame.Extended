using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Tilemaps.LDtk.Document;
using MonoGame.Extended.Tilemaps.Parsers;

namespace MonoGame.Extended.Tilemaps.LDtk.Converters;

internal static class LDtkTilemapDataConverter
{
    public static TilemapData Convert(LDtkLevel level, LDtkProject project)
    {
        ArgumentNullException.ThrowIfNull(level);
        ArgumentNullException.ThrowIfNull(project);

        int tileSize = GetLevelGridSize(level, project);
        Point levelSize = GetLevelSize(level, tileSize);

        TilemapData data = new TilemapData();
        data.Name = level.Identifier ?? "Untitled";
        data.Width = levelSize.X;
        data.Height = levelSize.Y;
        data.TileWidth = tileSize;
        data.TileHeight = tileSize;
        // LDtk only supports orthogonal maps.
        data.Orientation = TilemapOrientation.Orthogonal;

        if (!string.IsNullOrEmpty(level.BgColor))
        {
            data.BackgroundColor = LDtkColorParser.ParseColor(level.BgColor);
        }

        Dictionary<int, int> tilesetFirstGids = ConvertTilesets(project, data);
        ConvertLayers(level, data, tilesetFirstGids);
        ConvertLevelProperties(level, data);

        return data;
    }

    private static int GetLevelGridSize(LDtkLevel level, LDtkProject project)
    {
        if (level.LayerInstances != null)
        {
            foreach (LDtkLayerInstance layer in level.LayerInstances)
            {
                if (layer.Type == "Entities" || layer.GridSize <= 0)
                {
                    continue;
                }

                return layer.GridSize;
            }
        }

        return project.DefaultGridSize > 0 ? project.DefaultGridSize : 16;
    }

    private static Point GetLevelSize(LDtkLevel level, int tileSize)
    {
        if (level.LayerInstances != null)
        {
            foreach (LDtkLayerInstance layer in level.LayerInstances)
            {
                if (layer.Type == "Entities" || layer.CWid <= 0 || layer.CHei <= 0)
                {
                    continue;
                }

                // TODO: For now, treating LDtk levels as single-grid maps. 
                //       Might need to revisit if mixed-grid levels cause issues with
                //       per-layer cell sizes.
                return new Point(layer.CWid, layer.CHei);
            }
        }

        int x = tileSize <= 0 ? 0 : (tileSize + level.PxWid - 1) / tileSize;
        int y = tileSize <= 0 ? 0 : (tileSize + level.PxHei - 1) / tileSize;
        return new Point(x, y);
    }


    #region Tilesets

    private static Dictionary<int, int> ConvertTilesets(LDtkProject project, TilemapData data)
    {
        Dictionary<int, int> tilesetFirstGids = new Dictionary<int, int>();

        if (project.Defs?.Tilesets == null)
        {
            return tilesetFirstGids;
        }

        // GIDs start at 1 because 0 is the universally reserved empty tile sentinel,
        // keeping the global ID space consistent across both Tiled and LDtk formats.
        int currentFirstGid = 1;

        foreach (LDtkTilesetDefinition tilesetDef in project.Defs.Tilesets)
        {
            if (tilesetDef.TileGridSize <= 0)
            {
                continue;
            }

            // Skip embedded tilesets that have no external image file (e.g., LDtk's Internal_Icons).
            if (string.IsNullOrEmpty(tilesetDef.RelPath))
            {
                continue;
            }

            int columns = tilesetDef.PxWid / tilesetDef.TileGridSize;
            int rows = tilesetDef.PxHei / tilesetDef.TileGridSize;
            int tileCount = columns * rows;

            TilemapTilesetData tilesetData = new TilemapTilesetData();
            tilesetData.Name = tilesetDef.Identifier ?? string.Empty;
            tilesetData.TexturePath = tilesetDef.RelPath ?? string.Empty;
            tilesetData.TileWidth = tilesetDef.TileGridSize;
            tilesetData.TileHeight = tilesetDef.TileGridSize;
            tilesetData.TileCount = tileCount;
            tilesetData.Columns = columns;
            tilesetData.Spacing = tilesetDef.Spacing;
            tilesetData.Margin = tilesetDef.Padding;

            AddIntProperty(tilesetData.Properties, "LDtk_Uid", tilesetDef.Uid);

            if (!string.IsNullOrEmpty(tilesetDef.RelPath))
            {
                AddStringProperty(tilesetData.Properties, "LDtk_RelPath", tilesetDef.RelPath);
            }

            TilemapTilesetEntry entry = new TilemapTilesetEntry();
            entry.FirstGlobalId = currentFirstGid;
            entry.IsExternal = false;
            entry.InlineData = tilesetData;

            data.Tilesets.Add(entry);
            tilesetFirstGids[tilesetDef.Uid] = currentFirstGid;
            currentFirstGid += tileCount;
        }

        return tilesetFirstGids;
    }

    #endregion

    #region Layers

    private static void ConvertLayers(LDtkLevel level, TilemapData data, Dictionary<int, int> tilesetFirstGids)
    {
        if (level.LayerInstances == null || level.LayerInstances.Count == 0)
        {
            return;
        }

        // LDtk layers are ordered top-to-bottom; reverse to bottom-to-top for the runtime.
        for (int i = level.LayerInstances.Count - 1; i >= 0; i--)
        {
            LDtkLayerInstance layerInstance = level.LayerInstances[i];
            List<TilemapLayerData> converted = ConvertLayerInstance(layerInstance, tilesetFirstGids);

            foreach (TilemapLayerData layer in converted)
            {
                data.Layers.Add(layer);
            }
        }
    }

    private static List<TilemapLayerData> ConvertLayerInstance(LDtkLayerInstance layerInstance, Dictionary<int, int> tilesetFirstGids)
    {
        switch (layerInstance.Type)
        {
            case "Tiles":
                return ConvertTileLayer(layerInstance, tilesetFirstGids);

            case "AutoLayer":
                return ConvertAutoLayer(layerInstance, tilesetFirstGids);

            case "IntGrid":
                return ConvertIntGridLayer(layerInstance, tilesetFirstGids);

            case "Entities":
                return new List<TilemapLayerData> { ConvertEntityLayer(layerInstance) };

            default:
                return new List<TilemapLayerData>();
        }
    }

    private static List<TilemapLayerData> ConvertTileLayer(LDtkLayerInstance layerInstance, Dictionary<int, int> tilesetFirstGids)
    {
        int gridWidth = layerInstance.CWid;
        int gridHeight = layerInstance.CHei;
        string name = layerInstance.Identifier ?? "Untitled";

        // LDtk allows multiple manually placed tiles at a single grid cell.
        // Group tiles by cell to determine how many sub-layers are needed.
        Dictionary<long, List<LDtkTileInstance>> tilesByCell =
            GroupTilesByCell(layerInstance.GridTiles, layerInstance.GridSize);

        List<TilemapLayerData> result = CreateSubLayers(
            tilesByCell, name, gridWidth, gridHeight,
            tilesetFirstGids, layerInstance.TilesetDefUid, layerInstance);

        // AutoLayerTiles on a Tiles layer sit on top of the manually placed tiles.
        if (layerInstance.AutoLayerTiles != null && layerInstance.AutoLayerTiles.Count > 0)
        {
            Dictionary<long, List<LDtkTileInstance>> autoTilesByCell =
                GroupTilesByCell(layerInstance.AutoLayerTiles, layerInstance.GridSize);

            List<TilemapLayerData> autoLayers = CreateSubLayers(
                autoTilesByCell, $"{name}_Auto", gridWidth, gridHeight,
                tilesetFirstGids, layerInstance.TilesetDefUid, layerInstance);

            result.AddRange(autoLayers);
        }

        if (result.Count == 0)
        {
            result.Add(CreateEmptyTileLayer(name, gridWidth, gridHeight, layerInstance));
        }

        return result;
    }

    private static List<TilemapLayerData> ConvertAutoLayer(LDtkLayerInstance layerInstance, Dictionary<int, int> tilesetFirstGids)
    {
        int gridWidth = layerInstance.CWid;
        int gridHeight = layerInstance.CHei;
        string name = layerInstance.Identifier ?? "Untitled";

        Dictionary<long, List<LDtkTileInstance>> tilesByCell =
            GroupTilesByCell(layerInstance.AutoLayerTiles, layerInstance.GridSize);

        List<TilemapLayerData> result = CreateSubLayers(
            tilesByCell, name, gridWidth, gridHeight,
            tilesetFirstGids, layerInstance.TilesetDefUid, layerInstance);

        if (result.Count == 0)
        {
            result.Add(CreateEmptyTileLayer(name, gridWidth, gridHeight, layerInstance));
        }

        return result;
    }

    private static List<TilemapLayerData> ConvertIntGridLayer(LDtkLayerInstance layerInstance, Dictionary<int, int> tilesetFirstGids)
    {
        List<TilemapLayerData> result = new List<TilemapLayerData>();

        int gridWidth = layerInstance.CWid;
        int gridHeight = layerInstance.CHei;
        string name = layerInstance.Identifier ?? "Untitled";

        TilemapDataLayerData dataLayer = new TilemapDataLayerData();
        dataLayer.Width = gridWidth;
        dataLayer.Height = gridHeight;
        ApplyLayerBase(dataLayer, layerInstance);

        if (layerInstance.IntGridCsv != null && layerInstance.IntGridCsv.Count > 0)
        {
            // IntGrid values are always a data concern, regardless of whether the layer
            // also has a visual auto-tileset. The auto-tiles are pre-baked by LDtk at
            // export time and do not depend on the IntGrid values at parse time.
            string csv = string.Join(",", layerInstance.IntGridCsv);
            AddStringProperty(dataLayer.Properties, "LDtk_IntGridCsv", csv);
        }

        result.Add(dataLayer);

        if (layerInstance.AutoLayerTiles != null && layerInstance.AutoLayerTiles.Count > 0)
        {
            Dictionary<long, List<LDtkTileInstance>> tilesByCell =
                GroupTilesByCell(layerInstance.AutoLayerTiles, layerInstance.GridSize);

            List<TilemapLayerData> autoLayers = CreateSubLayers(
                tilesByCell, name, gridWidth, gridHeight,
                tilesetFirstGids, layerInstance.TilesetDefUid, layerInstance);

            result.AddRange(autoLayers);
        }

        return result;
    }

    private static TilemapObjectLayerData ConvertEntityLayer(LDtkLayerInstance layerInstance)
    {
        TilemapObjectLayerData data = new TilemapObjectLayerData();
        data.DrawOrder = TilemapObjectDrawOrder.TopDown;
        ApplyLayerBase(data, layerInstance);

        if (layerInstance.EntityInstances != null)
        {
            foreach (LDtkEntityInstance entity in layerInstance.EntityInstances)
            {
                TilemapRectangleObjectData obj = new TilemapRectangleObjectData();
                // LDtk entity IIDs are GUIDs; hash to an int for cross-format compatibility.
                obj.Id = entity.Iid != null ? entity.Iid.GetHashCode() : 0;
                obj.Name = entity.Identifier ?? string.Empty;
                obj.Class = entity.Tags != null ? string.Join(',', entity.Tags) : string.Empty;
                obj.X = entity.Px != null && entity.Px.Count > 0 ? entity.Px[0] : 0f;
                obj.Y = entity.Px != null && entity.Px.Count > 1 ? entity.Px[1] : 0f;
                obj.Rotation = 0f;
                obj.IsVisible = true;
                obj.Width = entity.Width;
                obj.Height = entity.Height;

                AddIntProperty(obj.Properties, "LDtk_DefUid", entity.DefUid);
                AddStringProperty(obj.Properties, "LDtk_Iid", entity.Iid ?? string.Empty);

                if (entity.FieldInstances != null && entity.FieldInstances.Count > 0)
                {
                    ConvertFieldInstances(entity.FieldInstances.ToArray(), obj.Properties);
                }

                data.Objects.Add(obj);
            }
        }

        return data;
    }

    #endregion

    #region Layer Helpers

    private static Dictionary<long, List<LDtkTileInstance>> GroupTilesByCell(List<LDtkTileInstance> tiles, int gridSize)
    {
        Dictionary<long, List<LDtkTileInstance>> tilesByCell =
            new Dictionary<long, List<LDtkTileInstance>>();

        if (tiles == null || gridSize <= 0)
        {
            return tilesByCell;
        }

        foreach (LDtkTileInstance tile in tiles)
        {
            if (tile.Px == null || tile.Px.Count < 2)
            {
                continue;
            }

            int cellX = tile.Px[0] / gridSize;
            int cellY = tile.Px[1] / gridSize;
            long key = ((long)cellY << 32) | (uint)cellX;

            if (!tilesByCell.TryGetValue(key, out List<LDtkTileInstance> list))
            {
                list = new List<LDtkTileInstance>();
                tilesByCell[key] = list;
            }

            list.Add(tile);
        }

        return tilesByCell;
    }

    private static List<TilemapLayerData> CreateSubLayers(Dictionary<long, List<LDtkTileInstance>> tilesByCell, string baseName, int gridWidth, int gridHeight, Dictionary<int, int> tilesetFirstGids, int? tilesetDefUid, LDtkLayerInstance layerInstance)
    {
        int maxDepth = 0;
        foreach (List<LDtkTileInstance> cell in tilesByCell.Values)
        {
            if (cell.Count > maxDepth)
            {
                maxDepth = cell.Count;
            }
        }

        List<TilemapLayerData> result = new List<TilemapLayerData>();

        for (int depth = 0; depth < maxDepth; depth++)
        {
            string subName = maxDepth == 1
                           ? baseName
                           : $"{baseName}_Sub{depth}";

            TilemapTileLayerData layerData = new TilemapTileLayerData();
            layerData.Width = gridWidth;
            layerData.Height = gridHeight;

            ApplyLayerBase(layerData, layerInstance);
            layerData.Name = subName;

            foreach (KeyValuePair<long, List<LDtkTileInstance>> kvp in tilesByCell)
            {
                if (depth >= kvp.Value.Count)
                {
                    continue;
                }

                LDtkTileInstance tile = kvp.Value[depth];
                int cellX = (int)(kvp.Key & 0xFFFFFFFF);
                int cellY = (int)(kvp.Key >> 32);
                int globalId = ConvertToGlobalId(tile.TileId, tilesetFirstGids, tilesetDefUid);

                TilemapTileFlipFlags flags = ConvertFlipFlags(tile.FlipFlags);
                layerData.Tiles.Add(new TilemapDecodedTile(
                    (ushort)cellX, (ushort)cellY, globalId, flags));
            }

            result.Add(layerData);
        }

        return result;
    }

    private static TilemapTileLayerData CreateEmptyTileLayer(string name, int gridWidth, int gridHeight, LDtkLayerInstance layerInstance)
    {
        TilemapTileLayerData layerData = new TilemapTileLayerData();
        layerData.Width = gridWidth;
        layerData.Height = gridHeight;
        ApplyLayerBase(layerData, layerInstance);
        return layerData;
    }

    private static void ApplyLayerBase(TilemapLayerData target, LDtkLayerInstance source)
    {
        target.Name = source.Identifier ?? string.Empty;
        target.Class = string.Empty;
        target.IsVisible = source.Visible;
        target.Opacity = source.Opacity;
        target.OffsetX = source.PxOffsetX;
        target.OffsetY = source.PxOffsetY;

        // LDtk parallax: 1 + LDtk factor = Tiled equivalent (0 = stationary, 1 = normal, 2 = 2x speed).
        target.ParallaxX = 1.0f + source.ParallaxFactorX;
        target.ParallaxY = 1.0f + source.ParallaxFactorY;

        // LDtk layers don't have tint color.
        target.TintColor = null;

        AddIntProperty(target.Properties, "LDtk_Uid", source.LayerDefUid);
        AddStringProperty(target.Properties, "LDtk_Type", source.Type ?? string.Empty);
        AddIntProperty(target.Properties, "LDtk_GridSize", source.GridSize);
    }

    #endregion

    #region GID and Flip Conversion

    private static int ConvertToGlobalId(int localId, Dictionary<int, int> tilesetFirstGids, int? tilesetDefUid)
    {
        if (tilesetDefUid.HasValue &&
            tilesetFirstGids.TryGetValue(tilesetDefUid.Value, out int firstGid))
        {
            return firstGid + localId;
        }

        return localId;
    }

    private static TilemapTileFlipFlags ConvertFlipFlags(int ldtkFlags)
    {
        TilemapTileFlipFlags flags = TilemapTileFlipFlags.None;

        if ((ldtkFlags & 1) != 0)
        {
            flags |= TilemapTileFlipFlags.FlipHorizontally;
        }

        if ((ldtkFlags & 2) != 0)
        {
            flags |= TilemapTileFlipFlags.FlipVertically;
        }

        // LDtk does not support diagonal flip.

        return flags;
    }

    #endregion

    #region Level Properties

    private static void ConvertLevelProperties(LDtkLevel level, TilemapData data)
    {
        AddStringProperty(data.Properties, "LDtk_Iid", level.Iid ?? string.Empty);
        AddIntProperty(data.Properties, "LDtk_Uid", level.Uid);
        AddIntProperty(data.Properties, "LDtk_WorldX", level.WorldX);
        AddIntProperty(data.Properties, "LDtk_WorldY", level.WorldY);
        AddIntProperty(data.Properties, "LDtk_WorldDepth", level.WorldDepth);
        data.WorldX = level.WorldX;
        data.WorldY = level.WorldY;
        data.WorldDepth = level.WorldDepth;

        if (level.Neighbours != null && level.Neighbours.Count > 0)
        {
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < level.Neighbours.Count; i++)
            {
                LDtkNeighbourLevel neighbour = level.Neighbours[i];
                if (i > 0)
                {
                    sb.Append(',');
                }
                sb.Append("{\"dir\":\"");
                sb.Append(neighbour.Dir ?? string.Empty);
                sb.Append("\",\"levelIid\":\"");
                sb.Append(neighbour.LevelIid ?? string.Empty);
                sb.Append("\"}");
            }
            sb.Append(']');
            AddStringProperty(data.Properties, "LDtk_Neighbours", sb.ToString());
        }

        if (level.FieldInstances != null && level.FieldInstances.Count > 0)
        {
            ConvertFieldInstances(level.FieldInstances.ToArray(), data.Properties);
        }
    }

    #endregion

    #region Field Instance Conversion

    private static void ConvertFieldInstances(LDtkFieldInstance[] fieldInstances, List<TilemapPropertyData> target)
    {
        foreach (LDtkFieldInstance field in fieldInstances)
        {
            if (string.IsNullOrEmpty(field.Identifier))
            {
                continue;
            }

            try
            {
                ConvertFieldInstance(field, target);
            }
            catch (Exception ex)
            {
                throw new TilemapParseException(
                    $"Failed to convert LDtk field '{field.Identifier}' of type '{field.Type}': {ex.Message}", ex);
            }
        }
    }

    private static void ConvertFieldInstance(LDtkFieldInstance field, List<TilemapPropertyData> target)
    {
        if (!field.Value.HasValue)
        {
            AddStringProperty(target, field.Identifier, string.Empty);
            return;
        }

        JsonElement jsonValue = field.Value.Value;

        switch (field.Type)
        {
            case "Int":
                if (jsonValue.ValueKind == JsonValueKind.Number && jsonValue.TryGetInt32(out int intValue))
                {
                    AddIntProperty(target, field.Identifier, intValue);
                }
                else
                {
                    AddStringProperty(target, field.Identifier, jsonValue.GetRawText());
                }
                break;

            case "Float":
                if (jsonValue.ValueKind == JsonValueKind.Number && jsonValue.TryGetSingle(out float floatValue))
                {
                    AddFloatProperty(target, field.Identifier, floatValue);
                }
                else
                {
                    AddStringProperty(target, field.Identifier, jsonValue.GetRawText());
                }
                break;

            case "Bool":
                if (jsonValue.ValueKind == JsonValueKind.True || jsonValue.ValueKind == JsonValueKind.False)
                {
                    AddBoolProperty(target, field.Identifier, jsonValue.GetBoolean());
                }
                else
                {
                    AddStringProperty(target, field.Identifier, jsonValue.GetRawText());
                }
                break;

            case "Color":
                if (jsonValue.ValueKind == JsonValueKind.String)
                {
                    Color color = LDtkColorParser.ParseColor(jsonValue.GetString());
                    AddColorProperty(target, field.Identifier, color);
                }
                else
                {
                    AddStringProperty(target, field.Identifier, jsonValue.GetRawText());
                }
                break;

            case "EntityRef":
                if (jsonValue.ValueKind == JsonValueKind.Object &&
                    jsonValue.TryGetProperty("entityIid", out JsonElement iidElement))
                {
                    AddStringProperty(target, field.Identifier, iidElement.GetString() ?? string.Empty);
                }
                else
                {
                    AddStringProperty(target, field.Identifier, string.Empty);
                }
                break;

            default:
                // String, Text, Multilines, FilePath, Point, and any unknown types are serialized as string.
                if (jsonValue.ValueKind == JsonValueKind.String)
                {
                    AddStringProperty(target, field.Identifier, jsonValue.GetString() ?? string.Empty);
                }
                else
                {
                    AddStringProperty(target, field.Identifier, jsonValue.GetRawText());
                }
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

    private static void AddColorProperty(List<TilemapPropertyData> target, string key, Color value)
    {
        target.Add(new TilemapPropertyData { Key = key, Type = TilemapPropertyType.Color, ColorValue = value });
    }

    #endregion
}
