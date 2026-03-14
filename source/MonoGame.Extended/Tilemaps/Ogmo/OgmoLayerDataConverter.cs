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
            return JsonSerializer.Deserialize<OgmoTileLayerData>(root.GetRawText(), options);
        }
        else if (root.TryGetProperty("grid", out _) ||
                 root.TryGetProperty("grid2D", out _))
        {
            return JsonSerializer.Deserialize<OgmoGridLayerData>(root.GetRawText(), options);
        }
        else if (root.TryGetProperty("entities", out _))
        {
            return JsonSerializer.Deserialize<OgmoEntityLayerData>(root.GetRawText(), options);
        }
        else if (root.TryGetProperty("decals", out _))
        {
            return JsonSerializer.Deserialize<OgmoDecalLayerData>(root.GetRawText(), options);
        }

        // If we can't determine the type, return base layer data
        return JsonSerializer.Deserialize<OgmoLayerData>(root.GetRawText(), options);
    }

    public override void Write(Utf8JsonWriter writer, OgmoLayerData value, JsonSerializerOptions options)
    {
        if (value is OgmoTileLayerData tileLayer)
        {
            JsonSerializer.Serialize(writer, tileLayer, options);
        }
        else if (value is OgmoGridLayerData gridLayer)
        {
            JsonSerializer.Serialize(writer, gridLayer, options);
        }
        else if (value is OgmoEntityLayerData entityLayer)
        {
            JsonSerializer.Serialize(writer, entityLayer, options);
        }
        else if (value is OgmoDecalLayerData decalLayer)
        {
            JsonSerializer.Serialize(writer, decalLayer, options);
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
