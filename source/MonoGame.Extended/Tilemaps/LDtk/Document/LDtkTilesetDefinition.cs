using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkTilesetDefinition
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("relPath")]
    public string RelPath { get; set; }

    [JsonPropertyName("embedAtlas")]
    public string EmbedAtlas { get; set; }

    [JsonPropertyName("pxWid")]
    public int PxWid { get; set; }

    [JsonPropertyName("pxHei")]
    public int PxHei { get; set; }

    [JsonPropertyName("tileGridSize")]
    public int TileGridSize { get; set; }

    [JsonPropertyName("spacing")]
    public int Spacing { get; set; }

    [JsonPropertyName("padding")]
    public int Padding { get; set; }

    [JsonPropertyName("__cWid")]
    public int CWid { get; set; }

    [JsonPropertyName("__cHei")]
    public int CHei { get; set; }
}
