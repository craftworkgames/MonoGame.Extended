using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkFieldDefinition
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("__type")]
    public string Type { get; set; }

    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("type")]
    public string TypeEnum { get; set; }

    [JsonPropertyName("isArray")]
    public bool IsArray { get; set; }

    [JsonPropertyName("canBeNull")]
    public bool CanBeNull { get; set; }
}
