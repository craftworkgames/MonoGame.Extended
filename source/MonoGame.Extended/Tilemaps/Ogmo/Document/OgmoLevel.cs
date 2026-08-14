using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoLevel
{
    [JsonPropertyName("ogmoVersion")]
    public string OgmoVersion { get; set; }

    [JsonPropertyName("width")]
    public float Width { get; set; }

    [JsonPropertyName("height")]
    public float Height { get; set; }

    [JsonPropertyName("offsetX")]
    public float OffsetX { get; set; }

    [JsonPropertyName("offsetY")]
    public float OffsetY { get; set; }

    [JsonPropertyName("values")]
    public Dictionary<string, JsonElement> Values { get; set; }

    [JsonPropertyName("layers")]
    public List<OgmoLayerData> Layers { get; set; }
}
