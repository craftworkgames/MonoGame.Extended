using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoDecalLayerData : OgmoLayerData
{
    [JsonPropertyName("decals")]
    public List<OgmoDecal> Decals { get; set; }

    [JsonPropertyName("folder")]
    public string Folder { get; set; }
}
