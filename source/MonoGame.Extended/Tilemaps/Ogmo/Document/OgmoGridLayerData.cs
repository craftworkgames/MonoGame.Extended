using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoGridLayerData : OgmoLayerData
{
    [JsonPropertyName("grid")]
    public List<string> Grid { get; set; }

    [JsonPropertyName("grid2D")]
    public List<List<string>> Grid2D { get; set; }

    [JsonPropertyName("arrayMode")]
    public int ArrayMode { get; set; }
}
