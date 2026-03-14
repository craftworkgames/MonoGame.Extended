using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkProject
{
    [JsonPropertyName("iid")]
    public string Iid { get; set; }

    [JsonPropertyName("jsonVersion")]
    public string JsonVersion { get; set; }

    [JsonPropertyName("appBuildId")]
    public int AppBuildId { get; set; }

    [JsonPropertyName("nextUid")]
    public int NextUid { get; set; }

    [JsonPropertyName("identifierStyle")]
    public string IdentifierStyle { get; set; }

    [JsonPropertyName("worldLayout")]
    public string WorldLayout { get; set; }

    [JsonPropertyName("worldGridWidth")]
    public int? WorldGridWidth { get; set; }

    [JsonPropertyName("worldGridHeight")]
    public int? WorldGridHeight { get; set; }

    [JsonPropertyName("defaultLevelWidth")]
    public int? DefaultLevelWidth { get; set; }

    [JsonPropertyName("defaultLevelHeight")]
    public int? DefaultLevelHeight { get; set; }

    [JsonPropertyName("defaultGridSize")]
    public int DefaultGridSize { get; set; }

    [JsonPropertyName("defaultPivotX")]
    public float DefaultPivotX { get; set; }

    [JsonPropertyName("defaultPivotY")]
    public float DefaultPivotY { get; set; }

    [JsonPropertyName("bgColor")]
    public string BgColor { get; set; }

    [JsonPropertyName("defaultLevelBgColor")]
    public string DefaultLevelBgColor { get; set; }

    [JsonPropertyName("externalLevels")]
    public bool ExternalLevels { get; set; }

    [JsonPropertyName("levels")]
    public List<LDtkLevel> Levels { get; set; } = new List<LDtkLevel>();

    [JsonPropertyName("defs")]
    public LDtkDefinitions Defs { get; set; }

    [JsonPropertyName("worlds")]
    public List<LDtkWorld> Worlds { get; set; } = new List<LDtkWorld>();

    [JsonPropertyName("dummyWorldIid")]
    public string DummyWorldIid { get; set; }

    [JsonPropertyName("toc")]
    public List<LDtkTableOfContentEntry> Toc { get; set; } = new List<LDtkTableOfContentEntry>();
}
