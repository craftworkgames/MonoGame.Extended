using System;
using System.IO;
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
                var skinSerializer = new GuiJsonSerializer(_contentManager);

                // TODO: Load this using the ContentManager instead.
                using (var stream = TitleContainer.OpenStream(assetName))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var skin = skinSerializer.Deserialize<GuiSkin>(jsonReader);
                    _skinService.Skin = skin;
                    return skin;
                }
            }

            throw new InvalidOperationException("This converter can only convert from a string");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiSkin);
        }
    }
}