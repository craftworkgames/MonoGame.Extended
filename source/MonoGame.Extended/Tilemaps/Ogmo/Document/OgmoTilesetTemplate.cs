using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoTilesetTemplate
{
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; }

    [JsonPropertyName("tileWidth")]
    public int TileWidth { get; set; }

    [JsonPropertyName("tileHeight")]
    public int TileHeight { get; set; }

    [JsonPropertyName("tileSeparationX")]
    public int TileSeparationX { get; set; }

    [JsonPropertyName("tileSeparationY")]
    public int TileSeparationY { get; set; }
}
