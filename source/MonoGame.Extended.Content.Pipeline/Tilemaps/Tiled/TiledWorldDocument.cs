using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps.Tiled;

/// <summary>
/// Document model for the Tiled native <c>.world</c> JSON format.
/// </summary>
internal sealed class TiledWorldDocument
{
    [JsonPropertyName("maps")]
    public List<TiledWorldMapEntry> Maps { get; set; } = new List<TiledWorldMapEntry>();
}

/// <summary>
/// A single map entry in a Tiled <c>.world</c> file.
/// </summary>
internal sealed class TiledWorldMapEntry
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}
