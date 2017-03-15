using System;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiNinePatchRegion2DJsonConverter : NinePatchRegion2DJsonConverter
    {
        private readonly IGuiTextureRegionService _textureRegionService;

        public GuiNinePatchRegion2DJsonConverter(IGuiTextureRegionService textureRegionService)
            : base(textureRegionService)
        {
            _textureRegionService = textureRegionService;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var ninePatch = base.ReadJson(reader, objectType, existingValue, serializer) as NinePatchRegion2D;

            if(ninePatch != null)
                _textureRegionService.NinePatches.Add(ninePatch);

            return ninePatch;
        }
    }
}