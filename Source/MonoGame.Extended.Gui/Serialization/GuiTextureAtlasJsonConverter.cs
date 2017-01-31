using System;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var textureAtlas = base.ReadJson(reader, objectType, existingValue, serializer) as TextureAtlas;

            if (textureAtlas != null)
                _textureRegionService.TextureAtlases.Add(textureAtlas);

            return textureAtlas;
        }
    }
}