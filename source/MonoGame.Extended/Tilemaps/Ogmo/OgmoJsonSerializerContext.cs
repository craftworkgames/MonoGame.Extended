using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MonoGame.Extended.Tilemaps.Ogmo.Document;

namespace MonoGame.Extended.Tilemaps.Ogmo;

/// <summary>
/// Source-generated <see cref="System.Text.Json.JsonSerializer"/> context for Ogmo documents.
/// <para>
/// Enables Native AOT compatibility.
/// </para>
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Skip)]
[JsonSerializable(typeof(OgmoProject))]
[JsonSerializable(typeof(OgmoLevel))]
[JsonSerializable(typeof(OgmoTileLayerData))]
[JsonSerializable(typeof(OgmoGridLayerData))]
[JsonSerializable(typeof(OgmoEntityLayerData))]
[JsonSerializable(typeof(OgmoDecalLayerData))]
[JsonSerializable(typeof(OgmoLayerData))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<List<string>>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class OgmoJsonSerializerContext : JsonSerializerContext { }
