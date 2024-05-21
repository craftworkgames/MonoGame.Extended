using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui.Serialization
{
    public class ControlJsonConverter : JsonConverter<Control>
    {
        private readonly IGuiSkinService _guiSkinService;
        private readonly ControlStyleJsonConverter _styleConverter;
        private const string _styleProperty = "Style";

        public ControlJsonConverter(IGuiSkinService guiSkinService, params Type[] customControlTypes)
        {
            _guiSkinService = guiSkinService;
            _styleConverter = new ControlStyleJsonConverter(customControlTypes);
        }

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Control);

        /// <inheritdoc />
        public override Control Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var style = _styleConverter.Read(ref reader, typeToConvert, options);
            var template = GetControlTemplate(style);
            var skin = _guiSkinService.Skin;
            var control = skin.Create(style.TargetType, template);

            var itemsControl = control as ItemsControl;
            if (itemsControl != null)
            {
                object childControls;

                if (style.TryGetValue(nameof(ItemsControl.Items), out childControls))
                {
                    var controlCollection = childControls as ControlCollection;

                    if (controlCollection != null)
                    {
                        foreach (var child in controlCollection)
                            itemsControl.Items.Add(child);
                    }
                }
            }

            style.Apply(control);
            return control;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Control value, JsonSerializerOptions options) { }



        private static string GetControlTemplate(ControlStyle style)
        {
            object template;

            if (style.TryGetValue(_styleProperty, out template))
                return template as string;

            return null;
        }
    }
}
