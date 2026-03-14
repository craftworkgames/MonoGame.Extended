using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkTileInstance
{
    [JsonPropertyName("px")]
    public List<int> Px { get; set; } = new List<int>();

    [JsonPropertyName("src")]
    public List<int> Src { get; set; } = new List<int>();

    [JsonPropertyName("f")]
    public int FlipFlags { get; set; }

    [JsonPropertyName("t")]
    public int TileId { get; set; }

    [JsonPropertyName("d")]
    public List<int> Data { get; set; } = new List<int>();

    [JsonPropertyName("a")]
    public float Alpha { get; set; } = 1.0f;
}
