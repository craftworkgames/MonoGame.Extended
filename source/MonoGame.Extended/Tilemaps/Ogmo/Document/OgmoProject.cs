using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoProject
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("ogmoVersion")]
    public string OgmoVersion { get; set; }

    [JsonPropertyName("levelPaths")]
    public List<string> LevelPaths { get; set; }

    [JsonPropertyName("backgroundColor")]
    public string BackgroundColor { get; set; }

    [JsonPropertyName("gridColor")]
    public string GridColor { get; set; }

    [JsonPropertyName("anglesRadians")]
    public bool AnglesRadians { get; set; }

    [JsonPropertyName("directoryDepth")]
    public int DirectoryDepth { get; set; }

    [JsonPropertyName("layerGridDefaultSize")]
    public OgmoVector2 LayerGridDefaultSize { get; set; }

    [JsonPropertyName("levelDefaultSize")]
    public OgmoVector2 LevelDefaultSize { get; set; }

    [JsonPropertyName("levelMinSize")]
    public OgmoVector2 LevelMinSize { get; set; }

    [JsonPropertyName("levelMaxSize")]
    public OgmoVector2 LevelMaxSize { get; set; }

    [JsonPropertyName("levelValues")]
    public List<OgmoValueTemplate> LevelValues { get; set; }

    [JsonPropertyName("defaultExportMode")]
    public string DefaultExportMode { get; set; }

    [JsonPropertyName("compactExport")]
    public bool CompactExport { get; set; }

    [JsonPropertyName("entityTags")]
    public List<string> EntityTags { get; set; }

    [JsonPropertyName("layers")]
    public List<OgmoLayerTemplate> Layers { get; set; }

    [JsonPropertyName("entities")]
    public List<OgmoEntityTemplate> Entities { get; set; }

    [JsonPropertyName("tilesets")]
    public List<OgmoTilesetTemplate> Tilesets { get; set; }
}
