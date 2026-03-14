using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkLayerDefinition
{
    [JsonPropertyName("__type")]
    public string Type { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("type")]
    public string TypeEnum { get; set; }

    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("gridSize")]
    public int GridSize { get; set; }

    [JsonPropertyName("displayOpacity")]
    public float DisplayOpacity { get; set; }

    [JsonPropertyName("pxOffsetX")]
    public int PxOffsetX { get; set; }

    [JsonPropertyName("pxOffsetY")]
    public int PxOffsetY { get; set; }

    [JsonPropertyName("parallaxFactorX")]
    public float ParallaxFactorX { get; set; }

    [JsonPropertyName("parallaxFactorY")]
    public float ParallaxFactorY { get; set; }

    [JsonPropertyName("tilesetDefUid")]
    public int? TilesetDefUid { get; set; }

    [JsonPropertyName("autoSourceLayerDefUid")]
    public int? AutoSourceLayerDefUid { get; set; }

    [JsonPropertyName("intGridValues")]
    public List<LDtkIntGridValueDefinition> IntGridValues { get; set; } = new List<LDtkIntGridValueDefinition>();

    [JsonPropertyName("autoRuleGroups")]
    public List<LDtkAutoLayerRuleGroup> AutoRuleGroups { get; set; } = new List<LDtkAutoLayerRuleGroup>();
}
