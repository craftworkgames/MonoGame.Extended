using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkWorld
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("iid")]
    public string Iid { get; set; }

    [JsonPropertyName("levels")]
    public List<LDtkLevel> Levels { get; set; } = new List<LDtkLevel>();

    [JsonPropertyName("worldGridWidth")]
    public int WorldGridWidth { get; set; }

    [JsonPropertyName("worldGridHeight")]
    public int WorldGridHeight { get; set; }

    [JsonPropertyName("worldLayout")]
    public string WorldLayout { get; set; }

    [JsonPropertyName("defaultLevelWidth")]
    public int DefaultLevelWidth { get; set; }

    [JsonPropertyName("defaultLevelHeight")]
    public int DefaultLevelHeight { get; set; }
}
