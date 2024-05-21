using System;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiTextureAtlasJsonConverter : ContentManagerJsonConverter<TextureAtlas>
    {
        private readonly IGuiTextureRegionService _textureRegionService;

        public GuiTextureAtlasJsonConverter(ContentManager contentManager, IGuiTextureRegionService textureRegionService)
            : base(contentManager, atlas => atlas.Name)
        {
            _textureRegionService = textureRegionService;
        }

        /// <inheritdoc />
        public override TextureAtlas Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var textureAtlas = base.Read(ref reader, typeToConvert, options);
            if (textureAtlas is not null)
            {
                _textureRegionService.TextureAtlases.Add(textureAtlas);
            }

            return textureAtlas;
        }
    }
}
