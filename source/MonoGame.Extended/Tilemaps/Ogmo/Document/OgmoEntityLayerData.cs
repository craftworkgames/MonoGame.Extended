using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoEntityLayerData : OgmoLayerData
{
    [JsonPropertyName("entities")]
    public List<OgmoEntity> Entities { get; set; }
}
