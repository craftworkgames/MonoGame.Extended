using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Wip;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui
{
    public class GuiLayoutJsonConverter : JsonConverter
    {
        private readonly ContentManager _contentManager;

        public GuiLayoutJsonConverter(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiLayout);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            serializer.Converters.Add(new ColorJsonConverter());
            serializer.Converters.Add(new Vector2JsonConverter());
            serializer.Converters.Add(new SizeFJsonConverter());
            serializer.Converters.Add(new GuiStyleSheetJsonConverter(_contentManager));

            var jObject = JObject.Load(reader);
            var styleSheetName = jObject.Property(nameof(GuiLayout.StyleSheet)).Value.ToObject<string>();
            var styleSheetPath = Path.Combine(_contentManager.RootDirectory, styleSheetName);

            using (var stream = TitleContainer.OpenStream(styleSheetPath))
            using(var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();
                var styleSheet = JsonConvert.DeserializeObject<GuiStyleSheet>(json, serializer.Converters.ToArray());
                serializer.Converters.Add(new GuiControlJsonConverter(styleSheet));

                return new GuiLayout
                {
                    StyleSheet = styleSheet,
                    Controls = jObject.Property(nameof(GuiLayout.Controls)).Value.ToObject<GuiControl[]>(serializer)
                };
            }
        }
    }
}