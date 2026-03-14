using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Document model for the generic <c>.tilemapworld</c> JSON format, used when an editor
/// does not provide a native world file (for example, Ogmo Editor).
/// </summary>
internal sealed class TilemapWorldDefinition
{
    /// <summary>
    /// Gets or sets the editor format. Supported values: <c>tiled</c>, <c>ogmo</c>.
    /// </summary>
    [JsonPropertyName("format")]
    public string Format { get; set; }

    /// <summary>
    /// Gets or sets the path to the editor project file, relative to the .tilemapworld file.
    /// Required for formats that use a separate project file (for example, Ogmo's .ogmo file).
    /// </summary>
    [JsonPropertyName("project")]
    public string Project { get; set; }

    [JsonPropertyName("maps")]
    public List<TilemapWorldDefinitionMap> Maps { get; set; } = new List<TilemapWorldDefinitionMap>();
}

/// <summary>
/// A single map entry in a <c>.tilemapworld</c> definition file.
/// </summary>
internal sealed class TilemapWorldDefinitionMap
{
    /// <summary>
    /// Gets or sets the path to the level file, relative to the .tilemapworld file.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("depth")]
    public int Depth { get; set; }
}
