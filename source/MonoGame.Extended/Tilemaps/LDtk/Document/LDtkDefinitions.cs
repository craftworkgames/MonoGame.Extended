using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkDefinitions
{
    [JsonPropertyName("layers")]
    public List<LDtkLayerDefinition> Layers { get; set; } = new List<LDtkLayerDefinition>();

    [JsonPropertyName("entities")]
    public List<LDtkEntityDefinition> Entities { get; set; } = new List<LDtkEntityDefinition>();

    [JsonPropertyName("tilesets")]
    public List<LDtkTilesetDefinition> Tilesets { get; set; } = new List<LDtkTilesetDefinition>();

    [JsonPropertyName("enums")]
    public List<LDtkEnumDefinition> Enums { get; set; } = new List<LDtkEnumDefinition>();

    [JsonPropertyName("externalEnums")]
    public List<LDtkEnumDefinition> ExternalEnums { get; set; } = new List<LDtkEnumDefinition>();

    [JsonPropertyName("levelFields")]
    public List<LDtkFieldDefinition> LevelFields { get; set; } = new List<LDtkFieldDefinition>();
}
