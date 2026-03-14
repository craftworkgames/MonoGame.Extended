using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

/// <summary>
/// Represents instance data for a table of contents entry in LDtk.
/// </summary>
public sealed class LDtkTocInstanceData
{
    /// <summary>
    /// Gets or sets the IID information for this instance.
    /// </summary>
    [JsonPropertyName("iids")]
    public LDtkEntityReferenceInfos Iids { get; set; }

    /// <summary>
    /// Gets or sets the world X coordinate in pixels.
    /// </summary>
    [JsonPropertyName("worldX")]
    public int WorldX { get; set; }

    /// <summary>
    /// Gets or sets the world Y coordinate in pixels.
    /// </summary>
    [JsonPropertyName("worldY")]
    public int WorldY { get; set; }

    /// <summary>
    /// Gets or sets the width in pixels.
    /// </summary>
    [JsonPropertyName("widPx")]
    public int WidPx { get; set; }

    /// <summary>
    /// Gets or sets the height in pixels.
    /// </summary>
    [JsonPropertyName("heiPx")]
    public int HeiPx { get; set; }

    /// <summary>
    /// Gets or sets the entity custom field values (exportToToc fields only).
    /// </summary>
    [JsonPropertyName("fields")]
    public JsonElement? Fields { get; set; }
}
