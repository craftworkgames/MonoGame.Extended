using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkLevel
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("iid")]
    public string Iid { get; set; }

    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("worldX")]
    public int WorldX { get; set; }

    [JsonPropertyName("worldY")]
    public int WorldY { get; set; }

    [JsonPropertyName("worldDepth")]
    public int WorldDepth { get; set; }

    [JsonPropertyName("pxWid")]
    public int PxWid { get; set; }

    [JsonPropertyName("pxHei")]
    public int PxHei { get; set; }

    [JsonPropertyName("__bgColor")]
    public string BgColor { get; set; }

    [JsonPropertyName("bgColor")]
    public string BgColorOverride { get; set; }

    [JsonPropertyName("bgRelPath")]
    public string BgRelPath { get; set; }

    [JsonPropertyName("bgPos")]
    public string BgPos { get; set; }

    [JsonPropertyName("bgPivotX")]
    public float BgPivotX { get; set; }

    [JsonPropertyName("bgPivotY")]
    public float BgPivotY { get; set; }

    [JsonPropertyName("externalRelPath")]
    public string ExternalRelPath { get; set; }

    [JsonPropertyName("fieldInstances")]
    public List<LDtkFieldInstance> FieldInstances { get; set; } = new List<LDtkFieldInstance>();

    [JsonPropertyName("layerInstances")]
    public List<LDtkLayerInstance> LayerInstances { get; set; }

    [JsonPropertyName("__neighbours")]
    public List<LDtkNeighbourLevel> Neighbours { get; set; } = new List<LDtkNeighbourLevel>();
}
