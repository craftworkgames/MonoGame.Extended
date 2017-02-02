using System;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiControlJsonConverter : JsonConverter
    {
        private readonly IGuiSkinService _guiSkinService;
        private readonly GuiControlStyleJsonConverter _styleConverter = new GuiControlStyleJsonConverter();

        public GuiControlJsonConverter(IGuiSkinService guiSkinService)
        {
            _guiSkinService = guiSkinService;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var controlFactory = new GuiControlFactory(_guiSkinService.Skin);
            var style = _styleConverter.ReadJson(reader, objectType, existingValue, serializer) as GuiControlStyle;
            var control = controlFactory.Create<GuiButton>("white-button", c => style?.Apply(c)); 
            return control;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiControl);
        }
    }
}