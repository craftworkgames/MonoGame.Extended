using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkAutoLayerRuleGroup
{
    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("rules")]
    public List<LDtkAutoLayerRule> Rules { get; set; } = new List<LDtkAutoLayerRule>();
}
