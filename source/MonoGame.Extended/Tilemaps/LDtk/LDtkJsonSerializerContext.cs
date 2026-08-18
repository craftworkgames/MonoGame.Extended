using System.Text.Json.Serialization;
using MonoGame.Extended.Tilemaps.LDtk.Document;

namespace MonoGame.Extended.Tilemaps.LDtk;

/// <summary>
/// Source-generated <see cref="System.Text.Json.JsonSerializer"/> context for LDtk documents.
/// <para>
/// Enables Native AOT compatibility.
/// </para>
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowReadingFromString)]
[JsonSerializable(typeof(LDtkProject))]
[JsonSerializable(typeof(LDtkLevel))]
internal partial class LDtkJsonSerializerContext : JsonSerializerContext
{
}
