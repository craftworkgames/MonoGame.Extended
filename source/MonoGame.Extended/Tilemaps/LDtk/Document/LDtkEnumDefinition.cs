using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkEnumDefinition
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("values")]
    public List<LDtkEnumValue> Values { get; set; } = new List<LDtkEnumValue>();

    [JsonPropertyName("externalRelPath")]
    public string ExternalRelPath { get; set; }
}
