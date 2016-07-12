using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Demo.Gui.Wip
{
    public class GuiDrawableJsonConverter : JsonConverter
    {
        private readonly GuiJsonConverterService _converterService;

        public GuiDrawableJsonConverter(GuiJsonConverterService converterService)
        {
            _converterService = converterService;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IGuiDrawable);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var type = (string)jObject.Property("Type");

            switch (type)
            {
                case "Sprite":
                    var textureRegion = (string)jObject.Property("TextureRegion");
                    var color = jObject.Property("Color")?.Value?.ToObject<Color>(serializer);
                    return new GuiSprite
                    {
                        TextureRegion = _converterService.GetTextureRegion(textureRegion),
                        Color = color ?? Color.White,
                        //HorizontalAlignment = (GuiHorizontalAlignment) Enum.Parse(typeof(GuiHorizontalAlignment), (string) jObject.Property("HorizontalAlignment"))
                    };
                case "Text":
                    var font = (string)jObject.Property("Font");
                    var text = (string)jObject.Property("Text");
                    var textColor = jObject.Property("Color")?.Value?.ToObject<Color>(serializer);
                    return new GuiText { Font = _converterService.GetFont(font), Text = text, Color = textColor ?? Color.White };
            }

            return null;
        }
    }
}