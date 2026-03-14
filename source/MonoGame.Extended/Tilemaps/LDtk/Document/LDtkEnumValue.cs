using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkEnumValue
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("color")]
    public int Color { get; set; }
}
