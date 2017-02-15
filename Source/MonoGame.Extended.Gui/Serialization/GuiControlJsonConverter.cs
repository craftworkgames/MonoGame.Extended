using System;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiControlJsonConverter : JsonConverter
    {
        private readonly IGuiSkinService _guiSkinService;
        private readonly GuiControlStyleJsonConverter _styleConverter = new GuiControlStyleJsonConverter();
        private const string _styleProperty = "Style";

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
            var style = (GuiControlStyle) _styleConverter.ReadJson(reader, objectType, existingValue, serializer);
            var skin = (string) style[_styleProperty];
            var control = controlFactory.Create(style.TargetType, skin);

            object childControls;

            if (style.TryGetValue(nameof(GuiControl.Controls), out childControls))
            {
                var controlCollection = childControls as GuiControlCollection;

                if (controlCollection != null)
                {
                    foreach (var child in controlCollection)
                        control.Controls.Add(child);
                }
            }

            style.Apply(control);
            return control;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiControl);
        }
    }
}