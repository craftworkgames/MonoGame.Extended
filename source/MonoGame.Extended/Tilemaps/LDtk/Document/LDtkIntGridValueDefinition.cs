using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkIntGridValueDefinition
{
    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; }
}
