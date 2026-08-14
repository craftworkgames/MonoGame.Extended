using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoValueTemplate
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("definition")]
    public string Definition { get; set; }

    [JsonPropertyName("defaults")]
    public JsonElement? Defaults { get; set; }

    [JsonPropertyName("bounded")]
    public bool Bounded { get; set; }

    [JsonPropertyName("min")]
    public float? Min { get; set; }

    [JsonPropertyName("max")]
    public float? Max { get; set; }

    [JsonPropertyName("maxLength")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("trimWhitespace")]
    public bool TrimWhitespace { get; set; }

    [JsonPropertyName("choices")]
    public List<string> Choices { get; set; }

    [JsonPropertyName("includeAlpha")]
    public bool IncludeAlpha { get; set; }
}
