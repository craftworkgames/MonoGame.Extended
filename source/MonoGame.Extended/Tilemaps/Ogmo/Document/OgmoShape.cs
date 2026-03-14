using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoShape
{
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("points")]
    public List<OgmoVector2> Points { get; set; }
}
