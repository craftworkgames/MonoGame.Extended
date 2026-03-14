using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkFieldInstance
{
    [JsonPropertyName("__identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("__type")]
    public string Type { get; set; }

    [JsonPropertyName("__value")]
    public JsonElement? Value { get; set; }

    [JsonPropertyName("defUid")]
    public int DefUid { get; set; }

    [JsonPropertyName("realEditorValues")]
    public List<object> RealEditorValues { get; set; } = new List<object>();
}
