using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoTileLayerData : OgmoLayerData
{
    [JsonPropertyName("tileset")]
    public string Tileset { get; set; }

    [JsonPropertyName("data")]
    public List<int> Data { get; set; }

    [JsonPropertyName("data2D")]
    public List<List<int>> Data2D { get; set; }

    [JsonPropertyName("dataCoords")]
    [JsonConverter(typeof(OgmoTileCoordListConverter))]
    public List<Point?> DataCoords { get; set; }

    [JsonPropertyName("dataCoords2D")]
    [JsonConverter(typeof(OgmoTileCoordList2DConverter))]
    public List<List<Point?>> DataCoords2D { get; set; }

    [JsonPropertyName("exportMode")]
    public int ExportMode { get; set; }

    [JsonPropertyName("arrayMode")]
    public int ArrayMode { get; set; }
}
