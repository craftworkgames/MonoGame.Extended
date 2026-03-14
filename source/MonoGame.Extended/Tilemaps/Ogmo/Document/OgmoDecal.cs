using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoDecal
{
    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    [JsonPropertyName("texture")]
    public string Texture { get; set; }

    [JsonPropertyName("rotation")]
    public float Rotation { get; set; }

    [JsonPropertyName("scaleX")]
    public float ScaleX { get; set; }

    [JsonPropertyName("scaleY")]
    public float ScaleY { get; set; }

    [JsonPropertyName("values")]
    public Dictionary<string, object> Values { get; set; }
}
