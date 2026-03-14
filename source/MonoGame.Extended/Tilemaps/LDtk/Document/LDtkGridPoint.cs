using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkGridPoint
{
    [JsonPropertyName("cx")]
    public int Cx { get; set; }

    [JsonPropertyName("cy")]
    public int Cy { get; set; }
}
