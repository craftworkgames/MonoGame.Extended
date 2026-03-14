using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

/// <summary>
/// Represents entity reference information in LDtk, identifying an entity instance by its IIDs.
/// </summary>
public sealed class LDtkEntityReferenceInfos
{
    /// <summary>
    /// Gets or sets the entity instance identifier.
    /// </summary>
    [JsonPropertyName("entityIid")]
    public string EntityIid { get; set; }

    /// <summary>
    /// Gets or sets the layer instance identifier containing the entity.
    /// </summary>
    [JsonPropertyName("layerIid")]
    public string LayerIid { get; set; }

    /// <summary>
    /// Gets or sets the level instance identifier containing the entity.
    /// </summary>
    [JsonPropertyName("levelIid")]
    public string LevelIid { get; set; }

    /// <summary>
    /// Gets or sets the world instance identifier containing the entity.
    /// </summary>
    [JsonPropertyName("worldIid")]
    public string WorldIid { get; set; }
}
