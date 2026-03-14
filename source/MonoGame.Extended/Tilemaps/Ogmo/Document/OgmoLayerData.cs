using System.Text.Json.Serialization;
using MonoGame.Extended.Tilemaps.Ogmo;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

[JsonConverter(typeof(OgmoLayerDataConverter))]
internal class OgmoLayerData
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("_eid")]
    public string ExportID { get; set; }

    [JsonPropertyName("offsetX")]
    public float OffsetX { get; set; }

    [JsonPropertyName("offsetY")]
    public float OffsetY { get; set; }

    [JsonPropertyName("gridCellWidth")]
    public int GridCellWidth { get; set; }

    [JsonPropertyName("gridCellHeight")]
    public int GridCellHeight { get; set; }

    [JsonPropertyName("gridCellsX")]
    public int GridCellsX { get; set; }

    [JsonPropertyName("gridCellsY")]
    public int GridCellsY { get; set; }
}
