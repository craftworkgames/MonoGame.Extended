using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Gui.Serialization;

public interface IGuiSkinService
{
    Skin Skin { get; set; }
}

public class SkinService : IGuiSkinService
{
    public Skin Skin { get; set; }
}

public class SkinJsonConverter : JsonConverter<Skin>
{
    private readonly ContentManager _contentManager;
    private readonly IGuiSkinService _skinService;
    private readonly Type[] _customControlTypes;

    public SkinJsonConverter(ContentManager contentManager, IGuiSkinService skinService, params Type[] customControlTypes)
    {
        _contentManager = contentManager;
        _skinService = skinService;
        _customControlTypes = customControlTypes;
    }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Skin);

    /// <inheritdoc />
    public override Skin Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var assetName = reader.GetString();

            // TODO: Load this using the ContentManager instead.
            using (var stream = TitleContainer.OpenStream(assetName))
            {
                var skin = Skin.FromStream(_contentManager, stream, _customControlTypes);
                _skinService.Skin = skin;
                return skin;
            }

        }

        throw new InvalidOperationException($"{nameof(SkinJsonConverter)} can only convert from a string");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Skin value, JsonSerializerOptions options) { }
}
