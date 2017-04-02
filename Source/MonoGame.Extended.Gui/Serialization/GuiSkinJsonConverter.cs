using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public interface IGuiSkinService
    {
        GuiSkin Skin { get; set; }
    }

    public class GuiSkinService : IGuiSkinService
    {
        public GuiSkin Skin { get; set; }
    }

    public class GuiSkinJsonConverter : JsonConverter
    {
        private readonly ContentManager _contentManager;
        private readonly IGuiSkinService _skinService;

        public GuiSkinJsonConverter(ContentManager contentManager, IGuiSkinService skinService)
        {
            _contentManager = contentManager;
            _skinService = skinService;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(string))
            {
                var assetName = (string) reader.Value;

                // TODO: Load this using the ContentManager instead.
                using (var stream = TitleContainer.OpenStream(assetName))
                {
                    var skin = GuiSkin.FromStream(stream, _contentManager);
                    _skinService.Skin = skin;
                    return skin;
                }
            }

            throw new InvalidOperationException($"{nameof(GuiSkinJsonConverter)} can only convert from a string");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiSkin);
        }
    }
}