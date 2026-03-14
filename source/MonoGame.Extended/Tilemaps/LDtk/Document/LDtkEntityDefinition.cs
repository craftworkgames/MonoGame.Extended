using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkEntityDefinition
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; }

    [JsonPropertyName("pivotX")]
    public float PivotX { get; set; }

    [JsonPropertyName("pivotY")]
    public float PivotY { get; set; }

    [JsonPropertyName("fieldDefs")]
    public List<LDtkFieldDefinition> FieldDefs { get; set; } = new List<LDtkFieldDefinition>();

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new List<string>();
}
