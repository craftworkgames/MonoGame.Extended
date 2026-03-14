using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkEntityInstance
{
    [JsonPropertyName("__identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("iid")]
    public string Iid { get; set; }

    [JsonPropertyName("defUid")]
    public int DefUid { get; set; }

    [JsonPropertyName("px")]
    public List<int> Px { get; set; } = new List<int>();

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("__pivot")]
    public List<float> Pivot { get; set; } = new List<float>();

    [JsonPropertyName("__tags")]
    public List<string> Tags { get; set; } = new List<string>();

    [JsonPropertyName("fieldInstances")]
    public List<LDtkFieldInstance> FieldInstances { get; set; } = new List<LDtkFieldInstance>();
}
