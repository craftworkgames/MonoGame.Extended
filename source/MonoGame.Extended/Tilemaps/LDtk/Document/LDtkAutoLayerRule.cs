using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkAutoLayerRule
{
    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("tileRectsIds")]
    public List<List<int>> TileRectsIds { get; set; } = new List<List<int>>();

    [JsonPropertyName("pattern")]
    public List<int> Pattern { get; set; } = new List<int>();
}
