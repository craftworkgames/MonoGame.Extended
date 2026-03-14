using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkTableOfContentEntry
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("instancesData")]
    public List<LDtkTocInstanceData> InstancesData { get; set; } = new List<LDtkTocInstanceData>();
}
