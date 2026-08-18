using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("_eid")]
    public string ExportID { get; set; }

    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    [JsonPropertyName("width")]
    public float Width { get; set; }

    [JsonPropertyName("height")]
    public float Height { get; set; }

    [JsonPropertyName("originX")]
    public float OriginX { get; set; }

    [JsonPropertyName("originY")]
    public float OriginY { get; set; }

    [JsonPropertyName("rotation")]
    public float Rotation { get; set; }

    [JsonPropertyName("flippedX")]
    public bool FlippedX { get; set; }

    [JsonPropertyName("flippedY")]
    public bool FlippedY { get; set; }

    [JsonPropertyName("nodes")]
    public List<OgmoVector2> Nodes { get; set; }

    [JsonPropertyName("values")]
    public Dictionary<string, JsonElement> Values { get; set; }
}
