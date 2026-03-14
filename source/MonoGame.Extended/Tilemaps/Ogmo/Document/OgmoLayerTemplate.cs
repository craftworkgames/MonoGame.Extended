using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoLayerTemplate
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("definition")]
    public string Definition { get; set; }

    [JsonPropertyName("gridSize")]
    public OgmoVector2 GridSize { get; set; }

    [JsonPropertyName("exportID")]
    public string ExportID { get; set; }

    [JsonPropertyName("exportMode")]
    public int ExportMode { get; set; }

    [JsonPropertyName("arrayMode")]
    public int ArrayMode { get; set; }

    [JsonPropertyName("defaultTileset")]
    public string DefaultTileset { get; set; }

    [JsonPropertyName("legend")]
    public Dictionary<string, string> Legend { get; set; }

    [JsonPropertyName("requiredTags")]
    public List<string> RequiredTags { get; set; }

    [JsonPropertyName("excludedTags")]
    public List<string> ExcludedTags { get; set; }

    [JsonPropertyName("folder")]
    public string Folder { get; set; }

    [JsonPropertyName("includeImageSequence")]
    public bool IncludeImageSequence { get; set; }

    [JsonPropertyName("scaleable")]
    public bool Scaleable { get; set; }

    [JsonPropertyName("rotatable")]
    public bool Rotatable { get; set; }

    [JsonPropertyName("values")]
    public List<OgmoValueTemplate> Values { get; set; }
}
