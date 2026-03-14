using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkNeighbourLevel
{
    [JsonPropertyName("dir")]
    public string Dir { get; set; }

    [JsonPropertyName("levelIid")]
    public string LevelIid { get; set; }

    [JsonPropertyName("levelUid")]
    public int? LevelUid { get; set; }
}
