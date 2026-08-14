using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MonoGame.Extended.Tilemaps.Ogmo.Document;

namespace MonoGame.Extended.Tilemaps.Ogmo;

internal sealed class OgmoLayerDataConverter : JsonConverter<OgmoLayerData>
{
    public override OgmoLayerData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("tileset", out _) ||
            root.TryGetProperty("data", out _) ||
            root.TryGetProperty("data2D", out _))
        {
            return JsonSerializer.Deserialize(root.GetRawText(), OgmoJsonSerializerContext.Default.OgmoTileLayerData);
        }
        else if (root.TryGetProperty("grid", out _) ||
                 root.TryGetProperty("grid2D", out _))
        {
            return JsonSerializer.Deserialize(root.GetRawText(), OgmoJsonSerializerContext.Default.OgmoGridLayerData);
        }
        else if (root.TryGetProperty("entities", out _))
        {
            return JsonSerializer.Deserialize(root.GetRawText(), OgmoJsonSerializerContext.Default.OgmoEntityLayerData);
        }
        else if (root.TryGetProperty("decals", out _))
        {
            return JsonSerializer.Deserialize(root.GetRawText(), OgmoJsonSerializerContext.Default.OgmoDecalLayerData);
        }

        #region Fallback Read

        // If we can't determine the type, return base layer data.
        // Manually construct base OgmoLayerData to avoid StackOverflowException.
        OgmoLayerData fallback = new();
        
        if (root.TryGetProperty("name", out JsonElement nameElement))
        {
            fallback.Name = nameElement.GetString();
        }
        if (root.TryGetProperty("_eid", out JsonElement eidElement))
        {
            fallback.ExportID = eidElement.GetString();
        }
        if (root.TryGetProperty("offsetX", out JsonElement offsetXElement)
            && offsetXElement.TryGetSingle(out float offsetX))
        {
            fallback.OffsetX = offsetX;
        }
        if (root.TryGetProperty("offsetY", out JsonElement offsetYElement)
            && offsetYElement.TryGetSingle(out float offsetY))
        {
            fallback.OffsetY = offsetY;
        }
        if (root.TryGetProperty("gridCellWidth", out JsonElement gridCellWidthElement)
            && gridCellWidthElement.TryGetInt32(out int gridCellWidth))
        {
            fallback.GridCellWidth = gridCellWidth;
        }
        if (root.TryGetProperty("gridCellHeight", out JsonElement gridCellHeightElement)
            && gridCellHeightElement.TryGetInt32(out int gridCellHeight))
        {
            fallback.GridCellHeight = gridCellHeight;
        }
        if (root.TryGetProperty("gridCellsX", out JsonElement gridCellsXElement)
            && gridCellsXElement.TryGetInt32(out int gridCellsX))
        {
            fallback.GridCellsX = gridCellsX;
        }
        if (root.TryGetProperty("gridCellsY", out JsonElement gridCellsYElement)
            && gridCellsYElement.TryGetInt32(out int gridCellsY))
        {
            fallback.GridCellsY = gridCellsY;
        }
        
        return fallback;

        #endregion Fallback Read
    }

    public override void Write(Utf8JsonWriter writer, OgmoLayerData value, JsonSerializerOptions options)
    {
        if (value is OgmoTileLayerData tileLayer)
        {
            JsonSerializer.Serialize(writer, tileLayer, OgmoJsonSerializerContext.Default.OgmoTileLayerData);
        }
        else if (value is OgmoGridLayerData gridLayer)
        {
            JsonSerializer.Serialize(writer, gridLayer, OgmoJsonSerializerContext.Default.OgmoGridLayerData);
        }
        else if (value is OgmoEntityLayerData entityLayer)
        {
            JsonSerializer.Serialize(writer, entityLayer, OgmoJsonSerializerContext.Default.OgmoEntityLayerData);
        }
        else if (value is OgmoDecalLayerData decalLayer)
        {
            JsonSerializer.Serialize(writer, decalLayer, OgmoJsonSerializerContext.Default.OgmoDecalLayerData);
        }
        else
        {
            #region Fallback Write
            
            // Fallback: Manually write base OgmoLayerData to avoid StackOverflowException.
            writer.WriteStartObject();
            
            writer.WriteString("name", value.Name);
            writer.WriteString("_eid", value.ExportID);
            writer.WriteNumber("offsetX", value.OffsetX);
            writer.WriteNumber("offsetY", value.OffsetY);
            writer.WriteNumber("gridCellWidth", value.GridCellWidth);
            writer.WriteNumber("gridCellHeight", value.GridCellHeight);
            writer.WriteNumber("gridCellsX", value.GridCellsX);
            writer.WriteNumber("gridCellsY", value.GridCellsY);
            
            writer.WriteEndObject();

            #endregion Fallback Write
        }
    }
}
